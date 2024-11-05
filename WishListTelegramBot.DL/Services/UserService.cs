using WishListTelegramBot.DL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace WishListTelegramBot.DL.Services
{
    public class UserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        // Create
        public async Task<User> CreateUserAsync(User user)
        {
            user.CreatedOn = DateTime.UtcNow;
            _context.User.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        // Read
        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _context.User.FindAsync(id);
        }

        // Read
        public async Task<User> GetUserByTgIdAsync(string tgId)
        {
            var users = await GetAllUsersAsync();
            return users != null ? users.Find(user => user.TgId == tgId) : null;
        }
        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _context.User.ToListAsync();
        }

        // Update
        public async Task<User> UpdateUserAsync(User user)
        {
            _context.User.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

        // Delete
        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.User.FindAsync(id);
            if (user == null) return false;

            _context.User.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

    }

}
