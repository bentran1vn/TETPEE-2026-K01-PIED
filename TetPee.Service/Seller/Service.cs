using Microsoft.EntityFrameworkCore;
using TetPee.Repository;

namespace TetPee.Service.Seller;

public class Service : IService
{
    private readonly AppDbContext _dbContext;

    public Service(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Base.Response.PageResult<Response.GetSellersResponse>> GetSellers(string? searchTerm, int pageSize, int pageIndex)
    {
        var query = _dbContext.Sellers.Where(x => true);
        
        if(searchTerm != null)
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
}