using BoxOffice;
using BoxOffice.Business;
using BoxOffice.Business.Interfaces;
using BoxOffice.Business.Services;
using BoxOffice.Data.Data;
using BoxOffice.Data.Entities;
using BoxOffice.Data.Interfaces;
using BoxOffice.Middleware;
using BoxOffice.WebApi.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();

builder.Services.AddExceptionHandler<TheatreExceptionHandler>();

builder.Services.AddDbContext<TheatreDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<BusinessLayerProfile>();
    cfg.AddProfile<WebApiLayerProfile>();
});

builder.Services.AddTransient<DataSeeder>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAuthorService, AuthorService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IGenreService, GenreService>();
builder.Services.AddScoped<IPerformanceService, PerformanceService>();
builder.Services.AddScoped<IStatisticService, StatisticService>();
builder.Services.AddScoped<ITicketService, TicketService>();

var app = builder.Build();

app.UseHsts();
app.UseHttpsRedirection();

app.UseExceptionHandler();
if (app.Environment.IsEnvironment("Test"))
{
    using var scope = app.Services.CreateScope();
    var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
    var context = scope.ServiceProvider.GetRequiredService<TheatreDbContext>();

    context.Database.EnsureDeleted();
    context.Database.EnsureCreated();

    seeder.Seed(context);
}

app.MapControllers();

app.UseSwagger();
app.UseSwaggerUI();

app.Run();

public partial class Program { }