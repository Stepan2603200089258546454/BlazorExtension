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

// ������������ ����������� ���������� ������������� ��� web-api
builder.Services.AddControllers(); //api
builder.Services.AddControllersWithViews(); //mvc

// ����������� HttpClient ��� WASM
builder.Services.AddHttpClient();
// ������������ ������ �� ��������
builder.Services.AddAntiforgery();

builder.AddDataBase();

builder.Services.AddCommonServices();

//MS openAPI
builder.Services.AddOpenApi();
// ������������ SignalR
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
        .WithSidebar(true) //������� ����
        .WithTheme(ScalarTheme.Kepler) //����
        .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient) //�� ��������� ��� ������ ����� ������������ ���
        .WithDarkMode(true) //������ ����
        .WithDarkModeToggle(true) //����� ������/������� ����
        .WithClientButton(true);
});
//SignalR
app.UseResponseCompression();
// ��������������� http �� https
app.UseHttpsRedirection();
// �������������� �� ����������� �������� ������ �� ����
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
        // ����������� ���������� ������� Identity
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
