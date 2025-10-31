using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ASI.Basecode.WebApp.Models
{
    /// <summary>
    /// Change Password View Model
    /// </summary>
    public class ChangePasswordViewModel
    {
        /// <summary>
        /// User ID
        /// </summary>
        [JsonPropertyName("userId")]
        [Required(ErrorMessage = "User ID is required.")]
        public string UserId { get; set; }

        /// <summary>
        /// Current Password
        /// </summary>
        [JsonPropertyName("currentPassword")]
        [Required(ErrorMessage = "Current password is required.")]
        public string CurrentPassword { get; set; }

        /// <summary>
        /// New Password
        /// </summary>
        [JsonPropertyName("newPassword")]
        [Required(ErrorMessage = "New password is required.")]
        [MinLength(6, ErrorMessage = "New password must be at least 6 characters long.")]
        public string NewPassword { get; set; }

        /// <summary>
        /// Confirm New Password
        /// </summary>
        [JsonPropertyName("confirmPassword")]
        [Required(ErrorMessage = "Confirm password is required.")]
        [Compare("NewPassword", ErrorMessage = "New password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
