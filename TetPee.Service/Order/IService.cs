namespace TetPee.Service.Order;

public interface IService
{
    public Task<Response.CreateOrderResponse> CreateOrder(Request.CreateOrderRequest request);
    
    public Task SepayWebhookHandler(Request.SepayWebhookRequest request);
}