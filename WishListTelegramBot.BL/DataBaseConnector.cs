using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using WishListTelegramBot.DL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WishListTelegramBot.BL
{
    public class DataBaseConnector
    {
        public WishListService WishListService;
        public WishService WishService;
        public UserService UserService;

        public DataBaseConnector(WishListService wishListService,
            WishService wishService,
            UserService userService) 
        {
            WishListService = wishListService;
            WishService = wishService;
            UserService = userService;
        }

    }
}
