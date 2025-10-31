using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using System.Linq;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Repositories
{
    public class RoomRepository : BaseRepository<Room>, IRoomRepository
    {
        public RoomRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public IQueryable<Room> GetRooms()
        {
            return GetAll();
        }

        public Room GetRoomById(string id)
        {
            return GetById(id);
        }

        public void AddRoom(Room room)
        {
            Add(room);
        }

        public void UpdateRoom(Room room)
        {
            Update(room);
        }

        public void DeleteRoom(Room room)
        {
            Delete(room);
        }

        public async Task SaveChangesAsync()
        {
            await UnitOfWork.SaveChangesAsync();
        }
    }
}
