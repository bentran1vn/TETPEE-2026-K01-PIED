using Microsoft.EntityFrameworkCore;
using TetPee.Api.Extensions;
using TetPee.Api.Middlewares;
using TetPee.Repository;

using UserService = TetPee.Service.User;
using CategoryService = TetPee.Service.Category;
using SellerService = TetPee.Service.Seller;
using IdentityService = TetPee.Service.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(  
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

builder.Services.AddJwtServices(builder.Configuration);

builder.Services.AddScoped<UserService.IService, UserService.Service>();
builder.Services.AddScoped<CategoryService.IService, CategoryService.Service>();
builder.Services.AddScoped<SellerService.IService, SellerService.Service>();
builder.Services.AddScoped<IdentityService.IService, IdentityService.Service>();

builder.Services.AddTransient<GlobalExceptionHandlerMiddleware>();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();