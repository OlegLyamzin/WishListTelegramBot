using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WishListTelegramBot.Core
{
    public class AppSettings
    {
        public string URL { get; set; }
        public string NAME { get; set; }
        public string API_KEY { get; set; }
        public string CONNECTION_STRING { get; set; }
        //public string? USERNAME { get; set; }
        //public string? PASSWORD { get; set; }
        //public string? URL_VPNSERVER_PANEL { get; set; }
        //public string? IP_VPNSERVER { get; set; }
        public string ADMINID { get; set; }
        public string SERVERS { get; set; }
        public string SUPERVISORID { get; set; }
        //public string? INBOUNDID { get; set; }
    }

}
