using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WishListTelegramBot.DL.Models
{
    public class WishList
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string? Name { get; set; }

        // Navigation property for related User
        public User User { get; set; }

        // Navigation property for related Wishes
        public ICollection<Wish> Wishes { get; set; }
    }
}
