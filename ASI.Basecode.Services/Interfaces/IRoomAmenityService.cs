using ASI.Basecode.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IRoomAmenityService
    {
        IEnumerable<RoomAmenity> GetAllRoomAmenities();
        Task<IEnumerable<RoomAmenity>> GetRoomAmenities(string roomId);
        Task<IEnumerable<string>> GetAmenitiesList(string roomId);
        Task AddAmenityToRoom(string roomId, string amenity, string createdBy = null);
        Task AddAmenitiesToRoom(string roomId, List<string> amenities, string createdBy = null);
        Task RemoveAmenityFromRoom(string roomId, string amenity);
        Task UpdateRoomAmenities(string roomId, List<string> amenities, string updatedBy = null);
        Task ClearRoomAmenities(string roomId);
        Task<bool> RoomHasAmenity(string roomId, string amenity);
        Task<IEnumerable<string>> GetAllUniqueAmenities();
    }
}
