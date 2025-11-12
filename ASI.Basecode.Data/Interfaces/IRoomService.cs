using ASI.Basecode.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IRoomService
    {
        IEnumerable<Room> GetAllRooms();
        Task<Room> GetRoomById(string id);
        Task AddRoom(Room room);
        Task UpdateRoom(Room room);
        Task DeleteRoom(string id);
    }
}
