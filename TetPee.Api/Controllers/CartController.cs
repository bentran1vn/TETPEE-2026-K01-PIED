using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TetPee.Service.Cart;
using TetPee.Service.Models;

namespace TetPee.Api.Controllers;


[Authorize]
[ApiController]
[Route("[controller]")]
public class CartController : ControllerBase
{
    private readonly IService _cartService;

    public CartController(IService cartService)
    {
        _cartService = cartService;
    }
    
    [HttpGet("")]
    public async Task<IActionResult> GetCart()
    {
        var result = await _cartService.GetCart();
        return Ok(ApiResponseFactory.SuccessResponse(result, "Cart Response", HttpContext.TraceIdentifier));
    }
    
    [HttpPost("")]
    public async Task<IActionResult> CreateCart()
    {
        await _cartService.CreateCart();
        return Ok(ApiResponseFactory.SuccessResponse(null, "Cart created", HttpContext.TraceIdentifier));
    }

    [HttpPost("product")]
    public async Task<IActionResult> AddProductToCart([FromBody] Request.AddProductToCartRequest request)
    {
        // var users = _dbContext.Users.ToList();
        // return Ok(users);
        await _cartService.AddProductToCart(request);
        return Ok(ApiResponseFactory.SuccessResponse("Successfully", "Product Add To Cart", HttpContext.TraceIdentifier));
    }
    
    [HttpDelete("product")]
    public async Task<IActionResult> DeleteProductFromCart([FromBody] Request.RemoveProductFromCartRequest request)
    {
        // var users = _dbContext.Users.ToList();
        // return Ok(users);
        await _cartService.RemoveProductFromCart(request);
        return Ok(ApiResponseFactory.SuccessResponse("Successfully", "Product Add To Cart", HttpContext.TraceIdentifier));
    }
}