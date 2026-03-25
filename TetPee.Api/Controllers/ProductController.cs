using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TetPee.Api.Extensions;
using TetPee.Service.Product;

namespace TetPee.Api.Controllers;


[ApiController]
[Route("[controller]")]
public class ProductController: ControllerBase
{
    private readonly IService _productService;

    public ProductController(IService productService)
    {
        _productService = productService;
    }
    
    [Authorize(Policy = JwtExtensions.SellerPolicy)]
    [HttpPost("")]
    public async Task<IActionResult> CreateSeller(Request.CreateProductRequest request)
    {
        var result = await _productService.CreateProduct(request);
        return Ok(result);
    }
}