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

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountController"/> class.
        /// </summary>
        /// <param name="signInManager">The sign in manager.</param>
        /// <param name="localizer">The localizer.</param>
        /// <param name="userService">The user service.</param>
        /// <param name="httpContextAccessor">The HTTP context accessor.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="tokenValidationParametersFactory">The token validation parameters factory.</param>
        /// <param name="tokenProviderOptionsFactory">The token provider tions factory.</param>
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

            // Look up user by username or email, not by ID
            var user = await _userService.GetUserByUsernameOrEmail(model.UserId);
            if (user == null)
            {
                return Unauthorized("Invalid username or password");
            }

            var isValidPassword = PasswordManager.VerifyPassword(model.Password, user.Password);
            if (!isValidPassword)
            {
                return Unauthorized("Invalid username or password");
            }

            await this._signInManager.SignInAsync(user);
            this._session.SetString("HasSession", "Exist");
            this._session.SetString("UserName", user.Username);

            return Ok(new { 
                user.Id,
                user.Username,
                user.Email,
                user.Role
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
            return Ok();
        }

        /// <summary>
        /// Get all users
        /// </summary>
        [HttpGet]
        [AllowAnonymous]  // Change this based on your auth requirements
        public IActionResult GetAll()
        {
            var users = _userService.GetAllUsers();
            return Ok(users);
        }

        /// <summary>
        /// Add a new user
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> AddUser([FromBody] User user)
        {
            try
            {
                // Validate model state
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "Invalid user data", errors = ModelState });
                }

                // Check if password is provided
                if (string.IsNullOrEmpty(user.Password))
                {
                    return BadRequest(new { message = "Password is required" });
                }

                // Add user
                await _userService.AddUser(user);

                // Return success response with user data (without password)
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
                        user.CreatedAt
                    }
                });
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "Error registering user");
                return StatusCode(500, new { message = "Failed to register user", success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// Delete a user by ID
        /// </summary>
        [HttpDelete("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteUser(string id)
        {
            // Your logic to delete user
            await _userService.DeleteUser(id);
            return Ok();
        }

        /// <summary>
        /// Change user password
        /// </summary>
        [HttpPut("change-password")]
        [AllowAnonymous] // Change to [Authorize] if you want only authenticated users to change password
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordViewModel model)
        {
            try
            {
                // Validate model state
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "Invalid data", errors = ModelState });
                }

                // Change password
                var result = await _userService.ChangePassword(model.UserId, model.CurrentPassword, model.NewPassword);

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
                // Log the exception
                _logger.LogError(ex, "Error changing password");
                return StatusCode(500, new { 
                    message = "Failed to change password", 
                    success = false, 
                    error = ex.Message 
                });
            }
        }
    }
}
