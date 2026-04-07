using BookShelf.Application;
using BookShelf.Infrastructure;
using BookShelf.API.Endpoints;
using BookShelf.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddApplication();
builder.Services.AddInfrastructure();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "BookShelf API", Version = "v1" });
});

var app = builder.Build();

// Configure pipeline
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapBookEndpoints();
app.MapReadingListEndpoints();

app.Run();

public partial class Program { }
