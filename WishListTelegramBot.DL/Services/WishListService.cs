using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WishListTelegramBot.DL.Models;

namespace WishListTelegramBot.DL.Services
{
    public class WishListService
    {
        private readonly AppDbContext _context;

        public WishListService(AppDbContext context)
        {
            _context = context;
        }

        // Create WishList
        public async Task<WishList> CreateWishListAsync(WishList wishList)
        {
            wishList.CreatedOn = DateTime.Now;
            _context.WishList.Add(wishList);
            await _context.SaveChangesAsync();
            return wishList;
        }

        // Read WishLists
        public async Task<List<WishList>> GetWishListsAsync()
        {
            return await _context.WishList.Include(wl => wl.Wishes).ToListAsync();
        }

        // Get WishList by Id
        public async Task<WishList> GetWishListByIdAsync(int id)
        {
            return await _context.WishList.Include(wl => wl.Wishes).FirstOrDefaultAsync(wl => wl.Id == id);
        }

        // Update WishList
        public async Task UpdateWishListAsync(WishList wishList)
        {
            var existingWishList = await _context.WishList.FindAsync(wishList.Id);
            if (existingWishList != null)
            {
                existingWishList.Name = wishList.Name;
                await _context.SaveChangesAsync();
            }
        }

        // Delete WishList
        public void DeleteWishList(int id)
        {
            var wishList = _context.WishList.Find(id);
            if (wishList != null)
            {
                _context.WishList.Remove(wishList);
                _context.SaveChanges();
            }
        }

        public async Task<List<WishList>> GetWishListByUserIdAsync(int? userId)
        {
            return await _context.WishList.Where(wishlist => wishlist.UserId == userId).ToListAsync();
        }
    }
}
