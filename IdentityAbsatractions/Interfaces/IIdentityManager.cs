using IdentityAbstractions.Models;

namespace IdentityAbstractions.Interfaces
{
    public interface IIdentityManager
    {
        /// <summary>
        /// Создание пользователя
        /// </summary>
        public Task<bool> CreateUserAsync(string email, string password);
        /// <summary>
        /// Получение пользователя по email
        /// </summary>
        public Task<ApplicationUser?> GetUserByEmailAsync(string email);
        /// <summary>
        /// Обновление пользователя
        /// </summary>
        public Task<bool> UpdateUserAsync(ApplicationUser user);
        /// <summary>
        /// Удаление пользователя
        /// </summary>
        public Task<bool> DeleteUserAsync(string userId);
        /// <summary>
        /// Удаление пользователя
        /// </summary>
        public Task<bool> DeleteUserAsync(ApplicationUser user);
        /// <summary>
        /// Блокировка пользователя
        /// </summary>
        public Task<bool> LockUserAsync(string userId, DateTimeOffset? lockoutEnd);
        /// <summary>
        /// Блокировка пользователя
        /// </summary>
        public Task<bool> LockUserAsync(ApplicationUser user, DateTimeOffset? lockoutEnd);
        /// <summary>
        /// Разблокировка пользователя
        /// </summary>
        public Task<bool> UnlockUserAsync(string userId);
        /// <summary>
        /// Разблокировка пользователя
        /// </summary>
        public Task<bool> UnlockUserAsync(ApplicationUser user);
        /// <summary>
        /// Проверка, заблокирован ли пользователь
        /// </summary>
        public Task<bool> IsUserLockedOutAsync(string userId);
        /// <summary>
        /// Проверка, заблокирован ли пользователь
        /// </summary>
        public Task<bool> IsUserLockedOutAsync(ApplicationUser user);
        /// <summary>
        /// Получение всех пользователей
        /// </summary>
        public Task<IList<ApplicationUser>> GetAllUsers(CancellationToken cancellationToken = default);
        /// <summary>
        /// Получение пользователей постранично
        /// </summary>
        public Task<IList<ApplicationUser>> GetPageUsers(int pageIndex, int pageSize, CancellationToken cancellationToken = default);
        /// <summary>
        /// Получение всех заблокированных пользователей
        /// </summary>
        public Task<IList<ApplicationUser>> GetLockedOutUsersAsync(CancellationToken cancellationToken = default);
        /// <summary>
        /// Получение всех заблокированных пользователей
        /// </summary>
        public Task<IList<ApplicationUser>> GetLockedOutUsersAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default);
        /// <summary>
        /// Получение ролей пользователя
        /// </summary>
        public Task<IList<ApplicationRole>> GetUserRolesAsync(string userId);
        /// <summary>
        /// Получение ролей пользователя
        /// </summary>
        public Task<IList<ApplicationRole>> GetUserRolesAsync(ApplicationUser user);
        /// <summary>
        /// Проверка наличия роли у пользователя
        /// </summary>
        public Task<bool> IsUserInRoleAsync(string userId, string roleName);
        /// <summary>
        /// Назначение роли пользователю
        /// </summary>
        public Task<bool> AssignRolesToUserAsync(string userId, IEnumerable<string> rolesName);
        /// <summary>
        /// Назначение роли пользователю
        /// </summary>
        public Task<bool> AssignRolesToUserAsync(ApplicationUser user, IEnumerable<string> rolesName);
        /// <summary>
        /// Удаление роли у пользователя
        /// </summary>
        public Task<bool> RemoveRolesFromUserAsync(string userId, IEnumerable<string> rolesName);
        /// <summary>
        /// Удаление роли у пользователя
        /// </summary>
        public Task<bool> RemoveRolesFromUserAsync(ApplicationUser user, IEnumerable<string> rolesName);
        /// <summary>
        /// Создание роли
        /// </summary>
        public Task<bool> CreateRoleAsync(string roleName);
        /// <summary>
        /// Удаление роли
        /// </summary>
        public Task<bool> DeleteRoleAsync(string roleId);
        /// <summary>
        /// Удаление роли
        /// </summary>
        public Task<bool> DeleteRoleAsync(ApplicationRole role);
        /// <summary>
        /// Получение всех ролей
        /// </summary>
        public Task<IList<ApplicationRole>> GetAllRoles(CancellationToken cancellationToken = default);
        /// <summary>
        /// Получение ролей постранично
        /// </summary>
        public Task<IList<ApplicationRole>> GetPageRoles(int pageIndex, int pageSize, CancellationToken cancellationToken = default);
        /// <summary>
        /// Получение свободных ролей для пользователя
        /// </summary>
        public Task<IList<ApplicationRole>> GetAvailableRolesForUserAsync(string userId, CancellationToken cancellationToken = default);
        /// <summary>
        /// Получение свободных ролей для пользователя
        /// </summary>
        public Task<IList<ApplicationRole>> GetAvailableRolesForUserAsync(ApplicationUser user, CancellationToken cancellationToken = default);
        /// <summary>
        /// Обновить название роли
        /// </summary>
        public Task<bool> UpdateRoleAsync(string roleId, string roleName);
        /// <summary>
        /// Обновить название роли
        /// </summary>
        public Task<bool> UpdateRoleAsync(ApplicationRole role, string roleName);
    }
}
