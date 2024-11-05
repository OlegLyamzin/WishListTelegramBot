using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WishListTelegramBot.BL.Models.Commands;

namespace WishListTelegramBot.BL.Models
{
    public class CommandCallBackItem
    {
        public string Name { get; set; }
        public CallBackData CallBackData { get; set; }

    }
}
