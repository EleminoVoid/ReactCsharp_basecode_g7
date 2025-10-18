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
    public class BookingController : ControllerBase<BookingController>
    {
        private readonly IBookingService _bookingService;

        public BookingController(
            IHttpContextAccessor httpContextAccessor,
            ILoggerFactory loggerFactory,
            IConfiguration configuration,
            IMapper mapper,
            IBookingService bookingService
        ) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            _bookingService = bookingService;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("bookings")]
        public IActionResult GetAll()
        {
            var bookings = _bookingService.GetAllBookings();
            return Ok(bookings);
        }
    }
}
