using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Repositories
{
    public class RoomAmenityRepository : BaseRepository<RoomAmenity>, IRoomAmenityRepository
    {
        public RoomAmenityRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public IQueryable<RoomAmenity> GetRoomAmenities()
        {
            return GetAll();
        }

        public IQueryable<RoomAmenity> GetAmenitiesByRoomId(string roomId)
        {
            return GetAll().Where(ra => ra.RoomId == roomId);
        }

        public RoomAmenity GetRoomAmenity(string roomId, string amenity)
        {
            return GetAll().FirstOrDefault(ra => ra.RoomId == roomId && ra.Amenity == amenity);
        }

        public void AddRoomAmenity(RoomAmenity amenity)
        {
            Add(amenity);
        }

        public void DeleteRoomAmenity(RoomAmenity amenity)
        {
            Delete(amenity);
        }

        public void DeleteRoomAmenities(string roomId)
        {
            var amenities = GetAmenitiesByRoomId(roomId).ToList();
            foreach (var amenity in amenities)
            {
                Delete(amenity);
            }
        }

        public async Task SaveChangesAsync()
        {
            await UnitOfWork.SaveChangesAsync();
        }

        public IEnumerable<string> GetAllUniqueAmenities()
        {
            return GetAll()
                .Select(ra => ra.Amenity)
                .Distinct()
                .OrderBy(a => a)
                .ToList();
        }

        public bool RoomAmenityExists(string roomId, string amenity)
        {
            return GetAll().Any(ra => ra.RoomId == roomId && ra.Amenity == amenity);
        }
    }
}
