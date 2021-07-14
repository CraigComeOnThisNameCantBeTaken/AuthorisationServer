using AuthServer.AuthModels;
using AuthServer.Database;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AuthServer.NetCoreIdentityServices
{
    public class ApplicationRoleStore : IRoleStore<ApplicationRole>
    {
        public Task<IdentityResult> CreateAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            var latestId = InMemoryPersistance.Roles.Last().Key;
            var newId = ++latestId;
            role.Id = newId;

            if (InMemoryPersistance.Roles.TryAdd(latestId, role))
            {
                return Task.FromResult(IdentityResult.Success);
            }

            return Task.FromResult(IdentityResult.Failed(
                new IdentityError
                {
                    Code = "red",
                    Description = "Couldnt add the role"
                }
            ));
        }

        public Task<IdentityResult> DeleteAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            var id = role.Id;

            if (InMemoryPersistance.Roles.TryRemove(id, out _))
            {
                return Task.FromResult(IdentityResult.Success);
            }

            return Task.FromResult(IdentityResult.Failed(
                new IdentityError
                {
                    Code = "red",
                    Description = "Couldnt delete the role"
                }
            ));
        }

        public void Dispose()
        {
        }

        public Task<ApplicationRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            var id = int.Parse(roleId);

            if (InMemoryPersistance.Roles.TryGetValue(id, out var role))
            {
                return Task.FromResult(role);
            }

            return Task.FromResult<ApplicationRole>(null);
        }

        public Task<ApplicationRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            var kvp = InMemoryPersistance.Roles
                .FirstOrDefault(u => u.Value.NormalizedName == normalizedRoleName);
            return Task.FromResult(kvp.Value);
        }

        public Task<string> GetNormalizedRoleNameAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.NormalizedName);
        }

        public Task<string> GetRoleIdAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.Id.ToString());
        }

        public Task<string> GetRoleNameAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.Name);
        }

        public Task SetNormalizedRoleNameAsync(ApplicationRole role, string normalizedName, CancellationToken cancellationToken)
        {
            role.NormalizedName = normalizedName;
            InMemoryPersistance.Roles[role.Id].NormalizedName = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetRoleNameAsync(ApplicationRole role, string roleName, CancellationToken cancellationToken)
        {
            role.Name = roleName;
            InMemoryPersistance.Roles[role.Id].Name = roleName;
            return Task.CompletedTask;
        }

        public async Task<IdentityResult> UpdateAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            var currentRoleData = await FindByIdAsync(role.Id.ToString(), cancellationToken);
            InMemoryPersistance.Roles.TryUpdate(role.Id, role, currentRoleData);

            return IdentityResult.Success;
        }
    }
}
