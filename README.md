# school_projects
This is a repository of several school projects I have completed, as example of my work. The projects have been uploaded as they were when I completed them and turned them in for grading. If the project was created as a part of a team I have received permission from my team members to upload the project, and I will identify here that the project was a group project.

This is not a complete list of all projects I have done, and I will be adding more as I get permission from teammates.

## Project Listing
1. FTP - A limited functionality FTP client and server.


## FTP
This project was created for my Data Communications and Networks class. The goal was to make a FTP client and server to learn about networking protocols. I did the project in .NET Framework using Visual Studio 2015. The entire solution is provided.

### Scope of the Project
The FTP server and client both run from the command line, neither has a GUI. The FTP server and client were not meant to be fully functional.
The server supports both port and passive connections, and will list the working directory and retrieve files. Directory navigation is not supported. The server is also hardcoded to only accept usernames "ftp" and "anonymous" and will accept any password. For simplicity's sake we were required to have our server always open on port 2121.
The client runs with two command line arguments: the server to connect to and (optionally) the port to connect on. If the port is not specified the client will default to port 21. The client can handle port and passive data connection, file retrieval and directory listing, and directory navigation. The commands for the program are accessible by typing help once logged in.
The project was not as much about building a stable peice of software as it was about learning the network protocols, so the project is a bit buggy. We were told not to focus on error handling, so there are cases where network and stream io exceptions are not caught.

### Running the Project
Instructions on running the project can be found within the projects own readme.txt
