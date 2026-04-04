using Microsoft.EntityFrameworkCore;
using TetPee.Repository;
using TetPee.Service.MailService;

namespace TetPee.Service.Seller;

public class Service : IService
{
    private readonly AppDbContext _dbContext;
    private readonly MailService.IService _mailService;

    public Service(AppDbContext dbContext, MailService.IService mailService)
    {
        _dbContext = dbContext;
        _mailService = mailService;
    }

    public async Task<Base.Response.PageResult<Response.GetSellersResponse>> GetSellers(string? searchTerm, int pageSize, int pageIndex)
    {
        var query = _dbContext.Sellers.Where(x => true);

        if (searchTerm != null)
        {
            query = query.Where(x =>
                x.User.FirstName.Contains(searchTerm) ||
                x.User.LastName.Contains(searchTerm) ||
                x.User.Email.Contains(searchTerm));
        }

        query = query.OrderBy(x => x.User.Email);

        query = query
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize);

        var selectedQuery = query
            .Select(x => new Response.GetSellersResponse()
            {
                Id = x.User.Id,
                Email = x.User.Email,
                FirstName = x.User.FirstName,
                LastName = x.User.LastName,
                ImageUrl = x.User.ImageUrl,
                Role = x.User.Role,
                CompanyName = x.CompanyName,
                TaxCode = x.TaxCode
            });

        // var query = _dbContext.Users.Where(x => x.Role == "Seller");
        // // Tốc độ bị chậm đị vì có nhiều role
        //
        // if(searchTerm != null)
        // {
        //     query = query.Where(x => 
        //         x.FirstName.Contains(searchTerm) ||
        //         x.LastName.Contains(searchTerm) ||
        //         x.Email.Contains(searchTerm));
        // }
        //
        // query = query.OrderBy(x => x.Email);
        //
        // query = query
        //     .Skip((pageIndex - 1) * pageSize)
        //     .Take(pageSize);
        //
        // var selectedQuery = query
        //     .Select(x => new Response.GetSellersResponse()
        //     {
        //         Id = x.Id,
        //         Email = x.Email,
        //         FirstName = x.FirstName,
        //         LastName = x.LastName,
        //         ImageUrl = x.ImageUrl,
        //         Role = x.Role,
        //         CompanyName = x.Seller!.CompanyName,
        //         TaxCode = x.Seller.TaxCode
        //     });

        var listResult = await selectedQuery.ToListAsync();
        var totalItems = listResult.Count();

        var result = new Base.Response.PageResult<Response.GetSellersResponse>()
        {
            Items = listResult,
            PageIndex = pageIndex,
            PageSize = pageSize,
            TotalItems = totalItems
        };

        return result;
    }

    public async Task<Response.GetSellerByIdResponse?> GetSellerById(Guid id)
    {
        var query = _dbContext.Sellers.Where(x => x.User.Id == id);

        var selectedQuery = query
            .Select(x => new Response.GetSellerByIdResponse()
            {
                Id = x.Id,
                Email = x.User.Email,
                FirstName = x.User.FirstName,
                LastName = x.User.LastName,
                ImageUrl = x.User.ImageUrl,
                Role = x.User.Role,
                CompanyName = x.CompanyName,
                TaxCode = x.TaxCode,
                PhoneNumber = x.User.PhoneNumber,
                Address = x.User.Address,
                CompanyAddress = x.CompanyAddress,
                DateOfBirth = x.User.DateOfBirth
            });

        var result = await selectedQuery.FirstOrDefaultAsync();

        return result;
    }

    public async Task<string> CreateSeller(Request.CreateSellerRequest request)
    {
        var existingUserQuery = _dbContext.Users.Where(x => x.Email == request.Email);

        bool isExistUser = await existingUserQuery.AnyAsync();

        if (isExistUser)
        {
            throw new ArgumentException(Message.UserExistWithMail);
        }

        var user = new Repository.Entity.User()
        {
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            HashedPassword = request.Password,
            Role = "Seller"
        };

        _dbContext.Add(user);

        var result = await _dbContext.SaveChangesAsync();

        if (result > 0)
        {
            var seller = new Repository.Entity.Seller()
            {
                CompanyAddress = request.CompanyAddress,
                CompanyName = request.CompanyName,
                TaxCode = request.TaxCode,
                UserId = user.Id,
            };

            _dbContext.Add(seller);

            var sellerResult = await _dbContext.SaveChangesAsync();

            await _mailService.SendMail(new MailContent()
{
    To = request.Email,
    Subject = "Welcome to TetPee",
    Body = $@"
<!DOCTYPE html>
<html>
<head>
  <meta charset='UTF-8'>
  <meta name='viewport' content='width=device-width, initial-scale=1.0'>
</head>
<body style='margin:0; padding:0; font-family: Arial, sans-serif; background-color:#f4f6f8;'>
  
  <table width='100%' cellpadding='0' cellspacing='0' style='background-color:#f4f6f8; padding: 20px 0;'>
    <tr>
      <td align='center'>
        
        <table width='600' cellpadding='0' cellspacing='0' style='background:#ffffff; border-radius:10px; overflow:hidden; box-shadow:0 4px 10px rgba(0,0,0,0.05);'>
          
          <!-- Header -->
          <tr>
            <td style='background: linear-gradient(90deg, #4CAF50, #2E7D32); padding: 20px; text-align:center; color:white;'>
              <h1 style='margin:0;'>Welcome to TetPee 🎉</h1>
            </td>
          </tr>

          <!-- Content -->
          <tr>
            <td style='padding: 30px; color:#333; line-height:1.6;'>
              
              <h2 style='margin-top:0;'>Hi {request.FirstName} {request.LastName}, 👋</h2>
              
              <p>
                Thank you for registering as a <strong>seller</strong> on <strong>TetPee</strong>.
                We're excited to have you join our platform!
              </p>

              <p>
                You can now start setting up your store, listing your products, and reaching thousands of customers.
              </p>

              <!-- Button -->
              <div style='text-align:center; margin: 30px 0;'>
                <a href='#' 
                   style='background:#4CAF50; color:white; padding:12px 24px; text-decoration:none; border-radius:6px; font-weight:bold; display:inline-block;'>
                  Get Started
                </a>
              </div>

              <p>
                If you have any questions or need support, feel free to reach out to us anytime.
              </p>

              <p>
                Best regards,<br/>
                <strong>The TetPee Team</strong>
              </p>
            </td>
          </tr>

          <!-- Footer -->
          <tr>
            <td style='background:#f1f1f1; padding:15px; text-align:center; font-size:12px; color:#777;'>
              © {DateTime.Now.Year} TetPee. All rights reserved.
            </td>
          </tr>

        </table>

      </td>
    </tr>
  </table>

</body>
</html>"
});

            if (sellerResult > 0) return "Add Seller successfully";

            return Message.FailToAddSeller;
        }

        return Message.FailToAddSeller;
    }
}