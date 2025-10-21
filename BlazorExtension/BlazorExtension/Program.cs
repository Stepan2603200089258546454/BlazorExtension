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
using OpenIddictServer;
using Scalar.AspNetCore;
using System;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents()
    .AddAuthenticationStateSerialization();

// регистрируем возможность управлени€ контроллерами дл€ web-api
builder.Services.AddControllers(); //api
builder.Services.AddControllersWithViews(); //mvc

// регистраци€ HttpClient дл€ WASM
builder.Services.AddHttpClient();
// регистрируем токены от подделок
builder.Services.AddAntiforgery();
// –егистрирует ƒЅ и OpenIddict
builder.AddDataBase();
// –егистрируем общие сервисы из библиотеки общих сомпонентов
builder.Services.AddCommonServices();

//MS openAPI
builder.Services.AddOpenApi();
// регистрируем SignalR
builder.Services.AddSignalR();
builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes
        .Concat(["application/octet-stream"]);
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
        .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient) //по умолчанию дл€ какого €зыка генерировать код
        .WithDarkMode(true) //темна€ тема
        .WithDarkModeToggle(true) //смена темной/светлой темы
        .WithClientButton(true);
});
//SignalR
app.UseResponseCompression();
// ѕеренаправление http на https
app.UseHttpsRedirection();
// перенаправл€ет на статические страницы ошибок по коду
//app.UseStatusCodePagesWithRedirects("/StatusCode/{0}");
//app.UseStatusCodePagesWithRedirects("/Error");

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
// ѕрименить miniAPI точки дл€ Identity 
app.AddIdentityEndpoints();

app.MapControllers(); //api
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets(); //mvc

// SignalR Hub 
//app.MapHub<TestHub>("/TestHub");

app.Run();
