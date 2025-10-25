using OpenIddict.Abstractions;
using OpenIddict.EntityFrameworkCore.Models;
using OpenIddictAbstractions.Models;
using OpenIddictServer.Helpers;
using OpenIddictServer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace OpenIddictServer.Services
{
    internal class OpenIddictManager : IOpenIddictManager
    {
        private readonly IOpenIddictApplicationManager _applicationManager;
        private readonly IOpenIddictScopeManager _scopeManager;
        private readonly IOpenIddictTokenManager _tokenManager;
        private readonly IOpenIddictAuthorizationManager _authorizationManager;

        public OpenIddictManager(
            IOpenIddictApplicationManager applicationManager,
            IOpenIddictScopeManager scopeManager,
            IOpenIddictTokenManager tokenManager,
            IOpenIddictAuthorizationManager authorizationManager)
        {
            _applicationManager = applicationManager;
            _scopeManager = scopeManager;
            _tokenManager = tokenManager;
            _authorizationManager = authorizationManager;
        }
        /// <summary>
        /// Найти приложение по id
        /// </summary>
        public async ValueTask<OpenIddictEntityFrameworkCoreApplication?> FindByIdAsync(string? id)
        {
            if (string.IsNullOrEmpty(id)) return null;
            return await _applicationManager.FindByIdAsync(id) as OpenIddictEntityFrameworkCoreApplication;
        }
        /// <summary>
        /// Список всех приложений
        /// </summary>
        public async Task<IList<OpenIddictEntityFrameworkCoreApplication>> GetApplicationsAsync()
        {
            return (await _applicationManager.ListAsync().ToListAsync())
                .Select(x => x as OpenIddictEntityFrameworkCoreApplication)
                .Where(x => x != null)
                .ToList();
        }
        /// <summary>
        /// Список всех расширений приложения
        /// </summary>
        public async Task<IList<OpenIddictEntityFrameworkCoreScope>> GetScopesAsync()
        {
            return (await _scopeManager.ListAsync().ToListAsync())
                .Select(x => x as OpenIddictEntityFrameworkCoreScope)
                .Where(x => x != null)
                .ToList();
        }
        /// <summary>
        /// Создать модель для создания или редактирования приложения
        /// </summary>
        public async ValueTask<CreateAppClientViewModel> EditAsync(string? id)
        {
            OpenIddictEntityFrameworkCoreApplication? app = await FindByIdAsync(id);
            return Edit(app);
        }
        /// <summary>
        /// Создать модель для создания или редактирования приложения
        /// </summary>
        public CreateAppClientViewModel Edit(OpenIddictEntityFrameworkCoreApplication? app)
        {
            return new CreateAppClientViewModel()
            {
                Id = app?.Id,
                ClientId = app?.ClientId,
                ClientSecret = app?.ClientSecret,
                DisplayName = app?.DisplayName,
                RedirectUris = app?.RedirectUris,
                PostLogoutRedirectUris = app?.PostLogoutRedirectUris,
            };
        }
        
        /// <summary>
        /// Получаем создающий/обновляющий дескриптор 
        /// </summary>
        private OpenIddictApplicationDescriptor GetAppDescriptor(CreateAppClientViewModel? model, OpenIddictEntityFrameworkCoreApplication? app)
        {
            OpenIddictApplicationDescriptor napp = new OpenIddictApplicationDescriptor()
            {
                ClientId = model?.ClientId ?? app?.ClientId,
                ClientType = ClientTypes.Confidential,
                ClientSecret = model?.ClientSecret ?? app?.ClientSecret,
                ConsentType =
                    ConsentTypes.Explicit, //Явный
                    //ConsentTypes.External, //Внешний
                    //ConsentTypes.Implicit, //Скрытый
                    //ConsentTypes.Systematic, //Систематический
                DisplayName = model?.DisplayName ?? app?.DisplayName,
                RedirectUris =
                {
                    new Uri((model?.RedirectUris ?? app?.RedirectUris ?? string.Empty).Trim('"', '[', ']'))
                },
                PostLogoutRedirectUris =
                {
                    new Uri((model?.PostLogoutRedirectUris ?? app?.PostLogoutRedirectUris ?? string.Empty).Trim('"', '[', ']'))
                },
                Permissions =
                {
                    Permissions.Endpoints.Authorization,
                    Permissions.Endpoints.Token,
                    Permissions.Endpoints.EndSession,
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
            };
            return napp;
        }
        /// <summary>
        /// Создать или обновить приложение
        /// </summary>
        public async ValueTask AddOrUpdateAsync(CreateAppClientViewModel? model)
        {
            if (model == null) return;

            OpenIddictEntityFrameworkCoreApplication? app = await FindByIdAsync(model?.Id);
            OpenIddictApplicationDescriptor descriptor = GetAppDescriptor(model, app);
            
            if (app != null)
            {
                await _applicationManager.UpdateAsync(app, descriptor);
            }
            else
            {
                await _applicationManager.CreateAsync(descriptor);
            }
        }
        /// <summary>
        /// Удалить приложение
        /// </summary>
        public async ValueTask DeleteAsync(string? id)
        {
            OpenIddictEntityFrameworkCoreApplication? app = await FindByIdAsync(id);
            await DeleteAsync(app);
        }
        /// <summary>
        /// Удалить приложение
        /// </summary>
        public async ValueTask DeleteAsync(OpenIddictEntityFrameworkCoreApplication? app)
        {
            if (app != null)
            {
                await _applicationManager.DeleteAsync(app);
            }
        }


        /// <summary>
        /// Получить список авторизаций по приложению
        /// </summary>
        public async Task<IList<OpenIddictEntityFrameworkCoreAuthorization?>> GetAuthorizationFromApplication(string? id)
        {
            OpenIddictEntityFrameworkCoreApplication? app = await FindByIdAsync(id);
            if (app != null)
            {
                return (await _authorizationManager.FindByApplicationIdAsync(app.Id).ToListAsync())
                    .Select(x => x as OpenIddictEntityFrameworkCoreAuthorization)
                    .ToList();
            }
            return new List<OpenIddictEntityFrameworkCoreAuthorization?>(capacity: 0);
        }
        /// <summary>
        /// Отозвать вход
        /// </summary>
        public async Task<bool> RevokeAsync(string? id)
        {
            if (string.IsNullOrEmpty(id)) return false;

            OpenIddictEntityFrameworkCoreAuthorization? auth = await _authorizationManager.FindByIdAsync(id) as OpenIddictEntityFrameworkCoreAuthorization;
            if (auth != null)
            {
                return await _authorizationManager.TryRevokeAsync(auth);
            }
            return false;
        }
    }
}
