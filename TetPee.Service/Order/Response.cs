namespace TetPee.Service.Order;

public class Response
{
    public class CreateOrderResponse
    {
        public Guid OrderId { get; set; }
        public decimal TotalAmount { get; set; }
        public required string BankName { get; set; }
        public required string BankAccount { get; set; }
        public required string Description { get; set; }
        public required string QRCode { get; set; }
    }
}