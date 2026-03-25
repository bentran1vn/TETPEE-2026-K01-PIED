using Microsoft.AspNetCore.Mvc;
using TetPee.Repository;
using TetPee.Repository.Entity;
using TetPee.Service.User;

using MediaService = TetPee.Service.MediaService;

namespace TetPee.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    // cai nay nang cao luc sau se giai thich

    private readonly IService _userService;
    private readonly MediaService.IService _mediaService;
    
    public UserController(AppDbContext dbContext, IService userService, MediaService.IService mediaService)
    {
        _dbContext = dbContext;
        _userService = userService;
        _mediaService = mediaService;
    }
    
    // HTTP METHOD: GET, POST, DELETE, PUT, PATCH
    // PARAM: Query string, Path Param, Body Param
    
    // Query string: http://localhost:5000/User?name=abc&age=20
        // name va age la query string
        // Query string nằm sau dấu ?
    
    // Path (Route) Param: http://localhost:5000/User/123
        // 123 la path param hoac route param
        // Path param nằm trong đường dẫn
        
    // GET là không có body
    // POST, PUT, PATCH có body
    
    // Tại sao phải dùng body: tránh để lộ những thông tin không mong muốn
    // Ví dụ: Username, Pass
    // Không thể http://localhost:5000/login?username=abc&password=123
    
    // Chuan REST FULL API
    // get all users: GET http://localhost:5000/User
    // create user: POST http://localhost:5000/User
    // get user by id: GET http://localhost:5000/User/{id}
    // update user by id: PUT http://localhost:5000/User/{id}
    // delete user by id: DELETE http://localhost:5000/User/{id}
    
    [HttpGet("")]
    public async Task<IActionResult> GetUsers(string? searchTerm, int pageSize = 10, int pageIndex = 1)
    {
        var users = await _userService.GetUsers(searchTerm, pageSize, pageIndex);
        return Ok(users);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        var user = await _userService.GetUserById(id);
        return Ok(user);
    }
    
    [HttpPut("{id}")]
    public IActionResult UpdateUserById(Guid id, [FromBody] Request.UpdateUserRequest request)
    {
        // var users = _dbContext.Users.ToList();
        // return Ok(users);
        return Ok("Get all users");
    }
    
    [HttpDelete("{id}")]
    public IActionResult DeleteUserById(Guid id)
    {
        // var users = _dbContext.Users.ToList();
        // return Ok(users);
        return Ok("Get all users");
    }
    
    [HttpPost("")]
    public async Task<IActionResult> CreateUsers([FromForm] Request.CreateUserRequest request, CancellationToken cancellationToken)
    {
        var user = new User()
        {
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            HashedPassword = request.Password // Chưa hash, chỉ demo
        };
        
        if(request.Avatar != null)
        {
            var media = await _mediaService.UploadImageAsync(request.Avatar);
            user.ImageUrl = media;
        }
        
        _dbContext.Users.Add(user);
        
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return Ok("Create user successfully");
    }
}

// Get all Category (Không phân trang, short theo bảng chữ cái của Name),
                // và Map ra response như sau (Id, Name)
// Get all Children Category By Category Id, (Không phân trang, short theo bảng chữ cái của Name),
                // và Map ra response như sau (Id, Name)
// Get all Seller tồn tại trong Hệ thống (Phân trang, sort theo bảng chữ cái, cho phép tìm kiếm theo Tên)
                // (Email, FirstName, LastName, ImageUrl, TaxCode, CompanyName)
// Get detail Seller By Id 
                // và Map ra response như sau (Email, FirstName, LastName, ImageUrl, PhoneNumber,
                                 // Address, DateOfBirth, TaxCode, CompanyName, CompanyAddress)