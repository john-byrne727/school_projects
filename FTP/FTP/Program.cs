using System;

namespace FTP
{
    /// <summary>
    /// <author>John Byrne (jpb2380)</author>
    /// Entry point for the FTP client program.
    /// </summary>
    class Program
    {
        private static bool debugMode;

        /// <summary>
        /// Main for the FTP client Program. 
        /// </summary>
        /// <param name="args">The server to connect to, and optionally the
        /// port to connect on.</param>
        static void Main(string[] args)
        {
            if (args.Length < 1 || args.Length > 2)
            {
                Console.Error.WriteLine("Usage: [mono] Ftp server [port]");
                Environment.Exit(1);
            }

            debugMode = false;
            var server = args[0];
            int port;
            if (args.Length == 2)
            {
                bool portGood=Int32.TryParse(args[1], out port);
                if (!portGood)
                {
                    Console.Error.WriteLine("Usage: [mono] Ftp server [port]\n"+
                        "If a port is provided it must be an integer.");
                    Environment.Exit(1);
                }
            }
            else
            {
                port = 21;
            }
            var cmd = new CommandConnection(server, port);
            var console = new ConsoleReader(cmd);

            while (console.run()) ;
        }

        /// <summary>
        /// If debug mode is active, prints a message to the screen.
        /// </summary>
        /// <param name="message">The message to print.</param>
        public static void Debug(string message)
        {
            if(debugMode)
                Console.WriteLine("DEBUG: " + message);
        }
        
        /// <summary>
        /// Toggles debug mode.
        /// </summary>
        public static void ToggleDebug()
        {
            debugMode = !debugMode;
        }
    }
}
