using RoyalCode.OperationResults;
using RoyalCode.OperationResults.TestApi.Apis;
using RoyalCode.OperationResults.TestApi.Application.SeedWork;
using RoyalCode.OperationResults.TestApi.Application.WeatherForecasts;


ApiOperationResultOptions.SetResultType(ApiResultTypes.OperationResultAsDefault);


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddSingleton<WorkContext>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseAuthorization();

app.MapApiResults();
app.MapControllers();

app.UseSwagger();
app.UseSwaggerUI();

app.Run();
