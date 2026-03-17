using Microsoft.EntityFrameworkCore;
using TetPee.Repository;

namespace TetPee.Service.Category;

public class Service : IService
{
    private readonly AppDbContext _dbContext;

    public Service(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Response.CategoryResponse>> GetCategory()
    {
        var query = _dbContext.Categories.Where(x => true);
        
        query = query.OrderBy(x => x.Name);
        
        var selectedQuery = query
            .Select(x => new Response.CategoryResponse()
        {
            Id = x.Id,
            Name = x.Name,
            ParentId = x.ParentId
        });

        var listResult = await selectedQuery.ToListAsync();
            
        return listResult;
    }

    public async Task<List<Response.CategoryResponse>> GetChildrenByCategoryId(Guid parentId)
    {
        var query = _dbContext.Categories.Where(x => x.ParentId == parentId);
        
        query = query.OrderBy(x => x.Name);
        
        var selectedQuery = query
            .Select(x => new Response.CategoryResponse()
            {
                Id = x.Id,
                Name = x.Name,
                ParentId = x.ParentId
            });

        var listResult = await selectedQuery.ToListAsync();
            
        return listResult;
    }
}