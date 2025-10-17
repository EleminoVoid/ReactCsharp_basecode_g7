using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;

namespace ASI.Basecode.Services.Services
{
    public class RoomAmenityService : IRoomAmenityService
    {
        private readonly IRoomAmenityRepository _repository;
        private readonly IMapper _mapper;

        public RoomAmenityService(IRoomAmenityRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public IEnumerable<RoomAmenity> GetAllRoomAmenities()
        {
            return _repository.GetRoomAmenities().ToList();
        }
    }
}
