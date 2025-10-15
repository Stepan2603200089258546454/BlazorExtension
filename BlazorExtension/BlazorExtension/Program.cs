using BlazorExtension.Client.Pages;
using BlazorExtension.Components;
using BlazorExtension.Components.Layout;
using CommonComponents;
using DataContext;
using DataContext.DataContext;
using IdentityAbstractions.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Scalar.AspNetCore;
using System;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents()
    .AddAuthenticationStateSerialization();

// регистрируем возможность управления контроллерами для web-api
builder.Services.AddControllers(); //api
builder.Services.AddControllersWithViews(); //mvc

// регистрация HttpClient для WASM
builder.Services.AddHttpClient();
// регистрируем токены от подделок
builder.Services.AddAntiforgery();

builder.AddDataBase();

builder.Services.AddCommonServices();

//MS openAPI
builder.Services.AddOpenApi();
// регистрируем SignalR
builder.Services.AddSignalR();
builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        ["application/octet-stream"]);
});

WebApplication app = builder.Build();

await app.AutoMigrate();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
//MS openAPI (/openapi/v1.json)
app.MapOpenApi();
//Scalar (/scalar/v1)
app.MapScalarApiReference(options =>
{
    options
        .WithTitle("Info API")
        .WithSidebar(true) //боковое меню
        .WithTheme(ScalarTheme.Kepler) //тема
        .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient) //по умолчанию для какого языка генерировать код
        .WithDarkMode(true) //темная тема
        .WithDarkModeToggle(true) //смена темной/светлой темы
        .WithClientButton(true);
});
//SignalR
app.UseResponseCompression();
// Перенаправление http на https
app.UseHttpsRedirection();
// перенаправляет на статические страницы ошибок по коду
//app.UseStatusCodePagesWithRedirects("/StatusCode/{0}");
app.UseStatusCodePagesWithRedirects("/Error");

app.UseRouting(); //mvc
app.UseAuthentication(); //mvc + api
app.UseAuthorization(); //mvc + api
app.UseAntiforgery();

app.MapStaticAssets();
//app.UseStaticFiles();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(
        typeof(BlazorExtension.Client._Imports).Assembly,
        // подключение библиотеки страниц Identity
        typeof(IdentityComponents._Imports).Assembly
    );

app.AddIdentityEndpoints();

app.MapControllers(); //api
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets(); //mvc

// SignalR Hub 
//app.MapHub<TestHub>("/TestHub");

app.Run();
