using System;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace FTP
{
    /// <summary>
    /// <author>John Byrne (jpb2380)</author>
    /// A factory class that handles creation logic for DataConnection objects,
    /// as well as sending and recieving proper information about the
    /// connections to and from the server.
    /// </summary>
    class DataConnectionFactory
    {
        /// <summary>
        /// Makes an new DataConnection object.
        /// </summary>
        /// <param name="passive">True if the data connection should be 
        /// passive, false if port.</param>
        /// <param name="writer">The command stream writer.</param>
        /// <param name="reader">The command stream reader.</param>
        /// <returns>The new DataConnection object.</returns>
        public static DataConnection makeConnection(bool passive, StreamWriter writer, StreamReader reader)
        {
            DataConnection data;
            string line;
            if (passive)
            {
                Program.Debug("Sending PASV message.");
                writer.WriteLine("PASV");
                writer.Flush();
                line = reader.ReadLine();
                data = makePasvConnection(line);
            }
            else
            {

                data = makePortConnection(writer);
                line = reader.ReadLine();
            }
            Console.WriteLine(line);
            return data;
        }

        /// <summary>
        /// Creates a new PASV DataConnection object. Parses the IP and port
        /// information and creates the client.
        /// </summary>
        /// <param name="line">The PASV response message with the IP and port
        /// information.</param>
        /// <returns>The new DataConnection object.</returns>
        private static DataConnection makePasvConnection(String line)
        {
            var openParen = line.IndexOf('(');
            var closeParen = line.IndexOf(')');
            var len = closeParen - openParen;
            var addr = line.Substring(openParen + 1, len - 1);
            var addrArray = addr.Split(',');
            var ip = addrArray[0] + '.' + addrArray[1] + '.' + addrArray[2] + '.' + addrArray[3];
            int port = (256 * Int32.Parse(addrArray[4])) + Int32.Parse(addrArray[5]);
            Program.Debug("Attempting to create a passive connection on IP: " + ip + " Port: " + port+".");
            var client = new TcpClient(ip, port);
            
            return new DataConnection(client);
        }

        /// <summary>
        /// Creates a new PORT DataConnection object. Starts a TcpListener and
        /// sends proper IP and port information to the server. 
        /// </summary>
        /// <param name="writer">The command stream writer</param>
        /// <returns>The new DataConnection object.</returns>
        private static DataConnection makePortConnection(StreamWriter writer)
        {
            var addresses = Dns.GetHostAddresses(Dns.GetHostName());
            var myIP = "";
            foreach (var addr in addresses)
            {
                if (addr.AddressFamily == AddressFamily.InterNetwork)
                {
                    myIP = addr.ToString();
                }
            }

            var portListener = new TcpListener(IPAddress.Any, 0);
            portListener.Start();
            var port =((IPEndPoint) portListener.LocalEndpoint).Port;
            var portString = "," + (port / 256) + "," + (port % 256);

            myIP = myIP.Replace('.', ',') + portString;


            Program.Debug("Sending PORT message with local endpoint \"" + myIP + "\", (Port=" + port + ").");
            writer.WriteLine("PORT " + myIP);
            writer.Flush();
            
            return new DataConnection(portListener);
        }
    }
}
