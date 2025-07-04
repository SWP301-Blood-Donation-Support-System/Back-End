using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisinessLayer.Utils.EmailConfiguration
{
    public class EmailConfiguration
    {
        public string From { get; set; } 
        public string SmtpServer { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }  // Changed from UserName to Username to match secrets.json
        public string Password { get; set; }
    }
}
