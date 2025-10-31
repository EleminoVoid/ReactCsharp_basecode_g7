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
using System.Threading.Tasks;

namespace ASI.Basecode.WebApp.Controllers
{
    [ApiController]
    [Route("api/rooms")]
    public class RoomController : ControllerBase<RoomController>
    {
        private readonly IRoomService _roomService;

        public RoomController(
            IHttpContextAccessor httpContextAccessor,
            ILoggerFactory loggerFactory,
            IConfiguration configuration,
            IMapper mapper,
            IRoomService roomService
        ) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            _roomService = roomService;
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

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Create([FromBody] Room room)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _roomService.AddRoom(room);
            return Ok(room);
        }

        [HttpPut("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Update(string id, [FromBody] Room room)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                room.Id = id; // Set the ID from the route
                await _roomService.UpdateRoom(room);
                return Ok(room);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Delete(string id)
        {
            await _roomService.DeleteRoom(id);
            return Ok();
        }
    }
}