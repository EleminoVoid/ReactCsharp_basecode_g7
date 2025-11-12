using ASI.Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Interfaces
{
    public interface IBookingRepository
    {
        IQueryable<Booking> GetBookings();
        Booking GetBookingById(string id);
        IQueryable<Booking> GetBookingsByUserId(string userId);
        IQueryable<Booking> GetBookingsByRoomId(string roomId);
        IQueryable<Booking> GetBookingsByStatus(string status);
        IQueryable<Booking> GetPendingBookings();
        IQueryable<Booking> GetUpcomingBookings(DateTime fromDate);
        IQueryable<Booking> GetBookingsInDateRange(DateTime startDate, DateTime endDate);
        IQueryable<Booking> GetConflictingBookings(string roomId, DateTime startDate, DateTime endDate, string excludeBookingId = null);
        IQueryable<Booking> GetRecurringBookingsSeries(string userId, string roomId, TimeSpan startTime, TimeSpan endTime, string type, DateTime fromDate);
        void AddBooking(Booking booking);
        void UpdateBooking(Booking booking);
        void DeleteBooking(Booking booking);
        void DeleteBookings(IEnumerable<Booking> bookings);
        Task SaveChangesAsync();
        bool HasConflict(string roomId, DateTime startDate, DateTime endDate, string excludeBookingId = null);
    }
}
