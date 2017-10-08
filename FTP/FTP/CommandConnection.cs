using System;
using System.Net.Sockets;
using System.IO;

namespace FTP
{
    /// <summary>
    /// <author>Author: John Byrne (jpb2380)</author>
    /// Represents a command connection with an FTP server.
    /// </summary>
    class CommandConnection
    {
        private TcpClient client;
        private StreamReader reader;
        private StreamWriter writer;
        private bool passive;

        private const int BADUSERNAME= 331;
        private const int TIMEOUT = 441;
        private const int FILENOTFOUND = 550;

        /// <summary>
        /// Establishes a new command connection at the server at
        /// address and the port at port, and manages logging in
        /// to that FTP Server.
        /// </summary>
        /// <param name="address">The server to connect to</param>
        /// <param name="port">The port on the server to connect to.</param>
        public CommandConnection(string address, int port)
        {
            try
            {
                client = new TcpClient(address, port);
                reader = new StreamReader(client.GetStream());
                readResponse();
                writer = new StreamWriter(client.GetStream());
                int code = 0;
                while (code != BADUSERNAME) {
                    Console.Write("Username: ");
                    writer.WriteLine("USER " + Console.ReadLine());
                    writer.Flush();
                    Program.Debug("Sending USER message.");
                    code = readResponse();
                }
                Console.Write("Password: ");
                writer.WriteLine("PASS " + Console.ReadLine());
                writer.Flush();
                Program.Debug("Sending PASS message.");
                readResponse();
            }
            catch (Exception e)
            {
                Console.WriteLine("There seems to have been an error connecting to the server you provided.");
                Console.WriteLine("Please check that it is a valid FTP server before trying again.");
                Console.WriteLine("The program will now close. Press any key to exit.");
                Console.ReadKey();
                Environment.Exit(1);
            }

            passive = false;
        }

        /// <summary>
        /// Reads a response from the server. Also detects a server timeout.
        /// </summary>
        /// <returns>The response code recieved from the server</returns>
        public int readResponse()
        {
            var line = reader.ReadLine();
            var codeS = line.Split(' ')[0];
            int code;
            while(!Int32.TryParse(codeS, out code))
            {
                Console.WriteLine(line);
                line = reader.ReadLine();
                codeS = line.Split(' ')[0];
            }
            Console.WriteLine(line);
            if(code == TIMEOUT)
            {
                Timeout();
            }
            return code;
        }

        /// <summary>
        /// Sends a request to the surver to change the method of file
        /// transmission.
        /// </summary>
        /// <param name="type">The type of transmission to switch to. 'a' for 
        /// ascii, 'b' for binary</param>
        public void Type(char type)
        {
            char code;
            if (type == 'a')
                code = 'A';
            else code = 'I';
            Program.Debug("Sending TYPE message with parameter \"" + code + "\".");
            writer.WriteLine("TYPE "+code);
            writer.Flush();
            readResponse();
        }

        /// <summary>
        /// Sends a directory change request to the server.
        /// </summary>
        /// <param name="path">The path of the directory to change to.</param>
        public void Cd(string path)
        {
            if (path.Equals(".."))
            {
                Program.Debug("Sending CDUP message.");
                writer.WriteLine("CDUP");
            }
            else
            {
                Program.Debug("Sending CWD message with path \"" + path + "\".");
                writer.WriteLine("CWD " + path);
            }
            writer.Flush();
            readResponse();
        }

        /// <summary>
        /// Requests a directory listing from server.
        /// </summary>
        public void Dir()
        {
            var data = DataConnectionFactory.makeConnection(passive, writer, reader);
            Program.Debug("Sending LIST message.");
            writer.WriteLine("LIST");
            writer.Flush();
            readResponse();
            data.readDirectory();
            readResponse();
        }

        /// <summary>
        /// Requests a file transfer from the server.
        /// </summary>
        /// <param name="path">The path of the file to be retrieved.</param>
        public void Get(string path)
        {
            var data = DataConnectionFactory.makeConnection(passive, writer, reader);
            Program.Debug("Sending RETR message with path \"" + path + "\".");
            writer.WriteLine("RETR "+path);
            writer.Flush();
            int code = readResponse();
            if (code == FILENOTFOUND)
                return;
            data.readFile(path);
            readResponse();
        }

        /// <summary>
        /// Toggles whether or not the client requests data with PASV or PORT.
        /// </summary>
        public void Passive()
        {
            passive = !passive;
            Console.Write("Passive Mode toggled ");
            if (passive)
                Console.WriteLine("ON.");
            else
                Console.WriteLine("OFF.");
        }

        /// <summary>
        /// Requests the name of the working directory from the server.
        /// </summary>
        public void Pwd()
        {
            Program.Debug("Sending PWD message.");
            writer.WriteLine("PWD");
            writer.Flush();
            readResponse();
        }

        /// <summary>
        /// Tells the server to terminate the connection.
        /// </summary>
        public void Quit()
        {
            Program.Debug("Sending QUIT message.");
            writer.WriteLine("QUIT");
            writer.Flush();
            readResponse();
        }

        /// <summary>
        /// Informs the user of a server timeout and exits the program.
        /// </summary>
        public void Timeout()
        {
            Console.Write("Your connection timed out. Press any key to exit the program.");
            Console.ReadKey();
            Environment.Exit(0);
        }
    }
}
