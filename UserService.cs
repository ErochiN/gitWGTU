using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace GitВГТУ
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _dbContext;

        public UserService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> RegisterAsync(string firstName, string lastName, string group, string email, string password)
        {
            if (await _dbContext.Users.AnyAsync(p => p.Email == email)) return false;

            string hash = BCrypt.Net.BCrypt.HashPassword(password);

            var user = new users
            {
                FirstName = firstName,
                LastName = lastName,
                Group = group,
                Email = email,
                Hash_password = hash
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> IniUser(string email, string password)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(p => p.Email == email);

            if (user == null) return false;

            return BCrypt.Net.BCrypt.Verify(password, user.Hash_password);
        }

        public async Task<int> GetUserId(string email)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
            var id = user?.Id;

            return (int)id;
        }
    }
}
