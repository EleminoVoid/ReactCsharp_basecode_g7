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
using System.Linq;
using System.Threading.Tasks;

namespace ASI.Basecode.WebApp.Controllers
{
    [ApiController]
    [Route("api/bookings")]
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

        /// <summary>
        /// Get all bookings
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetAll()
        {
            try
            {
                var bookings = _bookingService.GetAllBookings();
                var response = bookings.Select(b => new BookingResponseDto
                {
                    Id = b.Id,
                    UserId = b.UserId,
                    RoomId = b.RoomId,
                    StartDate = b.StartDate,
                    EndDate = b.EndDate,
                    Type = b.Type,
                    Status = b.Status,
                    CreatedAt = b.CreatedAt,
                    UpdatedAt = b.UpdatedAt,
                    UserName = b.User?.Username,
                    UserEmail = b.User?.Email,
                    RoomName = b.Room?.Name
                });
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all bookings");
                return StatusCode(500, new { message = "Failed to retrieve bookings", error = ex.Message });
            }
        }

        /// <summary>
        /// Get booking by ID
        /// </summary>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var booking = await _bookingService.GetBookingById(id);
                if (booking == null)
                {
                    return NotFound(new { message = $"Booking with ID {id} not found" });
                }

                var response = new BookingResponseDto
                {
                    Id = booking.Id,
                    UserId = booking.UserId,
                    RoomId = booking.RoomId,
                    StartDate = booking.StartDate,
                    EndDate = booking.EndDate,
                    Type = booking.Type,
                    Status = booking.Status,
                    CreatedAt = booking.CreatedAt,
                    UpdatedAt = booking.UpdatedAt,
                    UserName = booking.User?.Username,
                    UserEmail = booking.User?.Email,
                    RoomName = booking.Room?.Name
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting booking {id}");
                return StatusCode(500, new { message = "Failed to retrieve booking", error = ex.Message });
            }
        }

        /// <summary>
        /// Get bookings by user ID
        /// </summary>
        [HttpGet("user/{userId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByUserId(string userId)
        {
            try
            {
                var bookings = await _bookingService.GetBookingsByUserId(userId);
                var response = bookings.Select(b => new BookingResponseDto
                {
                    Id = b.Id,
                    UserId = b.UserId,
                    RoomId = b.RoomId,
                    StartDate = b.StartDate,
                    EndDate = b.EndDate,
                    Type = b.Type,
                    Status = b.Status,
                    CreatedAt = b.CreatedAt,
                    UpdatedAt = b.UpdatedAt,
                    RoomName = b.Room?.Name
                });
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting bookings for user {userId}");
                return StatusCode(500, new { message = "Failed to retrieve user bookings", error = ex.Message });
            }
        }

        /// <summary>
        /// Get bookings by room ID
        /// </summary>
        [HttpGet("room/{roomId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByRoomId(string roomId)
        {
            try
            {
                var bookings = await _bookingService.GetBookingsByRoomId(roomId);
                var response = bookings.Select(b => new BookingResponseDto
                {
                    Id = b.Id,
                    UserId = b.UserId,
                    RoomId = b.RoomId,
                    StartDate = b.StartDate,
                    EndDate = b.EndDate,
                    Type = b.Type,
                    Status = b.Status,
                    CreatedAt = b.CreatedAt,
                    UpdatedAt = b.UpdatedAt,
                    UserName = b.User?.Username,
                    UserEmail = b.User?.Email
                });
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting bookings for room {roomId}");
                return StatusCode(500, new { message = "Failed to retrieve room bookings", error = ex.Message });
            }
        }

        /// <summary>
        /// Get pending bookings
        /// </summary>
        [HttpGet("pending")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPending()
        {
            try
            {
                var bookings = await _bookingService.GetPendingBookings();
                var response = bookings.Select(b => new BookingResponseDto
                {
                    Id = b.Id,
                    UserId = b.UserId,
                    RoomId = b.RoomId,
                    StartDate = b.StartDate,
                    EndDate = b.EndDate,
                    Type = b.Type,
                    Status = b.Status,
                    CreatedAt = b.CreatedAt,
                    UserName = b.User?.Username,
                    UserEmail = b.User?.Email,
                    RoomName = b.Room?.Name
                });
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pending bookings");
                return StatusCode(500, new { message = "Failed to retrieve pending bookings", error = ex.Message });
            }
        }

        /// <summary>
        /// Get upcoming bookings
        /// </summary>
        [HttpGet("upcoming")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUpcoming()
        {
            try
            {
                var bookings = await _bookingService.GetUpcomingBookings();
                var response = bookings.Select(b => new BookingResponseDto
                {
                    Id = b.Id,
                    UserId = b.UserId,
                    RoomId = b.RoomId,
                    StartDate = b.StartDate,
                    EndDate = b.EndDate,
                    Type = b.Type,
                    Status = b.Status,
                    CreatedAt = b.CreatedAt,
                    UserName = b.User?.Username,
                    UserEmail = b.User?.Email,
                    RoomName = b.Room?.Name
                });
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting upcoming bookings");
                return StatusCode(500, new { message = "Failed to retrieve upcoming bookings", error = ex.Message });
            }
        }

        /// <summary>
        /// Get bookings in date range
        /// </summary>
        [HttpGet("date-range")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                var bookings = await _bookingService.GetBookingsByDateRange(startDate, endDate);
                var response = bookings.Select(b => new BookingResponseDto
                {
                    Id = b.Id,
                    UserId = b.UserId,
                    RoomId = b.RoomId,
                    StartDate = b.StartDate,
                    EndDate = b.EndDate,
                    Type = b.Type,
                    Status = b.Status,
                    CreatedAt = b.CreatedAt,
                    UserName = b.User?.Username,
                    RoomName = b.Room?.Name
                });
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting bookings by date range");
                return StatusCode(500, new { message = "Failed to retrieve bookings", error = ex.Message });
            }
        }

        /// <summary>
        /// Check room availability
        /// </summary>
        [HttpGet("check-availability")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckAvailability([FromQuery] string roomId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                var isAvailable = await _bookingService.CheckAvailability(roomId, startDate, endDate);
                return Ok(new
                {
                    roomId,
                    startDate,
                    endDate,
                    isAvailable
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking availability");
                return StatusCode(500, new { message = "Failed to check availability", error = ex.Message });
            }
        }

        /// <summary>
        /// Get available dates for a room
        /// </summary>
        [HttpGet("available-dates")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAvailableDates([FromQuery] string roomId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                var availableDates = await _bookingService.GetAvailableDates(roomId, startDate, endDate);
                return Ok(availableDates);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting available dates");
                return StatusCode(500, new { message = "Failed to get available dates", error = ex.Message });
            }
        }

        /// <summary>
        /// Create a new booking
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Create([FromBody] CreateBookingRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "Invalid booking data", errors = ModelState });
                }

                var booking = new Booking
                {
                    UserId = request.UserId,
                    RoomId = request.RoomId,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    Type = request.Type,
                    Status = "Pending"
                };

                var bookingId = await _bookingService.CreateBooking(booking, request.CreatedBy);
                var createdBooking = await _bookingService.GetBookingById(bookingId);

                var response = new BookingResponseDto
                {
                    Id = createdBooking.Id,
                    UserId = createdBooking.UserId,
                    RoomId = createdBooking.RoomId,
                    StartDate = createdBooking.StartDate,
                    EndDate = createdBooking.EndDate,
                    Type = createdBooking.Type,
                    Status = createdBooking.Status,
                    CreatedAt = createdBooking.CreatedAt,
                    RoomName = createdBooking.Room?.Name
                };

                return Ok(new
                {
                    message = "Booking created successfully",
                    success = true,
                    bookingId,
                    booking = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating booking");
                return StatusCode(500, new { message = ex.Message, success = false });
            }
        }

        /// <summary>
        /// Create recurring booking based on selected days
        /// </summary>
        [HttpPost("recurring")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateRecurring([FromBody] CreateRecurringBookingRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "Invalid recurring booking data", errors = ModelState });
                }

                // Validate selected days
                if (request.SelectedDays == null || request.SelectedDays.Count == 0)
                {
                    return BadRequest(new { message = "Please select at least one day of the week" });
                }

                // Validate day values (0-6)
                if (request.SelectedDays.Any(d => d < 0 || d > 6))
                {
                    return BadRequest(new { message = "Invalid day value. Use 0=Su, 1=M, 2=T, 3=W, 4=Th, 5=F, 6=S" });
                }

                // Extract time from start and end DateTime
                var startTime = request.StartDate.TimeOfDay;
                var endTime = request.EndDate.TimeOfDay;

                var recurringRequest = new RecurringBookingRequest
                {
                    UserId = request.UserId,
                    RoomId = request.RoomId,
                    StartDate = request.StartDate.Date,
                    RecurrenceEndDate = request.RecurrenceEndDate.Date,
                    StartTime = startTime,
                    EndTime = endTime,
                    Type = request.Type,
                    SelectedDays = request.SelectedDays
                };

                var bookingIds = await _bookingService.CreateRecurringBooking(recurringRequest, request.UserId);

                return Ok(new
                {
                    message = $"Created {bookingIds.Count} recurring bookings successfully",
                    success = true,
                    bookingIds,
                    count = bookingIds.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating recurring booking");

                // If some bookings were created but some failed
                if (ex.Message.Contains("Created"))
                {
                    return Ok(new { message = ex.Message, success = true, partialSuccess = true });
                }

                return StatusCode(500, new { message = ex.Message, success = false });
            }
        }

        /// <summary>
        /// Update booking
        /// </summary>
        [HttpPut("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateBookingRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "Invalid booking data", errors = ModelState });
                }

                var existingBooking = await _bookingService.GetBookingById(id);
                if (existingBooking == null)
                {
                    return NotFound(new { message = $"Booking with ID {id} not found" });
                }

                existingBooking.StartDate = request.StartDate;
                existingBooking.EndDate = request.EndDate;
                existingBooking.Type = request.Type;

                await _bookingService.UpdateBooking(existingBooking, request.UpdatedBy);

                var updatedBooking = await _bookingService.GetBookingById(id);
                var response = new BookingResponseDto
                {
                    Id = updatedBooking.Id,
                    UserId = updatedBooking.UserId,
                    RoomId = updatedBooking.RoomId,
                    StartDate = updatedBooking.StartDate,
                    EndDate = updatedBooking.EndDate,
                    Type = updatedBooking.Type,
                    Status = updatedBooking.Status,
                    UpdatedAt = updatedBooking.UpdatedAt,
                    RoomName = updatedBooking.Room?.Name
                };

                return Ok(new
                {
                    message = "Booking updated successfully",
                    success = true,
                    booking = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating booking {id}");
                return StatusCode(500, new { message = ex.Message, success = false });
            }
        }

        /// <summary>
        /// Approve booking (Admin)
        /// </summary>
        [HttpPut("{id}/approve")]
        [AllowAnonymous]
        public async Task<IActionResult> Approve(string id, [FromBody] UpdateStatusRequest request)
        {
            try
            {
                var success = await _bookingService.ApproveBooking(id, request.UpdatedBy);
                if (!success)
                {
                    return NotFound(new { message = $"Booking with ID {id} not found" });
                }

                return Ok(new
                {
                    message = "Booking approved successfully",
                    success = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error approving booking {id}");
                return StatusCode(500, new { message = ex.Message, success = false });
            }
        }

        /// <summary>
        /// Reject booking (Admin)
        /// </summary>
        [HttpPut("{id}/reject")]
        [AllowAnonymous]
        public async Task<IActionResult> Reject(string id, [FromBody] UpdateStatusRequest request)
        {
            try
            {
                var success = await _bookingService.RejectBooking(id, request.UpdatedBy);
                if (!success)
                {
                    return NotFound(new { message = $"Booking with ID {id} not found" });
                }

                return Ok(new
                {
                    message = "Booking rejected successfully",
                    success = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error rejecting booking {id}");
                return StatusCode(500, new { message = ex.Message, success = false });
            }
        }

        /// <summary>
        /// Cancel booking
        /// </summary>
        [HttpPut("{id}/cancel")]
        [AllowAnonymous]
        public async Task<IActionResult> Cancel(string id, [FromBody] UpdateStatusRequest request)
        {
            try
            {
                await _bookingService.CancelBooking(id, request.UpdatedBy);

                return Ok(new
                {
                    message = "Booking cancelled successfully",
                    success = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error cancelling booking {id}");
                return StatusCode(500, new { message = ex.Message, success = false });
            }
        }

        /// <summary>
        /// Delete booking
        /// </summary>
        [HttpDelete("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var booking = await _bookingService.GetBookingById(id);
                if (booking == null)
                {
                    return NotFound(new { message = $"Booking with ID {id} not found" });
                }

                await _bookingService.DeleteBooking(id);
                return Ok(new { message = "Booking deleted successfully", success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting booking {id}");
                return StatusCode(500, new { message = "Failed to delete booking", success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// Delete entire recurring booking series
        /// Deletes all bookings that match the same user, room, time, and type
        /// </summary>
        [HttpDelete("{id}/recurring")]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteRecurringSeries(string id)
        {
            try
            {
                var deletedCount = await _bookingService.DeleteRecurringBookingSeries(id, null);
                return Ok(new 
                { 
                    message = $"Successfully deleted {deletedCount} recurring bookings", 
                    success = true,
                    deletedCount 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting recurring booking series for {id}");
                return StatusCode(500, new { message = ex.Message, success = false });
            }
        }
    }

    // Response DTO to prevent circular reference
    public class BookingResponseDto
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string RoomId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string RoomName { get; set; }
    }

    // Request Models
    public class CreateBookingRequest
    {
        public string UserId { get; set; }
        public string RoomId { get; set; }
        public DateTime StartDate { get; set; }  // e.g., "2025-11-28T17:00:00"
        public DateTime EndDate { get; set; }    // e.g., "2025-11-28T19:00:00"
        public string Type { get; set; }
        public string CreatedBy { get; set; }
    }

    public class CreateRecurringBookingRequest
    {
        public string UserId { get; set; }
        public string RoomId { get; set; }

        /// <summary>
        /// Start date + time for the first booking (e.g., "2025-10-11T17:00:00")
        /// The time part (17:00:00) will be used for all recurring bookings
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// End time for each booking (e.g., "2025-10-11T19:00:00")
        /// Only the time part (19:00:00) matters - it will be applied to each booking
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// When to stop creating recurring bookings (e.g., "2025-11-29")
        /// This is NOT stored in the Booking table - it's just used to limit the recurrence
        /// </summary>
        public DateTime RecurrenceEndDate { get; set; }

        public string Type { get; set; }

        /// <summary>
        /// Selected days of the week: 0=Su, 1=M, 2=T, 3=W, 4=Th, 5=F, 6=S
        /// Example: [1, 2, 3, 4, 5] for Monday through Friday
        /// </summary>
        public List<int> SelectedDays { get; set; }
    }

    public class UpdateBookingRequest
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Type { get; set; }
        public string UpdatedBy { get; set; }
    }

    public class UpdateStatusRequest
    {
        public string UpdatedBy { get; set; }
    }
}
