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
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _repository;
        private readonly IRoomRepository _roomRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public BookingService(IBookingRepository repository, IRoomRepository roomRepository, IUserRepository userRepository, IMapper mapper)
        {
            _repository = repository;
            _roomRepository = roomRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public IEnumerable<Booking> GetAllBookings()
        {
            return _repository.GetBookings().ToList();
        }

        public async Task<Booking> GetBookingById(string id)
        {
            return await Task.FromResult(_repository.GetBookingById(id));
        }

        public async Task<IEnumerable<Booking>> GetBookingsByUserId(string userId)
        {
            var bookings = _repository.GetBookingsByUserId(userId).ToList();
            return await Task.FromResult(bookings);
        }

        public async Task<IEnumerable<Booking>> GetBookingsByRoomId(string roomId)
        {
            var bookings = _repository.GetBookingsByRoomId(roomId).ToList();
            return await Task.FromResult(bookings);
        }

        public async Task<IEnumerable<Booking>> GetPendingBookings()
        {
            var bookings = _repository.GetPendingBookings().ToList();
            return await Task.FromResult(bookings);
        }

        public async Task<IEnumerable<Booking>> GetUpcomingBookings()
        {
            var bookings = _repository.GetUpcomingBookings(DateTime.Now).ToList();
            return await Task.FromResult(bookings);
        }

        public async Task<IEnumerable<Booking>> GetBookingsByDateRange(DateTime startDate, DateTime endDate)
        {
            var bookings = _repository.GetBookingsInDateRange(startDate, endDate).ToList();
            return await Task.FromResult(bookings);
        }

        public async Task<string> CreateBooking(Booking booking, string createdBy = null)
        {
            // Validate room exists
            var room = _roomRepository.GetRoomById(booking.RoomId);
            if (room == null)
            {
                throw new Exception($"Room with ID {booking.RoomId} not found");
            }

            // Validate user exists
            var user = _userRepository.GetUsers().FirstOrDefault(u => u.Id == booking.UserId);
            if (user == null)
            {
                throw new Exception($"User with ID {booking.UserId} not found");
            }

            // Validate dates
            if (booking.StartDate >= booking.EndDate)
            {
                throw new Exception("End date must be after start date");
            }

            if (booking.StartDate < DateTime.Now)
            {
                throw new Exception("Cannot create booking in the past");
            }

            // Check for conflicts
            if (_repository.HasConflict(booking.RoomId, booking.StartDate, booking.EndDate))
            {
                throw new Exception("Room is not available for the selected time period");
            }

            // Set booking properties
            booking.Id = Guid.NewGuid().ToString();
            booking.CreatedAt = DateTime.Now;
            booking.CreatedBy = createdBy;
            booking.Status = "Pending";

            _repository.AddBooking(booking);
            await _repository.SaveChangesAsync();

            return booking.Id;
        }

        public async Task<List<string>> CreateRecurringBooking(RecurringBookingRequest request, string createdBy = null)
        {
            var createdBookingIds = new List<string>();
            var failedDates = new List<DateTime>();

            // Validate that user selected at least one day
            if (request.SelectedDays == null || !request.SelectedDays.Any())
            {
                throw new Exception("Please select at least one day of the week for recurring booking");
            }

            // Validate date range
            if (request.StartDate > request.RecurrenceEndDate)
            {
                throw new Exception("Recurrence end date must be after start date");
            }

            // Generate dates based on selected days
            var bookingDates = GenerateBookingDatesFromSelectedDays(request);

            foreach (var date in bookingDates)
            {
                var startDateTime = date.Add(request.StartTime);
                var endDateTime = date.Add(request.EndTime);

                // Check for conflicts
                if (!_repository.HasConflict(request.RoomId, startDateTime, endDateTime))
                {
                    var booking = new Booking
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserId = request.UserId,
                        RoomId = request.RoomId,
                        StartDate = startDateTime,
                        EndDate = endDateTime,
                        Type = request.Type,
                        Status = "Pending",
                        CreatedBy = createdBy,
                        CreatedAt = DateTime.Now
                    };

                    _repository.AddBooking(booking);
                    createdBookingIds.Add(booking.Id);
                }
                else
                {
                    failedDates.Add(date);
                }
            }

            await _repository.SaveChangesAsync();

            if (failedDates.Any())
            {
                var message = $"Created {createdBookingIds.Count} bookings. Failed to create {failedDates.Count} bookings due to conflicts on: {string.Join(", ", failedDates.Select(d => d.ToShortDateString()))}";
                throw new Exception(message);
            }

            return createdBookingIds;
        }

        /// <summary>
        /// Generates booking dates based on selected days of the week
        /// </summary>
        private List<DateTime> GenerateBookingDatesFromSelectedDays(RecurringBookingRequest request)
        {
            var dates = new List<DateTime>();
            var currentDate = request.StartDate.Date;

            // Convert selected day numbers to DayOfWeek enum
            var selectedDaysOfWeek = request.SelectedDays.Select(d => (DayOfWeek)d).ToList();

            while (currentDate <= request.RecurrenceEndDate.Date)
            {
                // Check if current day is in the selected days
                if (selectedDaysOfWeek.Contains(currentDate.DayOfWeek))
                {
                    dates.Add(currentDate);
                }

                currentDate = currentDate.AddDays(1);
            }

            return dates;
        }

        public async Task UpdateBooking(Booking booking, string updatedBy = null)
        {
            var existingBooking = _repository.GetBookingById(booking.Id);
            if (existingBooking == null)
            {
                throw new Exception($"Booking with ID {booking.Id} not found");
            }

            // Validate dates
            if (booking.StartDate >= booking.EndDate)
            {
                throw new Exception("End date must be after start date");
            }

            // Check for conflicts (excluding current booking)
            if (_repository.HasConflict(booking.RoomId, booking.StartDate, booking.EndDate, booking.Id))
            {
                throw new Exception("Room is not available for the selected time period");
            }

            existingBooking.StartDate = booking.StartDate;
            existingBooking.EndDate = booking.EndDate;
            existingBooking.Type = booking.Type;
            existingBooking.Status = booking.Status;
            existingBooking.UpdatedBy = updatedBy;
            existingBooking.UpdatedAt = DateTime.Now;

            _repository.UpdateBooking(existingBooking);
            await _repository.SaveChangesAsync();
        }

        public async Task<bool> UpdateBookingStatus(string id, string status, string updatedBy = null)
        {
            var booking = _repository.GetBookingById(id);
            if (booking == null)
            {
                return false;
            }

            booking.Status = status;
            booking.UpdatedBy = updatedBy;
            booking.UpdatedAt = DateTime.Now;

            _repository.UpdateBooking(booking);
            await _repository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ApproveBooking(string id, string updatedBy = null)
        {
            return await UpdateBookingStatus(id, "Approved", updatedBy);
        }

        public async Task<bool> RejectBooking(string id, string updatedBy = null)
        {
            return await UpdateBookingStatus(id, "Rejected", updatedBy);
        }

        public async Task CancelBooking(string id, string updatedBy = null)
        {
            var booking = _repository.GetBookingById(id);
            if (booking == null)
            {
                throw new Exception($"Booking with ID {id} not found");
            }

            booking.Status = "Cancelled";
            booking.UpdatedBy = updatedBy;
            booking.UpdatedAt = DateTime.Now;

            _repository.UpdateBooking(booking);
            await _repository.SaveChangesAsync();
        }

        public async Task DeleteBooking(string id)
        {
            var booking = _repository.GetBookingById(id);
            if (booking != null)
            {
                _repository.DeleteBooking(booking);
                await _repository.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Delete all bookings in a recurring series
        /// Finds bookings with matching user, room, time, and type
        /// </summary>
        public async Task<int> DeleteRecurringBookingSeries(string bookingId, string deletedBy = null)
        {
            var originalBooking = _repository.GetBookingById(bookingId);
            if (originalBooking == null)
            {
                throw new Exception($"Booking with ID {bookingId} not found");
            }

            // Find all bookings in the same recurring series
            var startTime = originalBooking.StartDate.TimeOfDay;
            var endTime = originalBooking.EndDate.TimeOfDay;

            var recurringBookings = _repository.GetRecurringBookingsSeries(
                originalBooking.UserId,
                originalBooking.RoomId,
                startTime,
                endTime,
                originalBooking.Type,
                originalBooking.StartDate.Date
            ).ToList();

            if (recurringBookings.Count == 0)
            {
                throw new Exception("No recurring bookings found");
            }

            // Delete all bookings in the series
            _repository.DeleteBookings(recurringBookings);
            await _repository.SaveChangesAsync();

            return recurringBookings.Count;
        }

        public async Task<bool> CheckAvailability(string roomId, DateTime startDate, DateTime endDate, string excludeBookingId = null)
        {
            var hasConflict = _repository.HasConflict(roomId, startDate, endDate, excludeBookingId);
            return await Task.FromResult(!hasConflict);
        }

        public async Task<List<DateTime>> GetAvailableDates(string roomId, DateTime startDate, DateTime endDate)
        {
            var availableDates = new List<DateTime>();
            var currentDate = startDate.Date;

            while (currentDate <= endDate.Date)
            {
                // Check if there are any bookings on this date
                var dayStart = currentDate;
                var dayEnd = currentDate.AddDays(1);

                if (!_repository.HasConflict(roomId, dayStart, dayEnd))
                {
                    availableDates.Add(currentDate);
                }

                currentDate = currentDate.AddDays(1);
            }

            return await Task.FromResult(availableDates);
        }
    }
}
