using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Repositories
{
    public class BookingRepository : BaseRepository<Booking>, IBookingRepository
    {
        public BookingRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public IQueryable<Booking> GetBookings()
        {
            return GetAll()
                .Include(b => b.User)
                .Include(b => b.Room);
        }

        public Booking GetBookingById(string id)
        {
            return GetAll()
                .Include(b => b.User)
                .Include(b => b.Room)
                .FirstOrDefault(b => b.Id == id);
        }

        public IQueryable<Booking> GetBookingsByUserId(string userId)
        {
            return GetAll()
                .Include(b => b.Room)
                .Where(b => b.UserId == userId);
        }

        public IQueryable<Booking> GetBookingsByRoomId(string roomId)
        {
            return GetAll()
                .Include(b => b.User)
                .Where(b => b.RoomId == roomId);
        }

        public IQueryable<Booking> GetBookingsByStatus(string status)
        {
            return GetAll()
                .Include(b => b.User)
                .Include(b => b.Room)
                .Where(b => b.Status == status);
        }

        public IQueryable<Booking> GetPendingBookings()
        {
            return GetBookingsByStatus("Pending");
        }

        public IQueryable<Booking> GetUpcomingBookings(DateTime fromDate)
        {
            return GetAll()
                .Include(b => b.User)
                .Include(b => b.Room)
                .Where(b => b.StartDate >= fromDate && b.Status == "Approved");
        }

        public IQueryable<Booking> GetBookingsInDateRange(DateTime startDate, DateTime endDate)
        {
            return GetAll()
                .Include(b => b.User)
                .Include(b => b.Room)
                .Where(b => b.StartDate < endDate && b.EndDate > startDate);
        }

        public IQueryable<Booking> GetConflictingBookings(string roomId, DateTime startDate, DateTime endDate, string excludeBookingId = null)
        {
            var query = GetAll()
                .Where(b => b.RoomId == roomId &&
                           b.Status != "Cancelled" &&
                           b.Status != "Rejected" &&
                           b.StartDate < endDate &&
                           b.EndDate > startDate);

            if (!string.IsNullOrEmpty(excludeBookingId))
            {
                query = query.Where(b => b.Id != excludeBookingId);
            }

            return query;
        }

        /// <summary>
        /// Find bookings that are part of the same recurring series based on pattern matching
        /// </summary>
        public IQueryable<Booking> GetRecurringBookingsSeries(string userId, string roomId, TimeSpan startTime, TimeSpan endTime, string type, DateTime fromDate)
        {
            var duration = endTime - startTime;

            return GetAll()
                .Where(b => b.UserId == userId &&
                           b.RoomId == roomId &&
                           b.Type == type &&
                           b.StartDate.TimeOfDay == startTime &&
                           b.EndDate.TimeOfDay == endTime &&
                           b.StartDate.Date >= fromDate.Date);
        }

        public bool HasConflict(string roomId, DateTime startDate, DateTime endDate, string excludeBookingId = null)
        {
            return GetConflictingBookings(roomId, startDate, endDate, excludeBookingId).Any();
        }

        public void AddBooking(Booking booking)
        {
            Add(booking);
        }

        public void UpdateBooking(Booking booking)
        {
            Update(booking);
        }

        public void DeleteBooking(Booking booking)
        {
            Delete(booking);
        }

        public void DeleteBookings(IEnumerable<Booking> bookings)
        {
            foreach (var booking in bookings)
            {
                Delete(booking);
            }
        }

        public async Task SaveChangesAsync()
        {
            await UnitOfWork.SaveChangesAsync();
        }
    }
}
