using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TetPee.Service.Identity;

namespace TetPee.Api.Controllers;


[ApiController]
[Route("[controller]")]
public class IdentityController : ControllerBase
{
    private readonly IService _identityService;

    public IdentityController(IService identityService)
    {
        _identityService = identityService;
    }
    
    [HttpGet("login")]
    public async Task<IActionResult> Login(string email, string password)
    {
        var result = await _identityService.Login(email, password);
        return Ok(result);
    }
}