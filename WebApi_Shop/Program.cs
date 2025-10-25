using Microsoft.EntityFrameworkCore;
using WebApi_Shop.Models;
using Microsoft.Extensions.FileProviders;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Регистрация DbContext для DI
builder.Services.AddDbContext<SwimShopDbContext>(options =>
    options.UseMySql("server=localhost;user=root;password=root;database=swim_shop_db",
        ServerVersion.Parse("8.0.42-mysql")));

// Важно: слушаем все интерфейсы на порту 5250 (работает для профиля проекта, не IIS Express)
builder.WebHost.UseUrls("http://0.0.0.0:5250");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Раздача статических файлов из папки Images по пути /images
var imagesPath = Path.Combine(app.Environment.ContentRootPath, "Images");
if (!Directory.Exists(imagesPath)) Directory.CreateDirectory(imagesPath);

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(imagesPath),
    RequestPath = "/images"
});

app.UseAuthorization();
app.MapControllers();
app.Run();
