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
    // Настройте контекст для использования sqlite.
    // для сохранения входа достаточно, для более серьёзной работы с БД нужно переехать на SQL Server или Postgre
    options.UseNpgsql("Host=localhost;Database=OpenIddictClient;Username=postgres;Password=Password;Port=5432");
    // Зарегистрируйте наборы сущностей, необходимые OpenIddict.
    // Примечание: используйте универсальную перегрузку, если вам нужно
    // для замены сущностей OpenIddict по умолчанию.
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

// OpenIddict предлагает встроенную интеграцию с Quartz.NET для выполнения запланированных задач
// (например, удаление потерянных разрешений из базы данных) через регулярные промежутки времени.
builder.Services.AddQuartz(options =>
{
    options.UseSimpleTypeLoader();
    options.UseInMemoryStore();
});

// Зарегистрируйте службу Quartz.NET и настройте ее так, чтобы она блокировала завершение работы до завершения заданий.
builder.Services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

builder.Services.AddOpenIddict()
    // Зарегистрируйте основные компоненты OpenIddict.
    .AddCore(options =>
    {
        // Настройте OpenIddict для использования хранилищ и моделей Entity Framework Core.
        // Примечание: вызовите функцию Replace Default Entities() для замены объектов OpenIddict по умолчанию.
        options.UseEntityFrameworkCore()
               .UseDbContext<ApplicationDbContext>();
        // Разработчики, предпочитающие использовать MongoDB, могут удалить предыдущие строки
        // и настроить OpenIddict для использования указанной базы данных MongoDB:
        // options.UseMongoDb()
        //        .UseDatabase(new MongoClient().GetDatabase("openiddict"));

        // Включить Quartz.NET интеграцию.
        options.UseQuartz();
    })

    // Зарегистрируйте клиентские компоненты OpenIddict.
    .AddClient(options =>
    {
        // Примечание: в этом примере используется поток кода, но при необходимости вы можете включить другие потоки.
        options.AllowAuthorizationCodeFlow()
               .AllowRefreshTokenFlow();
        // Зарегистрируйте учетные данные для подписи и шифрования, используемые для защиты
        // конфиденциальных данных, таких как токены состояния, созданные OpenIddict.
        options.AddDevelopmentEncryptionCertificate()
               .AddDevelopmentSigningCertificate();
        // Зарегистрируйте ASP.NET Основной хост и настройте ASP.NET Параметры, относящиеся к ядру.
        options.UseAspNetCore()
               .EnableStatusCodePagesIntegration()
               .EnableRedirectionEndpointPassthrough()
               .EnablePostLogoutRedirectionEndpointPassthrough();
        // Зарегистрируйте интеграцию с System.Net.Http и используйте идентификатор текущего
        // assembly в качестве более конкретного пользовательского агента, что может быть полезно при работе с
        // провайдерами, которые используют пользовательский агент для ограничения запросов (например, Reddit).
        options.UseSystemNetHttp()
               .SetProductInformation(typeof(Program).Assembly);
        // Добавьте регистрацию клиента, соответствующую определению клиентского приложения в серверном проекте.
        options.AddRegistration(new OpenIddictClientRegistration
        {
            Issuer = new Uri("https://localhost:7132/", UriKind.Absolute),

            ClientId = "mvc",
            ClientSecret = "901564A5-E7FE-42CB-B10D-61EF6A8F3654",
            Scopes = //запрашиваемая инфа (Scopes.Email, Scopes.Profile, Scopes.Roles - это по сути полные)
            { 
                Scopes.Email, //информация о почте
                Scopes.Profile, //информация о профиле
                Scopes.Roles, //доступ к списку ролей пользователя
                Scopes.OfflineAccess, //доступ к обновлению токена
                //"api1" //запрашиваем доступ к апи 1 (отдельное приложение и их может быть много)
            }, 
            // Примечание: чтобы избежать путаницы, рекомендуется использовать уникальную конечную точку перенаправления
            // URI для каждого провайдера, если только все зарегистрированные провайдеры не поддерживают возврат специального параметра "iss"
            //, содержащего их URL, как часть ответов на авторизацию. Для получения дополнительной информации
            // смотрите https://datatracker.ietf.org/doc/html/draft-ietf-oauth-security-topics#section-4.4.
            RedirectUri = new Uri("callback/login/local", UriKind.Relative),
            PostLogoutRedirectUri = new Uri("callback/logout/local", UriKind.Relative)
        });
    });

builder.Services.AddHttpClient();

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
