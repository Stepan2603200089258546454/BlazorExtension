using OpenIddict.Validation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Регистрируем компоненты проверки OpenIddict.
builder.Services.AddOpenIddict()
    .AddValidation(options =>
    {
        // Примечание: обработчик проверки использует обнаружение OpenID Connect
        // для получения адреса конечной точки интроспекции.
        options.SetIssuer("https://localhost:7132/");
        options.AddAudiences("resource_server_1");
        // Настройте обработчик проверки для использования интроспекции и зарегистрируйте клиентские
        // учетные данные, используемые при взаимодействии с удаленной конечной точкой интроспекции.
        options.UseIntrospection()
               .SetClientId("resource_server_1")
               .SetClientSecret("846B62D0-DEF9-4215-A99D-86E6B8DAB342");
        // Регистрируем интеграцию System.Net.Http.
        options.UseSystemNetHttp();
        // Регистрируем хост ASP.NET Core.
        options.UseAspNetCore();
    });

builder.Services.AddAuthentication(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
builder.Services.AddAuthorization();

builder.Services.AddControllers();
// Подробнее о настройке OpenAPI можно узнать на сайте https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
