using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TetPee.Repository;
using TetPee.Repository.Entity;

namespace TetPee.Service.Product;

public class Service : IService
{
    private readonly AppDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContext;

    public Service(AppDbContext dbContext, IHttpContextAccessor httpContext)
    {
        _dbContext = dbContext;
        _httpContext = httpContext;
    }

    public async Task<string> CreateProduct(Request.CreateProductRequest request)
    {
        var sellerId = _httpContext.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "SellerId")?.Value;

        var sellerIdGuid = Guid.Parse(sellerId!);

        var existingProductQuery = _dbContext.Products.Where(
            x => x.Name.ToLower().Trim() == request.Name.ToLower().Trim());

        bool isExistProduct = await existingProductQuery.AnyAsync();

        if (isExistProduct)
            throw new Exception("Product with the same name already exists");

        var existingSellerQuery = _dbContext.Sellers.Where(
            x => x.Id == sellerIdGuid);

        bool isExistSeller = await existingSellerQuery.AnyAsync();

        if (!isExistSeller)
            throw new Exception("Seller not exist");

        var product = new Repository.Entity.Product()
        {
            Description = request.Description,
            Name = request.Name,
            Price = request.Price,
            SellerId = sellerIdGuid
        };

        _dbContext.Add(product);

        var sellerResult = await _dbContext.SaveChangesAsync();

        if (request.CategoryIds != null && request.CategoryIds.Count > 0)
        {
            var productCateList = request.CategoryIds.Select(id => new ProductCategory()
            {
                CategoryId = id,
                ProductId = product.Id
            });

            // Same with above but using foreach loop
            // var productCateList1 = new List<ProductCategory>();
            // foreach (var id in request.CategoryIds)
            // {
            //     var productCate = new ProductCategory()
            //     {
            //         CategoryId = id,
            //         ProductId = product.Id
            //     };
            //     productCateList1.Add(productCate);
            // }

            _dbContext.AddRange(productCateList);
            await _dbContext.SaveChangesAsync();
        }

        if (sellerResult > 0) return "Add Product successfully";

        return "Add Product failed";
    }
}