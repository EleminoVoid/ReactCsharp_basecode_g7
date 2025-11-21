using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.Manager;
using ASI.Basecode.WebApp.Authentication;
using ASI.Basecode.WebApp.Models;
using ASI.Basecode.WebApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASI.Basecode.WebApp.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class AccountController : ControllerBase<AccountController>
    {
        private readonly SessionManager _sessionManager;
        private readonly SignInManager _signInManager;
        private readonly TokenValidationParametersFactory _tokenValidationParametersFactory;
        private readonly TokenProviderOptionsFactory _tokenProviderOptionsFactory;
        private readonly IConfiguration _appConfiguration;
        private readonly IUserService _userService;

        public AccountController(
                            SignInManager signInManager,
                            IHttpContextAccessor httpContextAccessor,
                            ILoggerFactory loggerFactory,
                            IConfiguration configuration,
                            IMapper mapper,
                            IUserService userService,
                            TokenValidationParametersFactory tokenValidationParametersFactory,
                            TokenProviderOptionsFactory tokenProviderOptionsFactory) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            this._sessionManager = new SessionManager(this._session);
            this._signInManager = signInManager;
            this._tokenProviderOptionsFactory = tokenProviderOptionsFactory;
            this._tokenValidationParametersFactory = tokenValidationParametersFactory;
            this._appConfiguration = configuration;
            this._userService = userService;
        }

        /// <summary>
        /// Login Method
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userService.GetUserByUsernameOrEmail(model.UserId);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }

            var isValidPassword = PasswordManager.VerifyPassword(model.Password, user.Password);
            if (!isValidPassword)
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }

            await this._signInManager.SignInAsync(user);
            this._session.SetString("HasSession", "Exist");
            this._session.SetString("UserName", user.Username);
            this._session.SetString("UserId", user.Id);
            this._session.SetString("UserRole", user.Role);

            return Ok(new { 
                user.Id,
                user.Username,
                user.Email,
                user.Role,
                user.Avatar,
                message = "Login successful"
            });
        }

        /// <summary>
        /// Sign Out current account
        /// </summary>
        [HttpPost("signout")]
        [AllowAnonymous]
        public async Task<IActionResult> SignOutUser()
        {
            await this._signInManager.SignOutAsync();
            this._session.Clear();
            return Ok(new { message = "Logged out successfully" });
        }

        /// <summary>
        /// Get all users
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetAll()
        {
            try
            {
                var users = _userService.GetAllUsers();
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all users");
                return StatusCode(500, new { message = "Failed to retrieve users", error = ex.Message });
            }
        }

        /// <summary>
        /// Get user by ID
        /// </summary>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var user = await _userService.GetUser(id);
                if (user == null)
                {
                    return NotFound(new { message = $"User with ID {id} not found" });
                }

                // Return user without password
                return Ok(new
                {
                    user.Id,
                    user.Username,
                    user.Email,
                    user.Role,
                    user.Avatar,
                    user.CreatedAt,
                    user.UpdatedAt,
                    user.CreatedBy,
                    user.UpdatedBy
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting user {id}");
                return StatusCode(500, new { message = "Failed to retrieve user", error = ex.Message });
            }
        }

        /// <summary>
        /// Search users by username or email
        /// </summary>
        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<IActionResult> Search([FromQuery] string query)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                {
                    return BadRequest(new { message = "Search query is required" });
                }

                var user = await _userService.GetUserByUsernameOrEmail(query);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                // Return user without password
                return Ok(new
                {
                    user.Id,
                    user.Username,
                    user.Email,
                    user.Role,
                    user.Avatar,
                    user.CreatedAt
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error searching user {query}");
                return StatusCode(500, new { message = "Failed to search user", error = ex.Message });
            }
        }

        /// <summary>
        /// Register/Add a new user
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "Invalid user data", errors = ModelState });
                }

                // Check if username already exists
                var existingUser = await _userService.GetUserByUsernameOrEmail(request.Username);
                if (existingUser != null)
                {
                    return BadRequest(new { message = "Username or email already exists" });
                }

                var user = new User
                {
                    Username = request.Username,
                    Email = request.Email,
                    Password = request.Password,
                    Role = request.Role ?? "User",
                    Avatar = request.Avatar,
                    CreatedBy = request.CreatedBy
                };

                await _userService.AddUser(user);

                return Ok(new
                {
                    message = "User registered successfully",
                    success = true,
                    user = new
                    {
                        user.Id,
                        user.Username,
                        user.Email,
                        user.Role,
                        user.Avatar,
                        user.CreatedAt
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering user");
                return StatusCode(500, new { message = "Failed to register user", success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// Update user details
        /// </summary>
        [HttpPut("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "Invalid user data", errors = ModelState });
                }

                var user = await _userService.GetUser(id);
                if (user == null)
                {
                    return NotFound(new { message = $"User with ID {id} not found" });
                }

                // Check if new username/email already exists (excluding current user)
                if (!string.IsNullOrEmpty(request.Username) && request.Username != user.Username)
                {
                    var existingUser = await _userService.GetUserByUsernameOrEmail(request.Username);
                    if (existingUser != null && existingUser.Id != id)
                    {
                        return BadRequest(new { message = "Username already exists" });
                    }
                    user.Username = request.Username;
                }

                if (!string.IsNullOrEmpty(request.Email) && request.Email != user.Email)
                {
                    var existingUser = await _userService.GetUserByUsernameOrEmail(request.Email);
                    if (existingUser != null && existingUser.Id != id)
                    {
                        return BadRequest(new { message = "Email already exists" });
                    }
                    user.Email = request.Email;
                }

                // Update other fields
                if (request.Role != null)
                    user.Role = request.Role;

                if (request.Avatar != null)
                    user.Avatar = request.Avatar;

                user.UpdatedBy = request.UpdatedBy;
                user.UpdatedAt = DateTime.Now;

                await _userService.UpdateUser(user);

                return Ok(new
                {
                    message = "User updated successfully",
                    success = true,
                    user = new
                    {
                        user.Id,
                        user.Username,
                        user.Email,
                        user.Role,
                        user.Avatar,
                        user.UpdatedAt
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating user {id}");
                return StatusCode(500, new { message = "Failed to update user", success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// Update user role (Admin only)
        /// </summary>
        [HttpPut("{id}/role")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateUserRole(string id, [FromBody] UpdateRoleRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Role))
                {
                    return BadRequest(new { message = "Role is required" });
                }

                var user = await _userService.GetUser(id);
                if (user == null)
                {
                    return NotFound(new { message = $"User with ID {id} not found" });
                }

                user.Role = request.Role;
                user.UpdatedBy = request.UpdatedBy;
                user.UpdatedAt = DateTime.Now;

                await _userService.UpdateUser(user);

                return Ok(new
                {
                    message = "User role updated successfully",
                    success = true,
                    user = new
                    {
                        user.Id,
                        user.Username,
                        user.Role,
                        user.UpdatedAt
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating user role {id}");
                return StatusCode(500, new { message = "Failed to update role", success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// Delete a user by ID
        /// </summary>
        [HttpDelete("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                var user = await _userService.GetUser(id);
                if (user == null)
                {
                    return NotFound(new { message = $"User with ID {id} not found" });
                }

                await _userService.DeleteUser(id);
                return Ok(new { message = "User deleted successfully", success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting user {id}");
                return StatusCode(500, new { message = "Failed to delete user", success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// Change user password (requires current password)
        /// </summary>
        [HttpPut("change-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "Invalid data", errors = ModelState });
                }

                var result = await _userService.ChangePassword(model.UserId, model.NewPassword);

                if (!result)
                {
                    return BadRequest(new { 
                        message = "Failed to change password. Please check your current password.",
                        success = false 
                    });
                }

                return Ok(new
                {
                    message = "Password changed successfully",
                    success = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password");
                return StatusCode(500, new { 
                    message = "Failed to change password", 
                    success = false, 
                    error = ex.Message 
                });
            }
        }

        /// <summary>
        /// Admin reset user password (no current password required)
        /// </summary>
        [HttpPut("{id}/reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(string id, [FromBody] ResetPasswordRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.NewPassword))
                {
                    return BadRequest(new { message = "New password is required" });
                }

                if (request.NewPassword.Length < 6)
                {
                    return BadRequest(new { message = "Password must be at least 6 characters long" });
                }

                var user = await _userService.GetUser(id);
                if (user == null)
                {
                    return NotFound(new { message = $"User with ID {id} not found" });
                }

                user.Password = PasswordManager.EncryptPassword(request.NewPassword);
                user.UpdatedBy = request.UpdatedBy;
                user.UpdatedAt = DateTime.Now;

                await _userService.UpdateUser(user);

                return Ok(new
                {
                    message = "Password reset successfully",
                    success = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error resetting password for user {id}");
                return StatusCode(500, new { 
                    message = "Failed to reset password", 
                    success = false, 
                    error = ex.Message 
                });
            }
        }
    }

    // Request Models
    public class RegisterUserRequest
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string Avatar { get; set; }
        public string CreatedBy { get; set; }
    }

    public class UpdateUserRequest
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string Avatar { get; set; }
        public string UpdatedBy { get; set; }
    }

    public class UpdateRoleRequest
    {
        public string Role { get; set; }
        public string UpdatedBy { get; set; }
    }

    public class ResetPasswordRequest
    {
        public string NewPassword { get; set; }
        public string UpdatedBy { get; set; }
    }
}
