namespace FTPServer
{
    /// <summary>
    /// <author>John Byrne (jpb2380)</author>
    /// Entry point for the FTPServer program.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Starts the server.
        /// </summary>
        /// <param name="args">No commmand line arguments</param>
        static void Main(string[] args)
        {
            TCPServer.run();
        }
    }
}
