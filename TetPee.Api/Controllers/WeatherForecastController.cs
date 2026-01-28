using Microsoft.AspNetCore.Mvc;
using TetPee.Repository;

namespace TetPee.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly AppDbContext _dbContext;

    // Use dependency injection to get the AppDbContext
    public WeatherForecastController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        var canConnect = _dbContext.Database.CanConnect();
        if (!canConnect)
        {
            throw new Exception("Unable to connect to the database.");
        }

        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = "Sample Summary" // Replace with actual logic
            })
            .ToArray();
    }
}