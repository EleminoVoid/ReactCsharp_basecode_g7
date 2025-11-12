using System.ComponentModel.DataAnnotations;

namespace ASI.Basecode.WebApp.Models
{
    public class ForgotPasswordRequest
    {
        [Required]
        public string UsernameOrEmail { get; set; }
    }
}
