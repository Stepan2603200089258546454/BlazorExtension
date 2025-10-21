/*
    Velusia.Client
    Dantooine.WebAssembly.Server
 */

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using OpenIddict.Client;
using OpenIddictClientMVC.Data;
using Quartz;
using OpenIddictClient;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
{
    options.UseNpgsql("Host=localhost;Database=OpenIddictClient;Username=postgres;Password=Password;Port=5432");
    // Зарегистрируйте наборы сущностей, необходимые OpenIddict.
    options.UseOpenIddictModels();
});

builder.AddOpenIddictClient<ApplicationDbContext>();

var app = builder.Build();

// Миграции EF Core
using (IServiceScope scope = app.Services.CreateScope())
{
    IDbContextFactory<ApplicationDbContext> dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
    using (ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync())
    {
        if ((await dbContext.Database.GetPendingMigrationsAsync())?.Any() == true) //проверяем нужны ли миграции
            dbContext.Database.Migrate(); //Пытаемся актуализировать и принять миграции при старте приложения
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
