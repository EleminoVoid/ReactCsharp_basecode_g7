using ASI.Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IBookingService
    {
        IEnumerable<Booking> GetAllBookings();
        Task<Booking> GetBookingById(string id);
        Task<IEnumerable<Booking>> GetBookingsByUserId(string userId);
        Task<IEnumerable<Booking>> GetBookingsByRoomId(string roomId);
        Task<IEnumerable<Booking>> GetPendingBookings();
        Task<IEnumerable<Booking>> GetUpcomingBookings();
        Task<IEnumerable<Booking>> GetBookingsByDateRange(DateTime startDate, DateTime endDate);
        Task<string> CreateBooking(Booking booking, string createdBy = null);
        Task<List<string>> CreateRecurringBooking(RecurringBookingRequest request, string createdBy = null);
        Task UpdateBooking(Booking booking, string updatedBy = null);
        Task<bool> UpdateBookingStatus(string id, string status, string updatedBy = null);
        Task<bool> ApproveBooking(string id, string updatedBy = null);
        Task<bool> RejectBooking(string id, string updatedBy = null);
        Task CancelBooking(string id, string updatedBy = null);
        Task DeleteBooking(string id);
        Task<int> DeleteRecurringBookingSeries(string bookingId, string deletedBy = null);
        Task<bool> CheckAvailability(string roomId, DateTime startDate, DateTime endDate, string excludeBookingId = null);
        Task<List<DateTime>> GetAvailableDates(string roomId, DateTime startDate, DateTime endDate);
    }

    /// <summary>
    /// Request model for creating recurring bookings based on selected days of the week
    /// </summary>
    public class RecurringBookingRequest
    {
        public string UserId { get; set; }
        public string RoomId { get; set; }

        /// <summary>
        /// Start date for the recurring series
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// End date for the recurring series
        /// </summary>
        public DateTime RecurrenceEndDate { get; set; }

        /// <summary>
        /// Start time for each booking (e.g., "05:00 pm")
        /// </summary>
        public TimeSpan StartTime { get; set; }

        /// <summary>
        /// End time for each booking (e.g., "07:00 pm")
        /// </summary>
        public TimeSpan EndTime { get; set; }

        public string Type { get; set; }

        /// <summary>
        /// Selected days of the week for recurring bookings
        /// Values: 0=Sunday, 1=Monday, 2=Tuesday, 3=Wednesday, 4=Thursday, 5=Friday, 6=Saturday
        /// Example: [1, 2, 3, 4, 5] for Monday through Friday
        /// </summary>
        public List<int> SelectedDays { get; set; }
    }
}
