using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.WebApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ASI.Basecode.WebApp.Controllers
{
    [Route("api/[controller]/[action]")]
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

        [HttpGet]
        [AllowAnonymous]
        [Route("amenities")]
        public IActionResult GetAll()
        {
            var amenities = _roomAmenityService.GetAllRoomAmenities();
            return Ok(amenities);
        }
    }
}
