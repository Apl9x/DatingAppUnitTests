using DatingApp.Api.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DatingApp.Api.Interfaces
{
    //Repository Design Pattern
    public interface IUserRepository
    {
        void Update(AppUser user);

        Task<bool> SaveAllAsync();

        Task<IEnumerable<AppUser>> GetUsersAsync();

        Task<AppUser> GetUserByIdAsync(int id);

        Task<AppUser> GetUserByUsernameAsync(string username);
    }
}
