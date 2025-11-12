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
            user = _repository.GetUsers().Where(x => x.Id == userId && x.Password == passwordKey).FirstOrDefault();
            return user != null ? LoginResult.Success : LoginResult.Failed;
        }

        public async Task<User> GetUser(string userId)
            => await Task.FromResult(_repository.GetUsers().FirstOrDefault(x => x.Id == userId));

        public async Task<User> GetUserByUsernameOrEmail(string usernameOrEmail)
            => await Task.FromResult(_repository.GetUsers()
                   .FirstOrDefault(x => x.Username.ToLower() == usernameOrEmail.ToLower()
                                     || x.Email.ToLower() == usernameOrEmail.ToLower()));

        public object GetAllUsers() => _repository.GetUsers().ToList();

        public async Task AddUser(User user)
        {
            user.Password = PasswordManager.EncryptPassword(user.Password);
            user.CreatedAt = DateTime.Now;
            user.Id = Guid.NewGuid().ToString();
            _repository.AddUser(user);
            await _repository.SaveChangesAsync();
        }

        public async Task UpdateUser(User user)
        {
            var existingUser = _repository.GetUsers().FirstOrDefault(x => x.Id == user.Id)
                               ?? throw new Exception($"User with ID {user.Id} not found");

            existingUser.Username = user.Username;
            existingUser.Email = user.Email;
            existingUser.Role = user.Role;
            existingUser.Avatar = user.Avatar;

            if (!string.IsNullOrEmpty(user.Password) && user.Password != existingUser.Password)
                existingUser.Password = user.Password;

            existingUser.UpdatedBy = user.UpdatedBy;
            existingUser.UpdatedAt = user.UpdatedAt ?? DateTime.Now;

            _repository.UpdateUser(existingUser);
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
            if (user == null) return false;
            if (!PasswordManager.VerifyPassword(currentPassword, user.Password)) return false;

            user.Password = PasswordManager.EncryptPassword(newPassword);
            user.UpdatedAt = DateTime.Now;
            _repository.UpdateUser(user);
            await _repository.SaveChangesAsync();
            return true;
        }

        public async Task<string?> GeneratePasswordResetToken(string usernameOrEmail, int minutesValid = 15)
        {
            var user = await GetUserByUsernameOrEmail(usernameOrEmail);
            if (user == null) return null;

            user.ResetToken = Convert.ToHexString(Guid.NewGuid().ToByteArray()) + Convert.ToHexString(Guid.NewGuid().ToByteArray());
            user.ResetTokenExpires = DateTime.UtcNow.AddMinutes(minutesValid);
            _repository.UpdateUser(user);
            await _repository.SaveChangesAsync();
            return user.ResetToken;
        }

        public async Task<bool> ResetPasswordWithToken(string token, string newPassword)
        {
            var user = _repository.GetUsers()
                .FirstOrDefault(u => u.ResetToken == token && u.ResetTokenExpires != null && u.ResetTokenExpires >= DateTime.UtcNow);

            if (user == null) return false;

            user.Password = PasswordManager.EncryptPassword(newPassword);
            user.ResetToken = null;
            user.ResetTokenExpires = null;
            user.UpdatedAt = DateTime.UtcNow;

            _repository.UpdateUser(user);
            await _repository.SaveChangesAsync();
            return true;
        }
    }
}
