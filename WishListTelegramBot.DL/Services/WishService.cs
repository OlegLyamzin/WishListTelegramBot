using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WishListTelegramBot.DL.Models;

namespace WishListTelegramBot.DL.Services
{
    
    public class WishService
    {
        private readonly AppDbContext _context;

        public WishService(AppDbContext context)
        {
            _context = context;
        }

        // Create Wish
        public async Task<Wish> CreateWishAsync(Wish wish)
        {
            _context.Wish.Add(wish);
            await _context.SaveChangesAsync();
            return wish;
        }

        // Read Wishes
        public async Task<List<Wish>> GetWishesAsync()
        {
            return await _context.Wish.ToListAsync();
        }

        // Get Wish by Id
        public async Task<Wish> GetWishByIdAsync(int id)
        {
            return await _context.Wish.FindAsync(id);
        }

        // Update Wish
        public async Task UpdateWishAsync(Wish wish)
        {
            var existingWish = await _context.Wish.FindAsync(wish.Id);
            if (existingWish != null)
            {
                existingWish.Name = wish.Name;
                existingWish.GiverId = wish.GiverId;
                await _context.SaveChangesAsync();
            }
        }

        // Delete Wish
        public void DeleteWish(int id)
        {
            var wish = _context.Wish.Find(id);
            if (wish != null)
            {
                _context.Wish.Remove(wish);
                _context.SaveChanges();
            }
        }

        public async Task<List<Wish>> GetWishesByWishListIdAsync(int wishListId)
        {
            return await _context.Wish.Where(wish => wish.WishListId == wishListId).ToListAsync();
        }
    }
}
