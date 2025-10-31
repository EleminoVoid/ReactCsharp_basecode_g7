using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.Manager;
using AutoMapper;
using System;
using System.Linq;
using System.Threading.Tasks;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.Services.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository repository, IMapper mapper)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public LoginResult AuthenticateUser(string userId, string password, ref User user)
        {
            user = new User();
            var passwordKey = PasswordManager.EncryptPassword(password);
            user = _repository.GetUsers().Where(x => x.Id == userId &&
                                                     x.Password == passwordKey).FirstOrDefault();

            return user != null ? LoginResult.Success : LoginResult.Failed;
        }

        public async Task<User> GetUser(string userId)
        {
            return await Task.FromResult(_repository.GetUsers().FirstOrDefault(x => x.Id == userId));
        }

        public async Task<User> GetUserByUsernameOrEmail(string usernameOrEmail)
        {
            return await Task.FromResult(_repository.GetUsers()
                .FirstOrDefault(x => x.Username.ToLower() == usernameOrEmail.ToLower() || 
                                     x.Email.ToLower() == usernameOrEmail.ToLower()));
        }

        public object GetAllUsers()
        {
            return _repository.GetUsers().ToList();
        }

        public async Task AddUser(User user)
        {
            // Hash the password before saving
            user.Password = PasswordManager.EncryptPassword(user.Password);
            user.CreatedAt = DateTime.Now;
            user.Id = Guid.NewGuid().ToString();
            
            _repository.AddUser(user);
            await _repository.SaveChangesAsync();
        }

        public async Task DeleteUser(string id)
        {
            var user = _repository.GetUsers().FirstOrDefault(x => x.Id == id);
            if (user != null)
            {
                _repository.DeleteUser(user);
                await _repository.SaveChangesAsync();
            }
        }

        public async Task<bool> ChangePassword(string userId, string currentPassword, string newPassword)
        {
            var user = _repository.GetUsers().FirstOrDefault(x => x.Id == userId);
            
            if (user == null)
            {
                return false;
            }

            // Verify current password
            var isValidPassword = PasswordManager.VerifyPassword(currentPassword, user.Password);
            if (!isValidPassword)
            {
                return false;
            }

            // Update with new password
            user.Password = PasswordManager.EncryptPassword(newPassword);
            user.UpdatedAt = DateTime.Now;
            
            _repository.UpdateUser(user);
            await _repository.SaveChangesAsync();

            return true;
        }
    }
}
