#nullable enable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ASI.Basecode.Data.Models
{
    public class Room
    {
        [Required]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [MinLength(3)]
        public string Name { get; set; } = string.Empty;
        
        public string? Floor { get; set; }

        [Range(1, 1000)]
        public int? Capacity { get; set; }
        
        public bool? Available { get; set; } = true;
        public string? Description { get; set; }
        public string? Image { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public virtual ICollection<RoomAmenity>? RoomAmenities { get; set; }
        public virtual ICollection<Booking>? Bookings { get; set; }
    }
}
