namespace TetPee.Service.Cart;

public class Response
{
    public class ProductResponse()
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}