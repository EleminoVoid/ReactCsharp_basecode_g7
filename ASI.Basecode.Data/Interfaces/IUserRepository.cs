using ASI.Basecode.Data.Models;
using System.Linq;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Interfaces
{
    public interface IUserRepository
    {
        IQueryable<User> GetUsers();
        void AddUser(User user);
        void UpdateUser(User user);
        void DeleteUser(User user);
        Task SaveChangesAsync();
    }
}
