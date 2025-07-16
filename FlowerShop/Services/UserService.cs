using FlowerShop.Models;

namespace FlowerShop.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public User GetUserById(int userId)
        {
            return _context.Users.Find(userId);
        }

        public void UpdateUser(UserProfileDto dto)
        {
            var user = _context.Users.Find(dto.Id);
            if (user == null) throw new Exception("Пользователь не найден");

            user.Username = dto.Username;
            if (!string.IsNullOrEmpty(dto.Password))
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            }

            _context.SaveChanges();
        }
    }
}