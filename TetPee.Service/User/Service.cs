using Microsoft.EntityFrameworkCore;
using TetPee.Repository;

namespace TetPee.Service.User;

public class Service
{
    private readonly AppDbContext _dbContext;

    public Service(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Base.Response.PageResult<Response.GetUsersResponse>> GetUsers(
        string? searchTerm,
        int pageSize,
        int pageIndex)
    {
        var query = _dbContext.Users.Where(x => true);
        
        if(searchTerm != null)
        {
            query = query.Where(x => 
                x.FirstName.Contains(searchTerm) ||
                x.LastName.Contains(searchTerm) ||
                x.Email.Contains(searchTerm));
        }
        
        query = query
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize);
        
        var selectedQuery = query
            .Select(x => new Response.GetUsersResponse()
        {
            Id = x.Id,
            Email = x.Email,
            FirstName = x.FirstName,
            LastName = x.LastName,
            ImageUrl = x.ImageUrl,
            PhoneNumber = x.PhoneNumber,
            Address = x.Address,
            Role = x.Role,
        });

        var listResult = await selectedQuery.ToListAsync();
        var totalItems = listResult.Count();
        
        var result = new Base.Response.PageResult<Response.GetUsersResponse>()
        {
            Items = listResult,
            PageIndex = pageIndex,
            PageSize = pageSize,
            TotalItems = totalItems
        };
        
        return result;
    }
}