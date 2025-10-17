using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using System.Linq;

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
    }
}
