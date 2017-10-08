FTP Project readme.txt
John Byrne (jpb2380)

The solution contains an FTP project and and FTPServer project.
FTP contains the client, FTPServer contains the server.

They were created using the .Net Framework.

Running FTP and FTPServer
	Both projects are built and available in their project directory bin/Debug
	folder. They DO NOT have project.json files and CAN NOT run via a 
	"dotnet run" command. Visual Studio did not automatically generate a
	project.json file when the project was created, and I wasn't sure how to do
	that manually.

File Listing:
	FTP
		CommandConnection.cs - Handles command connection logic.
		ConsoleReader.cs - Handles reading input from the console, figuring out
		which commands to run.
		DataConnection.cs - Handles reading off data connections.
		DataConnectionFactory.cs - Handles creating data connections.
		Program.cs - The entry point. Handles startup and the debug printing.
	FTPServer
		Connection.cs - Handles the connection logic, sending responses and
		data
		Program.cs - Entry point, starts the server.
		TCPServer.cs - Handles server logic and accepting clients.

Known Defects:
	It will occasionally crash from connection errors. It catches timeouts, but
	sometimes if the server is closed it will blow up the client too.

Other Files:
	In the FTPServer/bin/Debug folder two text files (test_file.txt and 
	nuclear_codes.secrets.txt) have been provided for testing retrieving files
	off of the server.