using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PescaSystem.Data;
using PescaSystem.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession(); // Habilitar sesiones

// Registrar MongoDbContext y UsuarioService en la inyección de dependencias
builder.Services.AddSingleton<MongoDbContext>();
builder.Services.AddScoped<UsuarioService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseSession(); // Agregar el uso de sesión aquí

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=PescaLog}/{action=Index}/{id?}");

app.Run();


