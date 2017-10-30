using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Models
{
    /// <summary>
    /// Represents the login data to be recieved from the front end in JSON
    /// </summary>
    public class FlakLogin
    {
        public String username;
        public String password;
        public String date;
    }
}
