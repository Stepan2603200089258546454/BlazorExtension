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
using static OpenIddict.Abstractions.OpenIddictConstants;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
{
    // ��������� �������� ��� ������������� sqlite.
    // ��� ���������� ����� ����������, ��� ����� ��������� ������ � �� ����� ��������� �� SQL Server ��� Postgre
    options.UseNpgsql("Host=localhost;Database=OpenIddictClient;Username=postgres;Password=Password;Port=5432");
    // ��������������� ������ ���������, ����������� OpenIddict.
    // ����������: ����������� ������������� ����������, ���� ��� �����
    // ��� ������ ��������� OpenIddict �� ���������.
    options.UseOpenIddict();
});
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}).AddCookie(options =>
{
    options.LoginPath = "/login";
    options.LogoutPath = "/logout";
    //options.ExpireTimeSpan = TimeSpan.FromDays(1);
    options.SlidingExpiration = false;
});

// OpenIddict ���������� ���������� ���������� � Quartz.NET ��� ���������� ��������������� �����
// (��������, �������� ���������� ���������� �� ���� ������) ����� ���������� ���������� �������.
builder.Services.AddQuartz(options =>
{
    options.UseSimpleTypeLoader();
    options.UseInMemoryStore();
});

// ��������������� ������ Quartz.NET � ��������� �� ���, ����� ��� ����������� ���������� ������ �� ���������� �������.
builder.Services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

builder.Services.AddOpenIddict()
    // ��������������� �������� ���������� OpenIddict.
    .AddCore(options =>
    {
        // ��������� OpenIddict ��� ������������� �������� � ������� Entity Framework Core.
        // ����������: �������� ������� Replace Default Entities() ��� ������ �������� OpenIddict �� ���������.
        options.UseEntityFrameworkCore()
               .UseDbContext<ApplicationDbContext>();
        // ������������, �������������� ������������ MongoDB, ����� ������� ���������� ������
        // � ��������� OpenIddict ��� ������������� ��������� ���� ������ MongoDB:
        // options.UseMongoDb()
        //        .UseDatabase(new MongoClient().GetDatabase("openiddict"));

        // �������� Quartz.NET ����������.
        options.UseQuartz();
    })

    // ��������������� ���������� ���������� OpenIddict.
    .AddClient(options =>
    {
        // ����������: � ���� ������� ������������ ����� ����, �� ��� ������������� �� ������ �������� ������ ������.
        options.AllowAuthorizationCodeFlow()
               .AllowRefreshTokenFlow();
        // ��������������� ������� ������ ��� ������� � ����������, ������������ ��� ������
        // ���������������� ������, ����� ��� ������ ���������, ��������� OpenIddict.
        options.AddDevelopmentEncryptionCertificate()
               .AddDevelopmentSigningCertificate();
        // ��������������� ASP.NET �������� ���� � ��������� ASP.NET ���������, ����������� � ����.
        options.UseAspNetCore()
               .EnableStatusCodePagesIntegration()
               .EnableRedirectionEndpointPassthrough()
               .EnablePostLogoutRedirectionEndpointPassthrough();
        // ��������������� ���������� � System.Net.Http � ����������� ������������� ��������
        // assembly � �������� ����� ����������� ����������������� ������, ��� ����� ���� ������� ��� ������ �
        // ������������, ������� ���������� ���������������� ����� ��� ����������� �������� (��������, Reddit).
        options.UseSystemNetHttp()
               .SetProductInformation(typeof(Program).Assembly);
        // �������� ����������� �������, ��������������� ����������� ����������� ���������� � ��������� �������.
        options.AddRegistration(new OpenIddictClientRegistration
        {
            Issuer = new Uri("https://localhost:7132/", UriKind.Absolute),

            ClientId = "mvc",
            ClientSecret = "901564A5-E7FE-42CB-B10D-61EF6A8F3654",
            Scopes = //������������� ���� (Scopes.Email, Scopes.Profile, Scopes.Roles - ��� �� ���� ������)
            { 
                Scopes.Email, //���������� � �����
                Scopes.Profile, //���������� � �������
                Scopes.Roles, //������ � ������ ����� ������������
                Scopes.OfflineAccess, //������ � ���������� ������
                //"api1" //����������� ������ � ��� 1 (��������� ���������� � �� ����� ���� �����)
            }, 
            // ����������: ����� �������� ��������, ������������� ������������ ���������� �������� ����� ���������������
            // URI ��� ������� ����������, ���� ������ ��� ������������������ ���������� �� ������������ ������� ������������ ��������� "iss"
            //, ����������� �� URL, ��� ����� ������� �� �����������. ��� ��������� �������������� ����������
            // �������� https://datatracker.ietf.org/doc/html/draft-ietf-oauth-security-topics#section-4.4.
            RedirectUri = new Uri("callback/login/local", UriKind.Relative),
            PostLogoutRedirectUri = new Uri("callback/logout/local", UriKind.Relative)
        });
    });

builder.Services.AddHttpClient();

var app = builder.Build();

// �������� EF Core
using (IServiceScope scope = app.Services.CreateScope())
{
    IDbContextFactory<ApplicationDbContext> dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
    using (ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync())
    {
        if ((await dbContext.Database.GetPendingMigrationsAsync())?.Any() == true) //��������� ����� �� ��������
            dbContext.Database.Migrate(); //�������� ��������������� � ������� �������� ��� ������ ����������
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
