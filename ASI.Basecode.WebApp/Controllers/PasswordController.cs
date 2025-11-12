using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ASI.Basecode.WebApp.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class PasswordController : ControllerBase
    {
        private readonly IUserService _userService;
        public PasswordController(IUserService userService) => _userService = userService;

        // Step 1: request a reset token
        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest req)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Do not leak existence; in dev you can return the token for testing.
            var token = await _userService.GeneratePasswordResetToken(req.UsernameOrEmail);
            if (token == null)
                return Ok(new { message = "If the account exists, a reset token has been issued." });

            // TODO: email the token; for dev we return it:
            return Ok(new { message = "Reset token generated", token });
        }

        // Step 2: confirm reset with token
        [HttpPost("reset-password-confirm")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPasswordConfirm([FromBody] ResetPasswordConfirmRequest req)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var ok = await _userService.ResetPasswordWithToken(req.Token, req.NewPassword);
            if (!ok) return BadRequest(new { message = "Invalid or expired token" });

            return Ok(new { message = "Password reset successfully" });
        }
    }
}