using Microsoft.AspNetCore.Mvc;
using TetPee.Repository;
using TetPee.Repository.Entity;
using TetPee.Service.User;

namespace TetPee.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    // cai nay nang cao luc sau se giai thich

    public UserController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
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
    public IActionResult GetUsers([FromQuery] string? searchTerm)
    {
        var users = _dbContext.Users.ToList();
        return Ok(users);
    }
    
    [HttpGet("{id}")]
    public IActionResult GetUserById([FromRoute] Guid id)
    {
        // var users = _dbContext.Users.ToList();
        // return Ok(users);
        return Ok(id);
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
    public IActionResult CreateUsers([FromBody] Request.CreateUserRequest request)
    {
        var user = new User()
        {
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            HashedPassword = request.Password // Chưa hash, chỉ demo
        };
        
        _dbContext.Users.Add(user);
        
        _dbContext.SaveChanges();
        
        return Ok("Create user successfully");
    }
}