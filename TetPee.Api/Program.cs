using CloudinaryDotNet;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using DotNetEnv;
using TetPee.Api.Extensions;
using TetPee.Api.Middlewares;
using TetPee.Repository;
using TetPee.Service.Models;
using UserService = TetPee.Service.User;
using CategoryService = TetPee.Service.Category;
using SellerService = TetPee.Service.Seller;
using IdentityService = TetPee.Service.Identity;
using ProductService = TetPee.Service.Product;
using JwtService = TetPee.Service.JwtService;
using MediaService = TetPee.Service.MediaService;
using CloudinaryService = TetPee.Service.CloudinaryService;
using MailService = TetPee.Service.MailService;
using DiscordService = TetPee.Service.DiscordService;
using CartService = TetPee.Service.Cart;
using OrderService = TetPee.Service.Order;
using Quartz;
using Resend;
using TetPee.Service.BackgroundJobService;

Env.Load();

var aspnetCoreEnv = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", aspnetCoreEnv);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsqlOptions => npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorCodesToAdd: null))
);

builder.Services.AddJwtServices(builder.Configuration);
builder.Services.AddSwaggerServices();

builder.Services.AddScoped<UserService.IService, UserService.Service>();
builder.Services.AddScoped<CategoryService.IService, CategoryService.Service>();
builder.Services.AddScoped<SellerService.IService, SellerService.Service>();
builder.Services.AddScoped<JwtService.IService, JwtService.Service>();
builder.Services.AddScoped<IdentityService.IService, IdentityService.Service>();
builder.Services.AddScoped<ProductService.IService, ProductService.Service>();
builder.Services.AddScoped<MediaService.IService, CloudinaryService.Service>();
// builder.Services.AddScoped<MailService.IService, MailService.Service>();
builder.Services.AddScoped<CartService.IService, CartService.Service>();
builder.Services.AddScoped<OrderService.IService, OrderService.Service>();
builder.Services.AddHttpClient<DiscordService.IService, DiscordService.Service>();
builder.Services.AddHttpClient<ResendClient>();
builder.Services.Configure<ResendClientOptions>(o =>
{
    var resendOptions = builder.Configuration
        .GetSection(nameof(MailService.ResendOptions))
        .Get<MailService.ResendOptions>();
    
    o.ApiToken = resendOptions?.ApiToken
                       ?? throw new InvalidOperationException("ResendOptions:ApiToken is missing.");
});
builder.Services.AddTransient<IResend, ResendClient>();
builder.Services.AddScoped<MailService.IService, MailService.ResendService>();

builder.Services.AddQuartz(options =>
{
    var jobKey = new JobKey(nameof(ProcessTransactionPendingJob));

    options
        .AddJob<ProcessTransactionPendingJob>(jobKey)
        .AddTrigger(trigger =>
            trigger
                .ForJob(jobKey)
                .WithSimpleSchedule(schedule => schedule
                    .WithIntervalInMinutes(1)
                    .RepeatForever()
                )
        );
});
builder.Services.AddQuartzHostedService(options =>
{
    options.WaitForJobsToComplete = true;
});

builder.Services.AddTransient<GlobalExceptionHandlerMiddleware>();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
app.UseStatusCodePages(async statusCodeContext =>
{
    var response = statusCodeContext.HttpContext.Response;
    if (!string.IsNullOrWhiteSpace(response.ContentType))
    {
        return;
    }

    response.ContentType = "application/json";

    var message = response.StatusCode switch
    {
        StatusCodes.Status401Unauthorized => "Unauthorized",
        StatusCodes.Status403Forbidden => "Forbidden",
        StatusCodes.Status404NotFound => "Resource not found",
        _ => "Request could not be processed"
    };

    var payload = ApiResponseFactory.ErrorResponse(
        message: message,
        traceId: statusCodeContext.HttpContext.TraceIdentifier);

    await response.WriteAsJsonAsync(payload);
});

// if (app.Environment.IsDevelopment())
// {
//     app.UseSwaggerAPI();
// }

app.UseSwaggerAPI();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
