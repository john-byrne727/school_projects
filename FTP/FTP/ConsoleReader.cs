using System;

namespace FTP
{
    /// <summary>
    /// <author>Author: John Byrne (jpb2380)</author>
    /// Facilitates user interaction with the console.
    /// </summary>
    class ConsoleReader
    {

        public const String HELP_MESSAGE = "Valid commands are as follows:\n" +
            "ascii      : Set server to ASCII transfer type\n" +
            "binary     : Set server to binary transfer type\n" +
            "cd <path>  : Change the remote working directory to the path\n"+
            "             provided\n" +
            "cdup       : Change the remote working directory to the\n" +
            "             parent directory (i.e., cd ..)\n" +
            "debug      : Toggle client debug mode\n" +
            "dir        : List the contents of the remote directory\n" +
            "get <path> : Get a remote file (saved to local working\n"+ 
            "             directory)\n" +
            "help       : Displays this help message\n" +
            "passive    : Toggle passive/port mode\n" +
            "pwd        : Print the working directory on the server\n" +
            "quit       : Close the connection to the server and terminate\n"+
            "Commands are not case sensitive, but paths are.";

        public static readonly string[] COMMANDS = { "ascii",
                          "binary",
                          "cd",
                          "cdup",
                          "debug",
                          "dir",
                          "get",
                          "help",
                          "passive",
                          "pwd",
                          "quit" };

        public const int ASCII = 0;
        public const int BINARY = 1;
        public const int CD = 2;
        public const int CDUP = 3;
        public const int DEBUG = 4;
        public const int DIR = 5;
        public const int GET = 6;
        public const int HELP = 7;
        public const int PASSIVE = 8;
        public const int PWD = 9;
        public const int QUIT = 10;

        public const string PROMPT = "FTP> ";
        private CommandConnection command;

        /// <summary>
        /// Creates a new ConsoleReader
        /// </summary>
        /// <param name="cmd">The command connection to send commands to</param>
        public ConsoleReader(CommandConnection cmd)
        {
            command = cmd;
        }

        /// <summary>
        /// The main run loop of reading commands from the console.
        /// </summary>
        /// <returns>True if the loop should be called again, false if the
        /// program should close.</returns>
        public bool run()
        {
            String input = null;
            try
            {
                Console.Write(PROMPT);
                input = Console.ReadLine();
            }
            catch(Exception e)
            {
                return false;
            }
            if (input.Length > 0)
            {
                string[] args = input.Split(' ');
                return processCode(args[0], args);
            }
            else
            {
                Console.WriteLine("Invalid Command");
                return true;
            }
        }

        /// <summary>
        /// Processes user input and performs proper local action or calls the
        /// command connection to send the proper message to the server.
        /// </summary>
        /// <param name="code">The code of the command to execute.</param>
        /// <param name="args">Any arguments following the code.</param>
        /// <returns>True if the program should keep running, false if the
        /// program should exit.</returns>
        public bool processCode(string code, string[] args)
        {
            int cmd = -1;
            for(int i=0; i<COMMANDS.Length; i++)
            {
                if (COMMANDS[i].Equals(code, StringComparison.CurrentCultureIgnoreCase))
                {
                    cmd = i;
                    break;
                }
            }
            
            switch (cmd) {
                case ASCII:
                    command.Type('a');
                    break;

                case BINARY:
                    command.Type('b');
                    break;

                case CD:
                    if (args[1] != null)
                    {
                        command.Cd(args[1]);
                    }
                    else
                    {
                        Console.WriteLine("Please specify a path.");
                    }
                    break;

                case CDUP:
                    command.Cd("..");
                    break;

                case DEBUG:
                    Program.Debug("Debug mode deactivated.");
                    Program.ToggleDebug();
                    Program.Debug("Debug mode activated.");
                    break;

                case DIR:
                    command.Dir();
                    break;

                case GET:
                    if (args[1] != null)
                    {
                        command.Get(args[1]);
                    }
                    else
                    {
                        Console.WriteLine("Please specify a path.");
                    }
                    break;

                case HELP:
                    Console.WriteLine(HELP_MESSAGE);
                    break;

                case PASSIVE:
                    command.Passive();
                    break;

                case PWD:
                    command.Pwd();
                    break;

                case QUIT:
                    command.Quit();
                    Console.Write("Connection closed. Press any key to exit the program.");
                    Console.ReadKey();
                    return false;

                default:
                    Console.WriteLine("Invalid command");
                    break;
            }
            return true;

        }
    }
}
