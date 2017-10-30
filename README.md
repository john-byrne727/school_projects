# school_projects
This is a repository of several school projects I have completed, as example of my work. The projects have been uploaded as they were when I completed them and turned them in for grading. If the project was created as a part of a team I have received permission from my team members to upload the project, and I will identify here that the project was a group project.

This is not a complete list of all projects I have done, and I will be adding more as I get permission from teammates.

## Project Listing
1. FTP - A limited functionality FTP client and server.
2. Flak - A websocket based messenger system.


## FTP
This project was created for my Data Communications and Networks class. The goal was to make a FTP client and server to learn about networking protocols. I did the project in .NET Framework using Visual Studio 2015. The entire solution is provided.

### Scope of the Project
The FTP server and client both run from the command line, neither has a GUI. The FTP server and client were not meant to be fully functional.
The server supports both port and passive connections, and will list the working directory and retrieve files. Directory navigation is not supported. The server is also hardcoded to only accept usernames "ftp" and "anonymous" and will accept any password. For simplicity's sake we were required to have our server always open on port 2121.
The client runs with two command line arguments: the server to connect to and (optionally) the port to connect on. If the port is not specified the client will default to port 21. The client can handle port and passive data connection, file retrieval and directory listing, and directory navigation. The commands for the program are accessible by typing help once logged in.
The project was not as much about building a stable peice of software as it was about learning the network protocols, so the project is a bit buggy. We were told not to focus on error handling, so there are cases where network and stream io exceptions are not caught.

### Running the Project
Instructions on running the project can be found within the projects own readme.txt

## Flak
This project was created for my Concept of Parallel and Distributed Systems class. The goal was to explore the functionality of websockets by making a messenger system. I did the project using .NET Core 1.1 using Visual Studio 2015. The entiere solution is provided.

### Scope of the Project
The project's system includes both a c# backend and HTML/JavaScript/CSS frontend. The project is run by starting the backend server and connecting to it via a browser. Functionality is best observed using more than one browser.
The primary function of the system is to be an instant messenger chat room. Messages sent from one client are meant to be reach the other client with minimal delay.
The basic project requirements included notifications of user's joining and leaving the chat room, the ability to "tag" users by putting the '@' symbol before their alias in a message, and an audible notification upon a user being tagged.
There were several additional optional requirements available for more credit on the project. I chose to implement persistent messages and a user account system requiring password authentication.
The project was meant to be a proof of concept more than an actually working project, and as such it's functionality is limited. The system was not designed or tested for activity over a network, it was only designed for activity over local ports.
Since it was not meant for use over networks, it does not have many security features. Persistent data other than passwords are stored in plaintext.
The project also stores persistent data in plaintext rather than a database. This is because the development time for the project was only a few weeks, and we were instructed by the professor to focus more on the websocket aspect of the project than the detailed design. This is also why much of the default files and libraries created by dotnet remain in the project even though they are unused.

### Running the Project
The project can be executed by using the dotnet run command from it's root folder. The client is accessed via the port provided. To use the system new users can be registered, or two users listed in the project's internal readme.txt can be accessed with the given username and password.