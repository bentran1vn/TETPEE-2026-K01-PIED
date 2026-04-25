using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TetPee.Repository;
using TetPee.Repository.Entity;

namespace TetPee.Service.Order;

public class Service : IService
{
    private readonly AppDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContext;

    public Service(AppDbContext dbContext, IHttpContextAccessor httpContext)
    {
        _dbContext = dbContext;
        _httpContext = httpContext;
    }

    public async Task<Response.CreateOrderResponse> CreateOrder(Request.CreateOrderRequest request)
    {
        var userId = _httpContext.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "UserId")?.Value;

        var userIdGuid = Guid.Parse(userId!);
        
        // List Object -> List Guid => Mapping List thì xài Select
        var productIds = request.Products.Select(x => x.ProductId).Distinct().ToList();
        
        var query = _dbContext.Products.Where(x => productIds.Contains(x.Id));
        
        var productCount = await query.CountAsync();
        
        if(productCount != productIds.Count)
        {
            throw new Exception("Some products not found");
        }
        
        var result = await query.ToListAsync();

        decimal totalAmount = 0;

        foreach (var prod in result)
        {
            // Tìm trong list giỏ sàng sản phẩm để tìm số lượng mà khách hàng muốn mua !
            var quantity = request.Products.First(x => x.ProductId == prod.Id).Quantity;
            
            if(quantity <= 0)
            {
                throw new Exception($"Quantity of product {prod.Id} must be greater than 0");
            }
            
            totalAmount += prod.Price * quantity;
        }
        
        if(totalAmount <= 0)
        {
            throw new Exception("Total amount must be greater than 0");
        }
        
        var order = new Repository.Entity.Order()
        {
            Id = Guid.NewGuid(),
            UserId = userIdGuid,
            TotalAmount = totalAmount,
            Address = request.Address,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };
        
        _dbContext.Add(order);
        await _dbContext.SaveChangesAsync();
        
        List<OrderDetail> orderDetails = new List<OrderDetail>();
        
        foreach (var prod in result)
        {
            var quantity = request.Products.First(x => x.ProductId == prod.Id).Quantity;
            
            var orderDetail = new OrderDetail()
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                ProductId = prod.Id,
                Quantity = quantity,
                UnitPrice = prod.Price
            };
            
            orderDetails.Add(orderDetail);
        }

        if (orderDetails.Any())
        {
            _dbContext.AddRange(orderDetails);
            await _dbContext.SaveChangesAsync();
        }
        
        string description = $"TETPEE-{order.Id}";
        
        var response = new Response.CreateOrderResponse()
        {
            OrderId = order.Id,
            TotalAmount = order.TotalAmount,
            BankName = "BIDV",
            BankAccount = "96247BENTRAN",
            Description = description,
            QRCode = ""
        };
        
        string qrCode = $"https://qr.sepay.vn/img?" +
                        $"acc={response.BankAccount}&" +
                        $"bank={response.BankName}&" +
                        $"amount={(int)totalAmount}&" +
                        $"des={description}&" +
                        $"template=qronly";
        
        response.QRCode = qrCode;
        
        return response;
    }
    
    /*
     * {
     * "gateway":"BIDV",
     * "transactionDate":"2026-04-06 23:41:15",
     * "accountNumber":"8886369921",
     * "subAccount":"96247BENTRAN",
     * "code":"TCMPBf9c3895c14b94583bad78673263", // TETPEEORDERID
     * "content":"QR - TCMPBf9c3895c14b94583bad786732631b1ca", 
     * "transferType":"in",
     * "description":"BankAPINotify QR - TCMPBf9c3895c14b94583bad786732631b1ca",
     * "transferAmount":2500,
     * "referenceCode":"bc8af415-13e4-4bf9-8352-a8af59df5808",
     * "accumulated":0,
     * "id":48628369
     * }
     */
    public async Task SepayWebhookHandler(Request.SepayWebhookRequest request)
    {
        var description = request.Code; // TETPEEORDERID
        
        var raw = description.Replace("TETPEE", ""); // TETPEEORDERID -> ORDERID
        
        Guid? orderId = null;
        
        if (raw.Length == 32) 
            // Mặc định 1 Guid sẽ có 32 ký tự nếu không có dấu gạch nối,
            // còn nếu có dấu gạch nối thì sẽ có 36 ký tự
        {
            // Vì ORDERID theo format là không có dấu gạch ngang
            // Sample: 0101b85c-b450-4bb9-8226-0d02b0eb6e03
            var formatted = 
                            $"{raw.Substring(0, 8)}-" +
                            $"{raw.Substring(8, 4)}-" +
                            $"{raw.Substring(12, 4)}-" +
                            $"{raw.Substring(16, 4)}-" +
                            $"{raw.Substring(20, 12)}";
            
            // Id nhưng mà hiện nó đang theo string, chuẩn GUID rồi đó

            if (Guid.TryParse(formatted, out var guid))
            {
                orderId = guid;
            }
            
            // orderId = Guid.Parse(formatted);
        } else
        {
            throw new Exception("Invalid description format");
        }
        
        if(orderId == null)
        {
            throw new Exception("OrderId not found in description");
        }
        
        var query = _dbContext.Orders
            .Where(x => x.Id == orderId)
            .Include(x => x.OrderDetails);
        
        var order = await query.FirstOrDefaultAsync();
        
        if(order == null)
        {
            throw new Exception("Order not found");
        }
        
        if(order.Status != "Pending") // Đơn hàng đã xử lí rồi mà
        {
            throw new Exception("Order already processed");
        }
        
        if(order.TotalAmount != request.TransferAmount)
        {
            throw new Exception("Invalid transfer amount");
        }
        
        order.Status = "Completed";
        _dbContext.Update(order);
        await _dbContext.SaveChangesAsync();
        
        var productIds = order.OrderDetails.Select(x => x.ProductId).ToList();
        
        // Tìm những sản phẩm chưa trong Cart với các id sau productIds của UserId
        var queryProdCart = _dbContext.CartDetails.Where(x =>
            x.Cart.UserId == order.UserId &&
            productIds.Contains(x.ProductId)
        );
        
        var removeCartDetails = await queryProdCart.ToListAsync();
        
        _dbContext.RemoveRange(removeCartDetails);
        
        await _dbContext.SaveChangesAsync();
    }
}

// Bản chất của SePay
// Sẽ 1 thằng ngồi lắng nghe hết tất cả các giao dịch của mình trong tài khoản
// Và mình có thể làm 1 thứ, nếu có giao dịch nào chuyển đến thì gọi 1 api callback
// Và không phải giao dịch nào cũng là giao dịch của hệ thống mình.
    // Giao dịch trả tiền nợ từ bạn A -> Call Api
    // Giao dịch mua hàng từ hệ thống TetPee -> Call Api
    // Giao dịch trả tiền cổ tức -> Call Api
    
    // Call Api của ai thì tuỳ mn setup với hệ thống của mình, nhưng ở đây
        // a muốn là nó call api của mình để thông báo là đã ck thành công
        
    // Không phải tất cả giao dịch nào cũng là giao dịch của hệ thống mình, vậy thì để
        // phân biệt giao dịch của mình, thì chúng ta sẽ tạo ra 1 dấu ấn riêng, làm dấu