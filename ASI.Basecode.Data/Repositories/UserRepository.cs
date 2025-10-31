using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using System.Linq;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public IQueryable<User> GetUsers()
        {
            return GetAll();
        }

        public void AddUser(User user)
        {
            Add(user);
        }

        public void UpdateUser(User user)
        {
            Update(user);
        }

        public void DeleteUser(User user)
        {
            Delete(user);
        }

        public async Task SaveChangesAsync()
        {
            await UnitOfWork.SaveChangesAsync();
        }
    }
}
