namespace TetPee.Service.Cart;

public class Request
{
        public class AddProductToCartRequest
        {
            public Guid ProductId { get; set; }
            public int Quantity { get; set; }
        }
        
        public class RemoveProductFromCartRequest
        {
            public Guid ProductId { get; set; }
        }
}