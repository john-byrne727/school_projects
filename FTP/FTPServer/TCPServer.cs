using System;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace FTPServer
{
    /// <summary>
    /// <author>John Byrne (jpb2380)</author>
    /// Handles accepting clients to the server.
    /// </summary>
    class TCPServer
    {
        private static AutoResetEvent connectionWaitHandle = new AutoResetEvent(false);

        /// <summary>
        /// The main loop for accepting clients.
        /// </summary>
        public static void run()
        {
            TcpListener listener = new TcpListener(IPAddress.Any, 2121);
            listener.Start();
            Console.WriteLine("Starting Server");
            while (true)
            {
                listener.BeginAcceptTcpClient(DoAcceptTcpClientCallback, listener);
                connectionWaitHandle.WaitOne();
            }
        }

        /// <summary>
        /// Asychronous client acceptance logic. Creates and runs a new
        /// connection object.
        /// </summary>
        /// <param name="ar">The async result.</param>
        public static void DoAcceptTcpClientCallback(IAsyncResult ar)
        {
            TcpListener listener = (TcpListener)ar.AsyncState;
            TcpClient client = listener.EndAcceptTcpClient(ar);
            connectionWaitHandle.Set();
            var cnct = new Connection(client);
            cnct.run();
        }
    }
}
