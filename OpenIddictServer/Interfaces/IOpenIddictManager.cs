using OpenIddict.EntityFrameworkCore.Models;
using OpenIddictAbstractions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIddictServer.Interfaces
{
    public interface IOpenIddictManager
    {
        /// <summary>
        /// Список всех приложений
        /// </summary>
        public Task<IList<OpenIddictEntityFrameworkCoreApplication>> GetApplicationsAsync();
        /// <summary>
        /// Список всех расширений приложения
        /// </summary>
        public Task<IList<OpenIddictEntityFrameworkCoreScope>> GetScopesAsync();

        /// <summary>
        /// Создать модель для создания или редактирования приложения
        /// </summary>
        public ValueTask<CreateAppClientViewModel> EditAsync(string? id);
        /// <summary>
        /// Создать модель для создания или редактирования приложения
        /// </summary>
        public CreateAppClientViewModel Edit(OpenIddictEntityFrameworkCoreApplication? app);

        /// <summary>
        /// Создать или обновить приложение
        /// </summary>
        public ValueTask AddOrUpdateAsync(CreateAppClientViewModel? model);


        /// <summary>
        /// Удалить приложение
        /// </summary>
        public ValueTask DeleteAsync(string? id);
        /// <summary>
        /// Удалить приложение
        /// </summary>
        public ValueTask DeleteAsync(OpenIddictEntityFrameworkCoreApplication? app);

    }
}
