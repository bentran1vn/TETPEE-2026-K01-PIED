namespace TetPee.Service.Seller;

public class Request
{
    public class CreateSellerRequest : Identity.Request.CreateUserRequest
    {
        public required string CompanyName { get; set; }
        public required string CompanyAddress { get; set; }
        public required string TaxCode { get; set; }
    }
}