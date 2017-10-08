using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace FTPServer
{
    /// <summary>
    /// <author>John Byrne (jpb2380)</author>
    /// Represents a connection with a client.
    /// </summary>
    class Connection
    {
        private TcpClient client;
        private TcpClient data;
        private StreamWriter dataWriter;
        private StreamWriter writer;
        private StreamReader reader;
        private string clientID;
        private static int id = 1;
        private bool passive;
        private string portIP;
        private int portPort;

        public static readonly string[] COMMANDS = {
                          "LIST",
                          "RETR",
                          "PASV",
                          "PORT",
                          "QUIT",
                          "FAIL"};

        
        public const int LIST = 0;
        public const int RETR = 1;
        public const int PASV = 2;
        public const int PORT = 3;
        public const int QUIT = 4;
        public const int FAIL = 5;

        /// <summary>
        /// Creates the connection object and sends the welcom message.
        /// </summary>
        /// <param name="client">The TcpClient of the connection.</param>
        public Connection(TcpClient client)
        {
            this.client = client;
            clientID = "Client " + id + ": ";
            id++;
            writer = new StreamWriter(client.GetStream());
            reader = new StreamReader(client.GetStream());
            Console.WriteLine(clientID + "Connected");
            writer.WriteLine("220- Welcome to John Byrne's FTP server program!\n" +
                                "220-                           _______\n" +
                                "220-                          | ___  o|\n" +
                                "220-                          |[_-_]_ |\n" +
                                "220-       ______________     |[_____]|\n" +
                                "220-      |.------------.|    |[_____]|\n" +
                                "220-      ||            ||    |[====o]|\n" +
                                "220-      ||    FTP     ||    |[_.--_]|\n" +
                                "220-      ||    FTP     ||    |[_____]|\n" +
                                "220-      ||    FTP     ||    |      :|\n" +
                                "220-      ||____________||    |      :|\n" +
                                "220-  .==.|\"\"  ......    |.==.|      :|\n" +
                                "220-  |::| '-.________.-' |::||      :|\n" +
                                "220-  |''|  (__________)-.|''||______:|\n" +
                                "220-  `\"\"`_.............._\\\"\"`______\n" +
                                "220-     /:::::::::::'':::\\`;'-.-.  `\\\n" +
                                "220-    /::=========.:.-::\"\\ \\ \\--\\   \\\n" +
                                "220-    \\`\"\"\"\"\"\"\"\"\"\"\"\"\"\"\"\"`/  \\ \\__)   \\\n" +
                                "220-     `\"\"\"\"\"\"\"\"\"\"\"\"\"\"\"\"`    '========'");

            writer.WriteLine("220  Please log in before proceeding.");
            writer.Flush();
            passive = false;
        }

        /// <summary>
        /// Runs the server logic for this particular client connection. Starts
        /// with user authentication then loops on normal execution.
        /// </summary>
        public void run()
        {
            var userRecieved = false;
            while (!userRecieved)
            {
                var username = readCommand();
                if (username[0] != "USER")
                {
                    writer.WriteLine("530 Please log in.");
                    writer.Flush();
                    continue;
                }
                if (username[1].Equals("ftp", StringComparison.CurrentCultureIgnoreCase) ||
                    username[1].Equals("anonymous", StringComparison.CurrentCultureIgnoreCase))
                {
                    writer.WriteLine("331 Username is good, any password will work");
                    writer.Flush();
                }
                else
                {
                    writer.WriteLine("430- This server only allows anonymous connections.");
                    writer.WriteLine("430  Please use username \"anonymous\" or\"ftp\".");
                    writer.Flush();
                    continue;
                }

                var password = readCommand();
                if (password[0] == "PASS")
                {
                    userRecieved = true;
                    writer.WriteLine("230 Log in successful!");
                    writer.Flush();
                }
                else
                {
                    writer.WriteLine("530 Please Log In. Password is required.");
                    writer.Flush();
                }
            }

            Console.WriteLine(clientID + "Client logged in.");
            while (CommandLoop()) ;
            client.Close();
            Console.WriteLine(clientID + "Client closed.");
        }

        /// <summary>
        /// Processes commands from the client, and sends the proper response.
        /// </summary>
        /// <returns>True if the client remains connected, false if not</returns>
        private bool CommandLoop()
        {
            var cmd = readCommand();
            int code=-1;
            for (int i = 0; i < COMMANDS.Length; i++)
            {
                if (COMMANDS[i].Equals(cmd[0], StringComparison.CurrentCultureIgnoreCase))
                {
                    code = i;
                    break;
                }
            }

            switch (code)
            {
                case LIST:
                    if (!passive)
                    {
                        data = new TcpClient(portIP, portPort);
                        dataWriter = new StreamWriter(data.GetStream());
                    }
                    Console.WriteLine(clientID + "Client Requested directory listing.");
                    writer.WriteLine("150 Directory Listing incoming.");
                    writer.Flush();
                    var dirs = Directory.EnumerateDirectories(".");
                    
                    foreach(var entry in dirs)
                    {
                        var dir = new DirectoryInfo(entry);
                        dataWriter.WriteLine("Directory: "+dir.Name);
                    }
                    var wd = new DirectoryInfo(".");
                    var files = wd.GetFiles();
                    foreach(var file in files)
                    {
                        dataWriter.WriteLine("File:      " + file.Name);
                    }

                    dataWriter.Flush();
                    dataWriter.Close();
                    data.Close();
                    writer.WriteLine("226 Directory send successful");
                    writer.Flush();
                    break;
                case RETR:
                    if (!passive)
                    {
                        data = new TcpClient(portIP, portPort);
                        dataWriter = new StreamWriter(data.GetStream());
                    }
                    try
                    {

                        Console.WriteLine(clientID + "Client Requested File "+cmd[1]+".");
                        var fileReader = new StreamReader(File.Open(cmd[1], FileMode.Open));
                        writer.WriteLine("150 Beginning Binary data transfer for " + cmd[1]);
                        writer.Flush();
                        while (!fileReader.EndOfStream)
                        {
                            dataWriter.WriteLine(fileReader.ReadLine());
                        }
                        dataWriter.Flush();
                        dataWriter.Close();
                        data.Close();
                        writer.WriteLine("226 Transfer successful.");
                        writer.Flush();
                        fileReader.Close();
                    }
                    catch (IOException e)
                    {
                        Console.WriteLine(clientID + "Client attempeted to retrieve file that did not exist"+
                            "or could not be opened \""+cmd[1]+"\"");
                        writer.WriteLine("550 Failed to open file.");
                        writer.Flush();
                    }
                    break;
                case PASV:
                    var addresses = Dns.GetHostAddresses(Dns.GetHostName());
                    var myIP = "";
                    foreach (var addr in addresses)
                    {
                        if (addr.AddressFamily == AddressFamily.InterNetwork)
                        {
                            myIP = addr.ToString();
                        }
                    }

                    var pasvListener = new TcpListener(IPAddress.Any, 0);
                    pasvListener.Start();
                    var pasv = ((IPEndPoint)pasvListener.LocalEndpoint).Port;
                    var pasvString = "," + (pasv / 256) + "," + (pasv % 256);

                    myIP = myIP.Replace('.', ',') + pasvString;

                    writer.WriteLine("227 Entering Passive Mode (" + myIP + ").");
                    writer.Flush();

                    data=pasvListener.AcceptTcpClient();
                    dataWriter = new StreamWriter(data.GetStream());
                    passive = true;

                    Console.WriteLine(clientID + "Client connecting via PASV.");
                    break;
                case PORT:
                    var addrArray = cmd[1].Split(',');
                    portIP = addrArray[0] + '.' + addrArray[1] + '.' + addrArray[2] + '.' + addrArray[3];
                    portPort = (256 * Int32.Parse(addrArray[4])) + Int32.Parse(addrArray[5]);
                    passive = false;
                    writer.WriteLine("200 PORT command successful. consider using PASV.");
                    writer.Flush();

                    Console.WriteLine(clientID + "Client connecting via PORT.");
                    break;
                case QUIT:
                    Console.WriteLine(clientID + "Client closing.");
                    writer.WriteLine("221 Bye Bye.");
                    writer.Flush();
                    return false;
                case FAIL:
                    return false;
                default:
                    Console.WriteLine(clientID + " Client sent invalid command.");
                    writer.WriteLine("200 Command not supported.");
                    writer.Flush();
                    break;
            }

            return true;
        }

        /// <summary>
        /// Reads messages off the client's stream.
        /// </summary>
        /// <returns>The message read.</returns>
        private string[] readCommand()
        {
            try
            {
                var cmd = reader.ReadLine();
                if (cmd == null)
                {
                    var args = new string[1];
                    args[0] = "junk";
                    return args;
                }
                return cmd.Split(' ');
            }
            catch(IOException e)
            {
                Console.WriteLine(clientID+"There was an unexpected IO error from the client");
                client.Close();
                var args = new string[1];
                args[0] = "fail";
                return args;
            }
        }
    }
}
