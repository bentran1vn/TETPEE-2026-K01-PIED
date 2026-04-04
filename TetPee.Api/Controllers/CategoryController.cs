using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TetPee.Service.Category;
using TetPee.Service.Models;

namespace TetPee.Api.Controllers;


[ApiController]
[Route("[controller]")]
public class CategoryController : ControllerBase
{
    private readonly IService _categoryService;

    public CategoryController(IService categoryService)
    {
        _categoryService = categoryService;
    }
    //
    // [Authorize]
    [HttpGet("")]
    public async Task<IActionResult> GetCategory()
    {
        var result = await _categoryService.GetCategory();
        return Ok(ApiResponseFactory.SuccessResponse(result, "Categories retrieved", HttpContext.TraceIdentifier));
    }

    [HttpGet("{parentId}/childrens")]
    public async Task<IActionResult> GetCategory(Guid parentId)
    {
        var result = await _categoryService.GetChildrenByCategoryId(parentId);
        return Ok(ApiResponseFactory.SuccessResponse(result, "Child categories retrieved", HttpContext.TraceIdentifier));
    }
}