using ASI.Basecode.Data.Models;
using System.Threading.Tasks;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IUserService
    {
        Task AddUser(User user);
        LoginResult AuthenticateUser(string userid, string password, ref User user);
        object GetAllUsers();
        Task<User> GetUser(string userId);
        Task<User> GetUserByUsernameOrEmail(string usernameOrEmail);
        Task<bool> ChangePassword(string userId, string currentPassword, string newPassword);
        Task DeleteUser(string id);
    }
}
