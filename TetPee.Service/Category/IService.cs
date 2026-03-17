namespace TetPee.Service.Category;

public interface IService
{
    public Task<List<Response.CategoryResponse>> GetCategory();
    public Task<List<Response.CategoryResponse>> GetChildrenByCategoryId(Guid parentId);
}