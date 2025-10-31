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
    public class RoomService : IRoomService
    {
        private readonly IRoomRepository _repository;
        private readonly IMapper _mapper;

        public RoomService(IRoomRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public IEnumerable<Room> GetAllRooms()
        {
            return _repository.GetRooms().ToList();
        }

        public async Task<Room> GetRoomById(string id)
        {
            return await Task.FromResult(_repository.GetRoomById(id));
        }

        public async Task AddRoom(Room room)
        {
            room.Id = Guid.NewGuid().ToString();
            room.CreatedAt = DateTime.Now;
            
            _repository.AddRoom(room);
            await _repository.SaveChangesAsync();
        }

        public async Task UpdateRoom(Room room)
        {
            var existingRoom = _repository.GetRoomById(room.Id);
            if (existingRoom == null)
            {
                throw new Exception($"Room with ID {room.Id} not found");
            }

            existingRoom.Name = room.Name;
            existingRoom.Description = room.Description;
            existingRoom.Capacity = room.Capacity;
            existingRoom.Floor = room.Floor;
            existingRoom.Available = room.Available;
            existingRoom.Image = room.Image;
            existingRoom.UpdatedAt = DateTime.Now;
            existingRoom.UpdatedBy = room.UpdatedBy;

            _repository.UpdateRoom(existingRoom);
            await _repository.SaveChangesAsync();
        }

        public async Task DeleteRoom(string id)
        {
            var room = _repository.GetRoomById(id);
            if (room != null)
            {
                _repository.DeleteRoom(room);
                await _repository.SaveChangesAsync();
            }
        }
    }
}
