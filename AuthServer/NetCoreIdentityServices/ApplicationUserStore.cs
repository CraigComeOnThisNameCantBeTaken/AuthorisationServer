using AuthServer.AuthModels;
using AuthServer.Database;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AuthServer.IdentityServices
{
    public class ApplicationUserStore :
        IUserStore<ApplicationUser>,
        IUserPasswordStore<ApplicationUser>
    {
        public Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            var latestId = InMemoryPersistance.Users.Any() ? InMemoryPersistance.Users.Last().Key : -1;
            var newId = ++latestId;
            user.Id = newId;

            if(InMemoryPersistance.Users.TryAdd(latestId, user)) {
                return Task.FromResult(IdentityResult.Success);
            }

            return Task.FromResult(IdentityResult.Failed(
                new IdentityError
                {
                    Code = "red",
                    Description = "Couldnt add the user"
                }
            ));
        }

        public Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            var id = user.Id;

            if (InMemoryPersistance.Users.TryRemove(id, out _))
            {
                return Task.FromResult(IdentityResult.Success);
            }

            return Task.FromResult(IdentityResult.Failed(
                new IdentityError
                {
                    Code = "red",
                    Description = "Couldnt delete the user"
                }
            ));
        }

        public void Dispose()
        {
        }

        public Task<ApplicationUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            var id = int.Parse(userId);

            if (InMemoryPersistance.Users.TryGetValue(id, out var user))
            {
                return Task.FromResult(user);
            }

            return Task.FromResult<ApplicationUser>(null);
        }

        public Task<ApplicationUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            var kvp = InMemoryPersistance.Users
                .FirstOrDefault(u => u.Value.NormalizedUserName == normalizedUserName);
            return Task.FromResult(kvp.Value);
        }

        public Task<string> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedUserName);
        }

        public Task<string> GetUserIdAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id.ToString());
        }

        public Task<string> GetUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        public Task SetNormalizedUserNameAsync(ApplicationUser user, string normalizedName, CancellationToken cancellationToken)
        {
            user.NormalizedUserName = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(ApplicationUser user, string userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;
            InMemoryPersistance.Users[user.Id].UserName = userName;
            return Task.CompletedTask;
        }

        public async Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            var currentUserData = await FindByIdAsync(user.Id.ToString(), cancellationToken);
            InMemoryPersistance.Users.TryUpdate(user.Id, user, currentUserData);

            return IdentityResult.Success;
        }

        public Task<bool> HasPasswordAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash != null);
        }

        public async Task SetPasswordHashAsync(ApplicationUser user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
            var currentUserData = await FindByIdAsync(user.Id.ToString(), cancellationToken);
            InMemoryPersistance.Users.TryUpdate(user.Id, user, currentUserData);
        }

        public Task<string> GetPasswordHashAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash);
        }
    }
}
