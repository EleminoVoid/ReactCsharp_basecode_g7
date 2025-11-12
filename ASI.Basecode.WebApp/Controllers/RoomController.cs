using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.WebApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASI.Basecode.WebApp.Controllers
{
    [ApiController]
    [Route("api/rooms")]
    public class RoomController : ControllerBase<RoomController>
    {
        private readonly IRoomService _roomService;
        private readonly IRoomAmenityService _amenityService;

        public RoomController(
            IHttpContextAccessor httpContextAccessor,
            ILoggerFactory loggerFactory,
            IConfiguration configuration,
            IMapper mapper,
            IRoomService roomService,
            IRoomAmenityService amenityService
        ) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            _roomService = roomService;
            _amenityService = amenityService;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetAll()
        {
            var rooms = _roomService.GetAllRooms();
            return Ok(rooms);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(string id)
        {
            var room = await _roomService.GetRoomById(id);
            if (room == null)
                return NotFound();
            return Ok(room);
        }

        /// <summary>
        /// Get room with amenities included
        /// </summary>
        [HttpGet("{id}/with-amenities")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByIdWithAmenities(string id)
        {
            try
            {
                var room = await _roomService.GetRoomById(id);
                if (room == null)
                    return NotFound();

                var amenities = await _amenityService.GetAmenitiesList(id);
                
                return Ok(new
                {
                    room.Id,
                    room.Name,
                    room.Floor,
                    room.Capacity,
                    room.Available,
                    room.Description,
                    room.Image,
                    room.CreatedAt,
                    room.UpdatedAt,
                    Amenities = amenities
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting room with amenities {id}");
                return StatusCode(500, new { message = "Failed to retrieve room", error = ex.Message });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Create([FromBody] CreateRoomRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var room = new Room
                {
                    Name = request.Name,
                    Floor = request.Floor,
                    Capacity = request.Capacity,
                    Available = request.Available ?? true,
                    Description = request.Description,
                    Image = request.Image,
                    CreatedBy = request.CreatedBy
                };

                await _roomService.AddRoom(room);

                // Add amenities if provided
                if (request.Amenities != null && request.Amenities.Count > 0)
                {
                    await _amenityService.AddAmenitiesToRoom(room.Id, request.Amenities, request.CreatedBy);
                }

                // Return room with amenities
                var amenities = await _amenityService.GetAmenitiesList(room.Id);
                
                return Ok(new
                {
                    room.Id,
                    room.Name,
                    room.Floor,
                    room.Capacity,
                    room.Available,
                    room.Description,
                    room.Image,
                    room.CreatedAt,
                    Amenities = amenities
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating room");
                return StatusCode(500, new { message = "Failed to create room", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateRoomRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var room = await _roomService.GetRoomById(id);
                if (room == null)
                    return NotFound(new { message = $"Room with ID {id} not found" });

                // Update room properties
                room.Id = id;
                room.Name = request.Name;
                room.Floor = request.Floor;
                room.Capacity = request.Capacity;
                room.Available = request.Available;
                room.Description = request.Description;
                room.Image = request.Image;
                room.UpdatedBy = request.UpdatedBy;
                
                await _roomService.UpdateRoom(room);

                // Update amenities if provided
                if (request.Amenities != null)
                {
                    await _amenityService.UpdateRoomAmenities(id, request.Amenities, request.UpdatedBy);
                }

                // Return updated room with amenities
                var amenities = await _amenityService.GetAmenitiesList(id);
                
                return Ok(new
                {
                    room.Id,
                    room.Name,
                    room.Floor,
                    room.Capacity,
                    room.Available,
                    room.Description,
                    room.Image,
                    room.UpdatedAt,
                    Amenities = amenities
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating room {id}");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                // Delete amenities first
                await _amenityService.ClearRoomAmenities(id);
                
                // Delete room
                await _roomService.DeleteRoom(id);
                
                return Ok(new { message = "Room deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting room {id}");
                return StatusCode(500, new { message = "Failed to delete room", error = ex.Message });
            }
        }
    }

    // Request models
    public class CreateRoomRequest
    {
        public string Name { get; set; }
        public string Floor { get; set; }
        public int? Capacity { get; set; }
        public bool? Available { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public List<string> Amenities { get; set; }
        public string CreatedBy { get; set; }
    }

    public class UpdateRoomRequest
    {
        public string Name { get; set; }
        public string Floor { get; set; }
        public int? Capacity { get; set; }
        public bool? Available { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public List<string> Amenities { get; set; }
        public string UpdatedBy { get; set; }
    }
}