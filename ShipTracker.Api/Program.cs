using Microsoft.EntityFrameworkCore;
using ShipTracker.Api.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("Postgres"))
       .UseSnakeCaseNamingConvention());
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseHttpsRedirection();
app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "ShipTracker.Api v1"));

app.UseAuthorization();

app.MapControllers();

app.Run();
