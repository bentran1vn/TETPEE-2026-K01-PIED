using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TetPee.Service.Category;

namespace TetPee.Api.Controllers;


[ApiController]
[Route("[controller]")]
public class CategoryController: ControllerBase
{
    private readonly IService _categoryService;

    public CategoryController(IService categoryService)
    {
        _categoryService = categoryService;
    }
    
    [Authorize]
    [HttpGet("")]
    public async Task<IActionResult> GetCategory()
    {
        var result = await _categoryService.GetCategory();
        return Ok(result);
    }
    
    [HttpGet("{parentId}/childrens")]
    public async Task<IActionResult> GetCategory(Guid parentId)
    {
        var result = await _categoryService.GetChildrenByCategoryId(parentId);
        return Ok(result);
    }
}