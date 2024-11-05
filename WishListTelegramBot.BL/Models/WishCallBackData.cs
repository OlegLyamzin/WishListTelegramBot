using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WishListTelegramBot.BL.Models
{
    public class WishCallBackData
    {
        public int Id { get; set; }
        public OperationType Operation { get;set; }
    }

    public enum OperationType
    {
        ChangePhoto, ChangeLink, ChangeDescription, ChangeName
    }
}
