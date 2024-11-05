using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WishListTelegramBot.DL.Models
{
    public class Wish
    {
        public int Id { get; set; }
        public int? WishListId { get; set; }
        public string? Name { get; set; }
        public int? GiverId { get; set; }
        public string? Description { get; set; }
        public string? PhotoId { get; set; }
        public string? Link { get; set; }

        // Navigation properties
        public WishList? WishList { get; set; }
        public User? Giver { get; set; }
    }
}
