#nullable enable

using System;
using System.ComponentModel.DataAnnotations;

namespace ASI.Basecode.Data.Models
{
    public class RoomAmenity
    {
        [Required]
        public string RoomId { get; set; } = string.Empty;

        [Required]
        public string Amenity { get; set; } = string.Empty;
        
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }

        // Navigation property
        public virtual Room? Room { get; set; }
    }
}
