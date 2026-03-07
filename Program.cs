using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using OnlineShop.Data;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Razor Pages
builder.Services.AddRazorPages();

// API + OpenAPI
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "OnlineShop API",
        Version = "v1"
    });
});

// DbContext
builder.Services.AddDbContext<OnlineShopDataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("OnlineShop")));

var app = builder.Build();

// OpenAPI JSON + Scalar
app.UseSwagger(c =>
{
    c.RouteTemplate = "openapi/{documentName}.json";
});

app.MapScalarApiReference(options =>
{
    options.Title = "OnlineShop API";
});

// Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();

app.Run();
