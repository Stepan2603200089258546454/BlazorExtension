using IdentityAbstractions.Interfaces;
using IdentityAbstractions.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataContext.IdentityServices
{
    public class IdentityManager : IIdentityManager
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public IdentityManager(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        /// <summary>
        /// Создание пользователя
        /// </summary>
        public async Task<bool> CreateUserAsync(string email, string password)
        {
            ApplicationUser user = new ApplicationUser
            {
                UserName = email,
                Email = email
            };
            IdentityResult result = await _userManager.CreateAsync(user, password);
            return result.Succeeded;
        }
        /// <summary>
        /// Получение пользователя по email
        /// </summary>
        public async Task<ApplicationUser?> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }
        /// <summary>
        /// Обновление пользователя
        /// </summary>
        public async Task<bool> UpdateUserAsync(ApplicationUser user)
        {
            IdentityResult result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }
        /// <summary>
        /// Удаление пользователя
        /// </summary>
        public async Task<bool> DeleteUserAsync(string userId)
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return false;
            return await DeleteUserAsync(user);
        }
        /// <summary>
        /// Удаление пользователя
        /// </summary>
        public async Task<bool> DeleteUserAsync(ApplicationUser user)
        {
            IdentityResult result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }
        /// <summary>
        /// Блокировка пользователя
        /// </summary>
        public async Task<bool> LockUserAsync(string userId, DateTimeOffset? lockoutEnd)
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return false;

            return await LockUserAsync(user, lockoutEnd);
        }
        /// <summary>
        /// Блокировка пользователя
        /// </summary>
        public async Task<bool> LockUserAsync(ApplicationUser user, DateTimeOffset? lockoutEnd)
        {
            IdentityResult result = await _userManager.SetLockoutEndDateAsync(user, lockoutEnd);
            return result.Succeeded;
        }
        /// <summary>
        /// Разблокировка пользователя
        /// </summary>
        public async Task<bool> UnlockUserAsync(string userId)
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return false;

            return await UnlockUserAsync(user);
        }
        /// <summary>
        /// Разблокировка пользователя
        /// </summary>
        public async Task<bool> UnlockUserAsync(ApplicationUser user)
        {
            IdentityResult result = await _userManager.SetLockoutEndDateAsync(user, null);
            return result.Succeeded;
        }
        /// <summary>
        /// Проверка, заблокирован ли пользователь
        /// </summary>
        public async Task<bool> IsUserLockedOutAsync(string userId)
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return false;

            return await IsUserLockedOutAsync(user);
        }
        /// <summary>
        /// Проверка, заблокирован ли пользователь
        /// </summary>
        public async Task<bool> IsUserLockedOutAsync(ApplicationUser user)
        {
            return await _userManager.IsLockedOutAsync(user);
        }
        /// <summary>
        /// Получение всех пользователей
        /// </summary>
        public async Task<IList<ApplicationUser>> GetAllUsers(CancellationToken cancellationToken = default)
        {
            return await _userManager.Users.ToListAsync(cancellationToken) ?? new List<ApplicationUser>();
        }
        /// <summary>
        /// Получение пользователей постранично
        /// </summary>
        public async Task<IList<ApplicationUser>> GetPageUsers(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            if (pageIndex < 1) pageIndex = 1;
            return await _userManager.Users
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }
        /// <summary>
        /// Получение всех заблокированных пользователей
        /// </summary>
        public async Task<IList<ApplicationUser>> GetLockedOutUsersAsync(CancellationToken cancellationToken = default)
        {
            return await _userManager.Users
                .Where(u => u.LockoutEnd != null && u.LockoutEnd > DateTimeOffset.UtcNow)
                .ToListAsync(cancellationToken);
        }
        /// <summary>
        /// Получение всех заблокированных пользователей
        /// </summary>
        public async Task<IList<ApplicationUser>> GetLockedOutUsersAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            if (pageIndex < 1) pageIndex = 1;
            return await _userManager.Users
                .Where(u => u.LockoutEnd != null && u.LockoutEnd > DateTimeOffset.UtcNow)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }
        /// <summary>
        /// Получение ролей пользователя
        /// </summary>
        public async Task<IList<ApplicationRole>> GetUserRolesAsync(string userId)
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new List<ApplicationRole>();

            return await GetUserRolesAsync(user);
        }
        /// <summary>
        /// Получение ролей пользователя
        /// </summary>
        public async Task<IList<ApplicationRole>> GetUserRolesAsync(ApplicationUser user)
        {
            var userRolesNames = await _userManager.GetRolesAsync(user);
            return await _roleManager.Roles.Where(x => userRolesNames.Contains(x.Name)).ToListAsync();
        }
        /// <summary>
        /// Проверка наличия роли у пользователя
        /// </summary>
        public async Task<bool> IsUserInRoleAsync(string userId, string roleName)
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return false;

            return await _userManager.IsInRoleAsync(user, roleName);
        }
        /// <summary>
        /// Назначение роли пользователю
        /// </summary>
        public async Task<bool> AssignRolesToUserAsync(string userId, IEnumerable<string> rolesName)
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return false;

            return await AssignRolesToUserAsync(user, rolesName);
        }
        /// <summary>
        /// Назначение роли пользователю
        /// </summary>
        public async Task<bool> AssignRolesToUserAsync(ApplicationUser user, IEnumerable<string> rolesName)
        {
            IdentityResult result = await _userManager.AddToRolesAsync(user, rolesName);
            return result.Succeeded;
        }
        /// <summary>
        /// Удаление роли у пользователя
        /// </summary>
        public async Task<bool> RemoveRolesFromUserAsync(string userId, IEnumerable<string> rolesName)
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return false;

            return await RemoveRolesFromUserAsync(user, rolesName);
        }
        /// <summary>
        /// Удаление роли у пользователя
        /// </summary>
        public async Task<bool> RemoveRolesFromUserAsync(ApplicationUser user, IEnumerable<string> rolesName)
        {
            IdentityResult result = await _userManager.RemoveFromRolesAsync(user, rolesName);
            return result.Succeeded;
        }
        /// <summary>
        /// Создание роли
        /// </summary>
        public async Task<bool> CreateRoleAsync(string roleName)
        {
            ApplicationRole role = new ApplicationRole(roleName);
            IdentityResult result = await _roleManager.CreateAsync(role);
            return result.Succeeded;
        }
        /// <summary>
        /// Удаление роли
        /// </summary>
        public async Task<bool> DeleteRoleAsync(string roleId)
        {
            ApplicationRole? role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
                return false;

            return await DeleteRoleAsync(role);
        }
        /// <summary>
        /// Удаление роли
        /// </summary>
        public async Task<bool> DeleteRoleAsync(ApplicationRole role)
        {
            IdentityResult result = await _roleManager.DeleteAsync(role);
            return result.Succeeded;
        }
        /// <summary>
        /// Получение всех ролей
        /// </summary>
        public async Task<IList<ApplicationRole>> GetAllRoles(CancellationToken cancellationToken = default)
        {
            return await _roleManager.Roles.ToListAsync(cancellationToken) ?? new List<ApplicationRole>();
        }
        /// <summary>
        /// Получение ролей постранично
        /// </summary>
        public async Task<IList<ApplicationRole>> GetPageRoles(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            if (pageIndex < 1) pageIndex = 1;
            return await _roleManager.Roles
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }
        /// <summary>
        /// Получение свободных ролей для пользователя
        /// </summary>
        public async Task<IList<ApplicationRole>> GetAvailableRolesForUserAsync(string userId, CancellationToken cancellationToken = default)
        {
            try
            {
                ApplicationUser? user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return new List<ApplicationRole>();

                return await GetAvailableRolesForUserAsync(user, cancellationToken);
            }
            catch(OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
            {
                return new List<ApplicationRole>();
            }
        }
        /// <summary>
        /// Получение свободных ролей для пользователя
        /// </summary>
        public async Task<IList<ApplicationRole>> GetAvailableRolesForUserAsync(ApplicationUser user, CancellationToken cancellationToken = default)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                // Получаем все роли
                IList<ApplicationRole> allRoles = await GetAllRoles(cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
                // Получаем роли пользователя
                IList<ApplicationRole> userRoles = await GetUserRolesAsync(user);
                cancellationToken.ThrowIfCancellationRequested();
                // Исключаем роли, которые уже есть у пользователя
                List<ApplicationRole> availableRoles = allRoles
                    .Where(role => userRoles.Any(x => x.Name == role.Name) == false)
                    .ToList();
                return availableRoles;
            }
            catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
            {
                return new List<ApplicationRole>();
            }
        }
        /// <summary>
        /// Обновить название роли
        /// </summary>
        public async Task<bool> UpdateRoleAsync(string roleId, string roleName)
        {
            ApplicationRole? findRole = await _roleManager.FindByIdAsync(roleId);
            if (findRole == null) 
                return false;

            return await UpdateRoleAsync(findRole, roleName);
        }
        /// <summary>
        /// Обновить название роли
        /// </summary>
        public async Task<bool> UpdateRoleAsync(ApplicationRole role, string roleName)
        {
            IdentityResult setNameResult = await _roleManager.SetRoleNameAsync(role, roleName);
            if (setNameResult.Succeeded == false) 
                return false;
            IdentityResult result = await _roleManager.UpdateAsync(role);
            return result.Succeeded;
        }
    }
}
