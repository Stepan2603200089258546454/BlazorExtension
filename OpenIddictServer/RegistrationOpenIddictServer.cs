using IdentityAbstractions.IdentityConstants;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenIddict.Abstractions;
using OpenIddictAbstractions.Constants;
using OpenIddictServer.Helpers;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace OpenIddictServer
{
    public static class RegistrationOpenIddictServer
    {
        /// <summary>
        /// Зарегистрируйте наборы сущностей, необходимые OpenIddict.
        /// </summary>
        public static DbContextOptionsBuilder UseOpenIddictModels(this DbContextOptionsBuilder builder)
        {
            // Зарегистрируйте наборы сущностей, необходимые OpenIddict.
            // Примечание: используйте универсальную перегрузку, если вам нужно
            // для замены сущностей OpenIddict по умолчанию.
            builder.UseOpenIddict();
            return builder;
        }
        /// <summary>
        /// Зарегистрирует Quartz и Настроит серверную часть
        /// </summary>
        /// <typeparam name="T">DbContext</typeparam>
        /// <param name="builder"></param>
        public static void AddOpenIddictServer<T>(this IHostApplicationBuilder builder) where T : DbContext
        {
            // OpenIddict предлагает встроенную интеграцию с Quartz.NET для выполнения запланированных задач
            // (например, удаление потерянных авторизаций/токенов из базы данных) через регулярные промежутки времени.
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
                    // // Примечание: вызовите функцию Replace Default Entities() для замены объектов OpenIddict по умолчанию.
                    options.UseEntityFrameworkCore()
                           .UseDbContext<T>();
                    // Включить Quartz.NET интеграцию.
                    options.UseQuartz()
                    // Все токены/ авторизации будут удалены во время работы по очистке, если они старше 14 дней.
                    // Этот срок жизни может быть изменен для токенов и авторизаций независимо:
                           .SetMinimumAuthorizationLifespan(TimeSpan.FromDays(7))
                           .SetMinimumTokenLifespan(TimeSpan.FromHours(12))
                    // Любое неудачное задание Quartz.NET будет по умолчанию повторено дважды.
                    // Это количество повторов можно настроить с помощью этого параметра:
                           .SetMaximumRefireCount(2);
                })
                // Зарегистрируйте серверные компоненты OpenIddict.
                .AddServer(options =>
                {
                    // Включите конечную точку авторизации, выхода из системы, токен и информацию о пользователе.
                    options.SetAuthorizationEndpointUris(OpenIddictConst.Route.OppenIddictServer.AuthorizationEndpoint)
                           .SetEndSessionEndpointUris(OpenIddictConst.Route.OppenIddictServer.EndSessionEndpoint)
                           .SetIntrospectionEndpointUris(OpenIddictConst.Route.OppenIddictServer.IntrospectionEndpoint)
                           .SetTokenEndpointUris(OpenIddictConst.Route.OppenIddictServer.TokenEndpoint)
                           .SetUserInfoEndpointUris(OpenIddictConst.Route.OppenIddictServer.UserInfoEndpoint)
                           .SetEndUserVerificationEndpointUris(OpenIddictConst.Route.OppenIddictServer.EndUserVerificationEndpoint);
                    // установить время жизни токенов
                    options.SetAccessTokenLifetime(TimeSpan.FromHours(1)) //время жизни основного токена (от него и пляшем в основном)
                           .SetRefreshTokenLifetime(TimeSpan.FromDays(14));
                    // Отметьте области "электронная почта", "профиль" и "роли" как поддерживаемые.
                    options.RegisterScopes(Scopes.Email, Scopes.Profile, Scopes.Roles, Scopes.OfflineAccess);
                    // Включить нужные потоки
                    options.AllowAuthorizationCodeFlow() // Включить поток кода авторизации клиента.
                           .AllowRefreshTokenFlow() // Включить поток токен обновления клиента.
                           .AllowPasswordFlow() // Включить поток работы с данными клиента
                           .AllowClientCredentialsFlow(); // Включить поток учетных данных клиента.
                    // Принимать анонимных клиентов (т. е. клиентов, которые не отправляют client_id).
                    //options.AcceptAnonymousClients();
                    // Зарегистрируйте учетные данные для подписи и шифрования.
                    options.AddDevelopmentEncryptionCertificate()
                           .AddDevelopmentSigningCertificate();
                    // Зарегистрируйте ASP.NET Основной хост и настройте ASP.NET Параметры, относящиеся к ядру.
                    options.UseAspNetCore()
                           .EnableAuthorizationEndpointPassthrough()
                           .EnableEndSessionEndpointPassthrough()
                           .EnableTokenEndpointPassthrough()
                           .EnableUserInfoEndpointPassthrough()
                           .EnableStatusCodePagesIntegration();
                })
                // Зарегистрируйте компоненты проверки OpenIddict.
                .AddValidation(options =>
                {
                    // Импортируйте конфигурацию из локального экземпляра сервера OpenIddict.
                    options.UseLocalServer();
                    // Зарегистрируйте ASP.NET Основной хост.
                    options.UseAspNetCore();
                });
        }
        /// <summary>
        /// Чистит и создает тестовые клиенты для отладки и тестов
        /// </summary>
        public static async Task InitAppClient(IServiceScope scope)
        {
            //Создаем все приложения с которыми мы согласны работать

            IOpenIddictApplicationManager manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();
            //Очищаем что было
            var list = await manager.ListAsync().ToListAsync();
            foreach (var item in list)
            {
                await manager.DeleteAsync(item);
            }
            // Проверено (работает с примером Console)
            if (await manager.FindByClientIdAsync("console") == null)
            {
                await manager.CreateAsync(new OpenIddictApplicationDescriptor
                {
                    ClientId = "console",
                    ClientSecret = "388D45FA-B36B-4988-BA59-B187D329C207",
                    DisplayName = "My client application",
                    Permissions =
                    {
                        Permissions.Endpoints.Token,
                        Permissions.GrantTypes.ClientCredentials
                    }
                });
            }
            // Проверено (работает с примером ConsoleBrowser)
            if (await manager.FindByClientIdAsync("console_app") == null)
            {
                await manager.CreateAsync(new OpenIddictApplicationDescriptor
                {
                    ApplicationType = ApplicationTypes.Native,
                    ClientId = "console_app",
                    //ClientType = ClientTypes.Public,
                    ClientType = ClientTypes.Confidential,
                    ClientSecret = "400D45FA-B36B-4988-BA59-B187D329C207",
                    ConsentType = ConsentTypes.Explicit,
                    DisplayName = "Console application",
                    RedirectUris =
                    {
                        new Uri("http://localhost/")
                    },
                    Permissions =
                    {
                        Permissions.Endpoints.Authorization,
                        Permissions.Endpoints.Token,
                        Permissions.GrantTypes.AuthorizationCode,
                        Permissions.GrantTypes.Password, //позволяет авторизовать пользователя программно
                        Permissions.GrantTypes.RefreshToken, //позволяет работать с offline обновлениями
                        Permissions.GrantTypes.ClientCredentials, //узнавать токен клиента
                        Permissions.ResponseTypes.Code,
                        Permissions.Scopes.Email,
                        Permissions.Scopes.Profile,
                        Permissions.Scopes.Roles,
                    },
                    Requirements =
                    {
                        Requirements.Features.ProofKeyForCodeExchange
                    }
                });
            }
            // Проверено (работает с примером ClientMVC)
            if (await manager.FindByClientIdAsync("mvc") == null)
            {
                await manager.CreateAsync(new OpenIddictApplicationDescriptor
                {
                    ClientId = "mvc",
                    ClientSecret = "901564A5-E7FE-42CB-B10D-61EF6A8F3654",
                    ConsentType = ConsentTypes.Explicit,
                    DisplayName = "MVC client application",
                    RedirectUris =
                    {
                        new Uri("https://localhost:7117/callback/login/local")
                    },
                    PostLogoutRedirectUris =
                    {
                        new Uri("https://localhost:7117/callback/logout/local")
                    },
                    Permissions =
                    {
                        Permissions.Endpoints.Authorization,
                        Permissions.Endpoints.EndSession,
                        Permissions.Endpoints.Token,
                        Permissions.GrantTypes.AuthorizationCode,
                        Permissions.GrantTypes.RefreshToken, //позволяет работать с offline обновлениями
                        Permissions.GrantTypes.ClientCredentials, //узнавать токен клиента
                        Permissions.ResponseTypes.Code,
                        Permissions.Scopes.Email,
                        Permissions.Scopes.Profile,
                        Permissions.Scopes.Roles,
                        Permissions.Prefixes.Scope + "api1" //Можно использовать апи 1
                    },
                    Requirements =
                    {
                        Requirements.Features.ProofKeyForCodeExchange
                    }
                });
            }
            // Проверено (работает с примером ClientBlazor)
            if (await manager.FindByClientIdAsync("blazor") == null)
            {
                await manager.CreateAsync(new OpenIddictApplicationDescriptor
                {
                    ClientId = "blazor",
                    ClientSecret = "901564A5-E7FE-42CB-B10D-61EF6A8F3654",
                    ConsentType = ConsentTypes.Explicit,
                    DisplayName = "blazor client application",
                    RedirectUris =
                    {
                        new Uri("https://localhost:7068/callback/login/local")
                    },
                    PostLogoutRedirectUris =
                    {
                        new Uri("https://localhost:7068/callback/logout/local")
                    },
                    Permissions =
                    {
                        Permissions.Endpoints.Authorization,
                        Permissions.Endpoints.EndSession,
                        Permissions.Endpoints.Token,
                        Permissions.GrantTypes.AuthorizationCode,
                        Permissions.GrantTypes.RefreshToken, //позволяет работать с offline обновлениями
                        Permissions.GrantTypes.ClientCredentials, //узнавать токен клиента
                        Permissions.ResponseTypes.Code,
                        Permissions.Scopes.Email,
                        Permissions.Scopes.Profile,
                        Permissions.Scopes.Roles
                    },
                    Requirements =
                    {
                        Requirements.Features.ProofKeyForCodeExchange
                    }
                });
            }
            // Проверено (работает с примером ResourceApi)
            if (await manager.FindByClientIdAsync("resource_server_1") is null)
            {
                await manager.CreateAsync(new OpenIddictApplicationDescriptor
                {
                    ClientId = "resource_server_1",
                    ClientSecret = "846B62D0-DEF9-4215-A99D-86E6B8DAB342",
                    Permissions =
                    {
                        Permissions.Endpoints.Introspection
                    }
                });
            }

            // Создаем расширения которые будут использовать этот сервер

            var manager2 = scope.ServiceProvider.GetRequiredService<IOpenIddictScopeManager>();

            //Очищаем что было
            var listScope = await manager2.ListAsync().ToListAsync();
            foreach (var item in listScope)
            {
                await manager2.DeleteAsync(item);
            }
            // Проверено (работает с примером ResourceApi)
            if (await manager2.FindByNameAsync("api1") is null)
            {
                await manager2.CreateAsync(new OpenIddictScopeDescriptor
                {
                    Name = "api1",
                    DisplayName = "API Service 1",
                    Resources =
                    {
                        "resource_server_1"
                    }
                });
            }
        }
    }
}
