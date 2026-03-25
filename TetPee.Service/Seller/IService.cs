namespace TetPee.Service.Seller;

public interface IService
{
    public Task<Base.Response.PageResult<Response.GetSellersResponse>> GetSellers(
        string? searchTerm,
        int pageSize,
        int pageIndex);
    
    public Task<Response.GetSellerByIdResponse?> GetSellerById(
        Guid id);

    public Task<string> CreateSeller(Request.CreateSellerRequest request);
}