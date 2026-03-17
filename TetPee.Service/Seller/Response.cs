namespace TetPee.Service.Seller;

public class Response
{
    public class GetSellersResponse : User.Response.GetAllUsersResponse
    {
        public string? CompanyName { get; set; }
        public string? TaxCode { get; set; }
    }
    
    public class GetSellerByIdResponse : User.Response.GetUsersResponse
    {
        public string? CompanyName { get; set; }
        public string? CompanyAddress { get; set; }
        public string? TaxCode { get; set; }
    }
}