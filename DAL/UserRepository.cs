using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Models;

namespace Repository
{
    public class UserRepository : IUserStore<User>, IUserEmailStore<User>, IUserPhoneNumberStore<User>,
    IUserTwoFactorStore<User>, IUserPasswordStore<User>, IUserRoleStore<User>
    {
        private readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                user.Id = await connection.QuerySingleAsync<int>($@"
                INSERT INTO tod.t_users
                (
                    UserName,
                    NormalizedUserName,
                    Email,
                    NormalizedEmail,
                    EmailConfirmed,
                    PasswordHash,
                    PhoneNumber,
                    PhoneNumberConfirmed,
                    TwoFactorEnabled
                )
                VALUES
                (
                    @{nameof(User.UserName)},
                    @{nameof(User.NormalizedUserName)},
                    @{nameof(User.Email)},
                    @{nameof(User.NormalizedEmail)},
                    @{nameof(User.EmailConfirmed)},
                    @{nameof(User.PasswordHash)},
                    @{nameof(User.PhoneNumber)},
                    @{nameof(User.PhoneNumberConfirmed)},
                    @{nameof(User.TwoFactorEnabled)}
                );
                SELECT id
                FROM tod.t_users
                WHERE UserName = @{nameof(User.UserName)}", user);
            }

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                await connection.ExecuteAsync($@"
                DELETE FROM tod.t_users 
                WHERE Id = @{nameof(User.Id)}", user);
            }

            return IdentityResult.Success;
        }

        public async Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                return await connection.QuerySingleOrDefaultAsync<User>($@"
                SELECT * 
                FROM tod.t_users
                WHERE Id = @{nameof(userId)}", new { userId });
            }
        }

        public async Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                var user = await connection.QuerySingleOrDefaultAsync<User>($@"
                SELECT * 
                FROM tod.t_users
                WHERE NormalizedEmail = @{nameof(normalizedUserName)}", new { normalizedUserName });
                user.Roles = (await connection.QueryAsync<Role>($@"
                SELECT TR.Id, TR.Name, TR.NormalizedName 
                FROM tod.t_roles as TR 
                INNER JOIN tod.t_userroles AS TUR ON TR.Id = TUR.RoleId 
                WHERE TUR.UserId = @{nameof(user.Id)}", new { user.Id })).ToList();
                return user;
            }
        }

        public Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedUserName);
        }

        public Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id.ToString());
        }

        public Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        public Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken)
        {
            user.NormalizedUserName = normalizedName;
            return Task.FromResult(0);
        }

        public Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;
            return Task.FromResult(0);
        }

        public async Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                await connection.ExecuteAsync($@"
                UPDATE tod.t_users 
                SET
                    UserName = @{nameof(User.UserName)},
                    NormalizedUserName = @{nameof(User.NormalizedUserName)},
                    Email = @{nameof(User.Email)},
                    NormalizedEmail = @{nameof(User.NormalizedEmail)},
                    EmailConfirmed = @{nameof(User.EmailConfirmed)},
                    PasswordHash = @{nameof(User.PasswordHash)},
                    PhoneNumber = @{nameof(User.PhoneNumber)},
                    PhoneNumberConfirmed = @{nameof(User.PhoneNumberConfirmed)},
                    TwoFactorEnabled = @{nameof(User.TwoFactorEnabled)}
                WHERE Id = @{nameof(User.Id)}", user);
            }

            return IdentityResult.Success;
        }

        public Task SetEmailAsync(User user, string email, CancellationToken cancellationToken)
        {
            user.Email = email;
            return Task.FromResult(0);
        }

        public Task<string> GetEmailAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.EmailConfirmed);
        }

        public Task SetEmailConfirmedAsync(User user, bool confirmed, CancellationToken cancellationToken)
        {
            user.EmailConfirmed = confirmed;
            return Task.FromResult(0);
        }

        public async Task<User> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                return await connection.QuerySingleOrDefaultAsync<User>($@"
                SELECT * FROM tod.t_users
                WHERE NormalizedEmail = @{nameof(normalizedEmail)}", new { normalizedEmail });
            }
        }

        public Task<string> GetNormalizedEmailAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedEmail);
        }

        public Task SetNormalizedEmailAsync(User user, string normalizedEmail, CancellationToken cancellationToken)
        {
            user.NormalizedEmail = normalizedEmail;
            return Task.FromResult(0);
        }

        public Task SetPhoneNumberAsync(User user, string phoneNumber, CancellationToken cancellationToken)
        {
            user.PhoneNumber = phoneNumber;
            return Task.FromResult(0);
        }

        public Task<string> GetPhoneNumberAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PhoneNumber);
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PhoneNumberConfirmed);
        }

        public Task SetPhoneNumberConfirmedAsync(User user, bool confirmed, CancellationToken cancellationToken)
        {
            user.PhoneNumberConfirmed = confirmed;
            return Task.FromResult(0);
        }

        public Task SetTwoFactorEnabledAsync(User user, bool enabled, CancellationToken cancellationToken)
        {
            user.TwoFactorEnabled = enabled;
            return Task.FromResult(0);
        }

        public Task<bool> GetTwoFactorEnabledAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.TwoFactorEnabled);
        }

        public Task SetPasswordHashAsync(User user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
            return Task.FromResult(0);
        }

        public Task<string> GetPasswordHashAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash != null);
        }

        public void Dispose()
        {
            // Nothing to dispose.
        }

        public Task AddToRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task RemoveFromRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IList<string>> GetRolesAsync(User user, CancellationToken cancellationToken)
        {
            if (user.Roles.Count == 0)
            {
                IList<string> list = new List<string>();
                //return list;
            }
            return Task.FromResult<IList<string>>(user.Roles.Select(x => x.Name).ToList());
        }

        public Task<bool> IsInRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Roles.Any(x => x.Name == roleName));
        }

        public Task<IList<User>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
