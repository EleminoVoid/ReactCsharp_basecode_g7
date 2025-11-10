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
    [Route("api/roomamenities")]
    public class RoomAmenityController : ControllerBase<RoomAmenityController>
    {
        private readonly IRoomAmenityService _roomAmenityService;

        public RoomAmenityController(
            IHttpContextAccessor httpContextAccessor,
            ILoggerFactory loggerFactory,
            IConfiguration configuration,
            IMapper mapper,
            IRoomAmenityService roomAmenityService
        ) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            _roomAmenityService = roomAmenityService;
        }

        /// <summary>
        /// Get all room amenities
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetAll()
        {
            try
            {
                var amenities = _roomAmenityService.GetAllRoomAmenities();
                return Ok(amenities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all room amenities");
                return StatusCode(500, new { message = "Failed to retrieve amenities", error = ex.Message });
            }
        }

        /// <summary>
        /// Get all amenities for a specific room
        /// </summary>
        [HttpGet("room/{roomId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRoomAmenities(string roomId)
        {
            try
            {
                var amenities = await _roomAmenityService.GetRoomAmenities(roomId);
                return Ok(amenities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting amenities for room {roomId}");
                return StatusCode(500, new { message = "Failed to retrieve room amenities", error = ex.Message });
            }
        }

        /// <summary>
        /// Get amenities list (strings only) for a specific room
        /// </summary>
        [HttpGet("room/{roomId}/list")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAmenitiesList(string roomId)
        {
            try
            {
                var amenities = await _roomAmenityService.GetAmenitiesList(roomId);
                return Ok(amenities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting amenities list for room {roomId}");
                return StatusCode(500, new { message = "Failed to retrieve amenities list", error = ex.Message });
            }
        }

        /// <summary>
        /// Get all unique amenities across all rooms
        /// </summary>
        [HttpGet("unique")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllUniqueAmenities()
        {
            try
            {
                var amenities = await _roomAmenityService.GetAllUniqueAmenities();
                return Ok(amenities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting unique amenities");
                return StatusCode(500, new { message = "Failed to retrieve unique amenities", error = ex.Message });
            }
        }

        /// <summary>
        /// Add a single amenity to a room
        /// </summary>
        [HttpPost("room/{roomId}/amenity")]
        [AllowAnonymous]
        public async Task<IActionResult> AddAmenityToRoom(string roomId, [FromBody] AddAmenityRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request?.Amenity))
                {
                    return BadRequest(new { message = "Amenity is required" });
                }

                await _roomAmenityService.AddAmenityToRoom(roomId, request.Amenity, request.CreatedBy);
                return Ok(new { message = "Amenity added successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding amenity to room {roomId}");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Add multiple amenities to a room
        /// </summary>
        [HttpPost("room/{roomId}/amenities")]
        [AllowAnonymous]
        public async Task<IActionResult> AddAmenitiesToRoom(string roomId, [FromBody] AddAmenitiesRequest request)
        {
            try
            {
                if (request?.Amenities == null || request.Amenities.Count == 0)
                {
                    return BadRequest(new { message = "At least one amenity is required" });
                }

                await _roomAmenityService.AddAmenitiesToRoom(roomId, request.Amenities, request.CreatedBy);
                return Ok(new { message = "Amenities added successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding amenities to room {roomId}");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Update all amenities for a room (replaces existing)
        /// </summary>
        [HttpPut("room/{roomId}")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateRoomAmenities(string roomId, [FromBody] UpdateAmenitiesRequest request)
        {
            try
            {
                await _roomAmenityService.UpdateRoomAmenities(roomId, request?.Amenities, request?.UpdatedBy);
                return Ok(new { message = "Room amenities updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating amenities for room {roomId}");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Remove a specific amenity from a room
        /// </summary>
        [HttpDelete("room/{roomId}/amenity/{amenity}")]
        [AllowAnonymous]
        public async Task<IActionResult> RemoveAmenityFromRoom(string roomId, string amenity)
        {
            try
            {
                await _roomAmenityService.RemoveAmenityFromRoom(roomId, amenity);
                return Ok(new { message = "Amenity removed successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error removing amenity from room {roomId}");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Clear all amenities from a room
        /// </summary>
        [HttpDelete("room/{roomId}")]
        [AllowAnonymous]
        public async Task<IActionResult> ClearRoomAmenities(string roomId)
        {
            try
            {
                await _roomAmenityService.ClearRoomAmenities(roomId);
                return Ok(new { message = "All room amenities cleared successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error clearing amenities for room {roomId}");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Check if a room has a specific amenity
        /// </summary>
        [HttpGet("room/{roomId}/has-amenity/{amenity}")]
        [AllowAnonymous]
        public async Task<IActionResult> RoomHasAmenity(string roomId, string amenity)
        {
            try
            {
                var hasAmenity = await _roomAmenityService.RoomHasAmenity(roomId, amenity);
                return Ok(new { hasAmenity });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking amenity for room {roomId}");
                return StatusCode(500, new { message = "Failed to check amenity", error = ex.Message });
            }
        }
    }

    // Request models
    public class AddAmenityRequest
    {
        public string Amenity { get; set; }
        public string CreatedBy { get; set; }
    }

    public class AddAmenitiesRequest
    {
        public List<string> Amenities { get; set; }
        public string CreatedBy { get; set; }
    }

    public class UpdateAmenitiesRequest
    {
        public List<string> Amenities { get; set; }
        public string UpdatedBy { get; set; }
    }
}
