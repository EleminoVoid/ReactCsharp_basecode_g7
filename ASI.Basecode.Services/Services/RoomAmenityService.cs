using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Services
{
    public class RoomAmenityService : IRoomAmenityService
    {
        private readonly IRoomAmenityRepository _repository;
        private readonly IRoomRepository _roomRepository;
        private readonly IMapper _mapper;

        public RoomAmenityService(IRoomAmenityRepository repository, IRoomRepository roomRepository, IMapper mapper)
        {
            _repository = repository;
            _roomRepository = roomRepository;
            _mapper = mapper;
        }

        public IEnumerable<RoomAmenity> GetAllRoomAmenities()
        {
            return _repository.GetRoomAmenities().ToList();
        }

        public async Task<IEnumerable<RoomAmenity>> GetRoomAmenities(string roomId)
        {
            var amenities = _repository.GetAmenitiesByRoomId(roomId).ToList();
            return await Task.FromResult(amenities);
        }

        public async Task<IEnumerable<string>> GetAmenitiesList(string roomId)
        {
            var amenities = _repository.GetAmenitiesByRoomId(roomId)
                .Select(ra => ra.Amenity)
                .ToList();
            return await Task.FromResult(amenities);
        }

        public async Task AddAmenityToRoom(string roomId, string amenity, string createdBy = null)
        {
            // Validate room exists
            var room = _roomRepository.GetRoomById(roomId);
            if (room == null)
            {
                throw new Exception($"Room with ID {roomId} not found");
            }

            // Check if amenity already exists
            if (_repository.RoomAmenityExists(roomId, amenity))
            {
                throw new Exception($"Amenity '{amenity}' already exists for this room");
            }

            var roomAmenity = new RoomAmenity
            {
                RoomId = roomId,
                Amenity = amenity,
                CreatedBy = createdBy,
                CreatedAt = DateTime.Now
            };

            _repository.AddRoomAmenity(roomAmenity);
            await _repository.SaveChangesAsync();
        }

        public async Task AddAmenitiesToRoom(string roomId, List<string> amenities, string createdBy = null)
        {
            // Validate room exists
            var room = _roomRepository.GetRoomById(roomId);
            if (room == null)
            {
                throw new Exception($"Room with ID {roomId} not found");
            }

            if (amenities == null || !amenities.Any())
            {
                return;
            }

            foreach (var amenity in amenities.Distinct())
            {
                // Skip if already exists
                if (_repository.RoomAmenityExists(roomId, amenity))
                {
                    continue;
                }

                var roomAmenity = new RoomAmenity
                {
                    RoomId = roomId,
                    Amenity = amenity,
                    CreatedBy = createdBy,
                    CreatedAt = DateTime.Now
                };

                _repository.AddRoomAmenity(roomAmenity);
            }

            await _repository.SaveChangesAsync();
        }

        public async Task RemoveAmenityFromRoom(string roomId, string amenity)
        {
            var roomAmenity = _repository.GetRoomAmenity(roomId, amenity);
            if (roomAmenity == null)
            {
                throw new Exception($"Amenity '{amenity}' not found for room {roomId}");
            }

            _repository.DeleteRoomAmenity(roomAmenity);
            await _repository.SaveChangesAsync();
        }

        public async Task UpdateRoomAmenities(string roomId, List<string> amenities, string updatedBy = null)
        {
            // Validate room exists
            var room = _roomRepository.GetRoomById(roomId);
            if (room == null)
            {
                throw new Exception($"Room with ID {roomId} not found");
            }

            // Clear existing amenities
            _repository.DeleteRoomAmenities(roomId);

            // Add new amenities
            if (amenities != null && amenities.Any())
            {
                foreach (var amenity in amenities.Distinct())
                {
                    var roomAmenity = new RoomAmenity
                    {
                        RoomId = roomId,
                        Amenity = amenity,
                        CreatedBy = updatedBy,
                        CreatedAt = DateTime.Now,
                        UpdatedBy = updatedBy,
                        UpdatedAt = DateTime.Now
                    };

                    _repository.AddRoomAmenity(roomAmenity);
                }
            }

            await _repository.SaveChangesAsync();
        }

        public async Task ClearRoomAmenities(string roomId)
        {
            _repository.DeleteRoomAmenities(roomId);
            await _repository.SaveChangesAsync();
        }

        public async Task<bool> RoomHasAmenity(string roomId, string amenity)
        {
            return await Task.FromResult(_repository.RoomAmenityExists(roomId, amenity));
        }

        public async Task<IEnumerable<string>> GetAllUniqueAmenities()
        {
            var amenities = _repository.GetAllUniqueAmenities();
            return await Task.FromResult(amenities);
        }
    }
}
