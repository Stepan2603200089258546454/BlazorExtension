using OpenIddict.Validation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// ������������ ���������� �������� OpenIddict.
builder.Services.AddOpenIddict()
    .AddValidation(options =>
    {
        // ����������: ���������� �������� ���������� ����������� OpenID Connect
        // ��� ��������� ������ �������� ����� ������������.
        options.SetIssuer("https://localhost:7132/");
        options.AddAudiences("resource_server_1");
        // ��������� ���������� �������� ��� ������������� ������������ � ��������������� ����������
        // ������� ������, ������������ ��� �������������� � ��������� �������� ������ ������������.
        options.UseIntrospection()
               .SetClientId("resource_server_1")
               .SetClientSecret("846B62D0-DEF9-4215-A99D-86E6B8DAB342");
        // ������������ ���������� System.Net.Http.
        options.UseSystemNetHttp();
        // ������������ ���� ASP.NET Core.
        options.UseAspNetCore();
    });

builder.Services.AddAuthentication(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
builder.Services.AddAuthorization();

builder.Services.AddControllers();
// ��������� � ��������� OpenAPI ����� ������ �� ����� https://aka.ms/aspnet/openapi
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
