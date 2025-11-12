using ASI.Basecode.Data.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Interfaces
{
    public interface IRoomAmenityRepository
    {
        IQueryable<RoomAmenity> GetRoomAmenities();
        IQueryable<RoomAmenity> GetAmenitiesByRoomId(string roomId);
        RoomAmenity GetRoomAmenity(string roomId, string amenity);
        void AddRoomAmenity(RoomAmenity amenity);
        void DeleteRoomAmenity(RoomAmenity amenity);
        void DeleteRoomAmenities(string roomId);
        Task SaveChangesAsync();
        IEnumerable<string> GetAllUniqueAmenities();
        bool RoomAmenityExists(string roomId, string amenity);
    }
}
