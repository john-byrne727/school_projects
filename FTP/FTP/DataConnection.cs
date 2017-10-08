using System;
using System.Net.Sockets;
using System.IO;

namespace FTP
{
    /// <summary>
    /// <author>John Byrne (jpb2380)</author>
    /// Represents a data connection, either port or passive.
    /// </summary>
    class DataConnection
    {
        private TcpClient client;
        private TcpListener listener;
        private StreamReader reader;

        /// <summary>
        /// Creates a new passive data connection.
        /// </summary>
        /// <param name="client">The TcpClient the data connection will handle.
        /// </param>
        public DataConnection(TcpClient client)
        {
            this.client = client;
            reader = new StreamReader(client.GetStream());
            Program.Debug("Made a data connection");
        }


        /// <summary>
        /// Creates a new port data connection.
        /// </summary>
        /// <param name="listener">The TcpListener the port connection will be
        /// accepted on.</param>
        public DataConnection(TcpListener listener)
        {
            this.listener = listener;
        }

        /// <summary>
        /// Reads a directory listing in from the connection.
        /// </summary>
        public void readDirectory()
        {
            if (client == null)
            {
                portConnect();
            }
            Program.Debug("Starting to read off data connection.");
            while (!reader.EndOfStream)
            {
                Console.WriteLine(reader.ReadLine());
            }
            Program.Debug("Done reading off data connnection.");
        }

        /// <summary>
        /// Reads a file from the connection and saves it locally.
        /// </summary>
        /// <param name="fileName">The name of the file.</param>
        public void readFile(string fileName)
        {
            if (client == null)
            {
                portConnect();
            }
            StreamWriter file;
            try
            {
                file= new StreamWriter(File.Open(fileName, FileMode.Create));
            }
            catch(Exception e)
            {
                Console.WriteLine("ERROR: The file" + fileName + " could not be created locally.");
                return;
            }
            Program.Debug("Starting to read off data connection.");
            while (!reader.EndOfStream)
            {
                file.WriteLine(reader.ReadLine());
            }
            file.Close();
            Program.Debug("Done reading off data connnection, file created.");
        }

        /// <summary>
        /// Accepts the port connection from the listener.
        /// </summary>
        private void portConnect()
        {
            Program.Debug("Accepting Port TCP Connection");
            client = listener.AcceptTcpClient();
            listener.Stop();
            reader = new StreamReader(client.GetStream());
            Program.Debug("Made a Port Connection.");
        }
    }
}
