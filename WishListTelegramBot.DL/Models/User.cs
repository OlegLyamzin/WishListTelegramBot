using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WishListTelegramBot.DL.Models
{
    public class User
    {
        public int? Id { get; set; }
        public string? TgId { get; set; }
        public string? Username { get; set; }
        public string? Name { get; set; }
        public string? Lastname { get; set; }
        public DateTime CreatedOn { get; set; }

        // Navigation property for related WishLists
        public ICollection<WishList> WishLists { get; set; }
    }
}
