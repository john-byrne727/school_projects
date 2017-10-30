using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Models
{
    /// <summary>
    /// Represents the commands to be received from the front end in JSON.
    /// </summary>
    public class FlakCommand
    {
        public String command;
        public String message;
    }
}
