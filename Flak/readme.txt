  ______ _       _    
 |  ____| |     | |   
 | |__  | | __ _| | __
 |  __| | |/ _` | |/ /
 | |    | | (_| |   < 
 |_|    |_|\__,_|_|\_\

Flak readme.txt
John Byrne (jpb2380)

1. Requirements I Tried to Meet
    a. All the base requirements
	b. Persistent Messages
	c. User Accounts
2. Running Flak
    a. Start the server with dotnet run
	b. The client will be available on the localhost the server tells you at
	   the url /index.html (for example "localhost:5000/index.html").
3. Pre-loaded Accounts
	a. Username: hopeful_student
	   Password: apple
	b. Username: professor_awesome
	   Password: apple
4. Source Files I Used:
    	a. Startup.cs
	       Sets up the server, calls function to read in existing user
		   information.
	b. SocketHandler.cs
	       Handles the websockets serverside, does logic on the models
	c. Models/FlakData.cs
	       Top level model containing list of users. Handles certain system
		   logic that didn't fit within other models, such as file reading.
	d. Models/User.cs
	        Contains user information, handles password hashing and
			verification.
	e. Models/FlakCommand.cs, Models/FlakLogin.cs, Models/FlakResponse.cs
	        These classes contain fields corresponding to the JSON objects sent
			over the websocket. They are used in serialization and
			deserialization.
	f. wwwroot/index.html
	        The HTML of the client.
	g. wwwroot/js/site.js
	        The javascript handling the client side logic and handling the
			client side of the web socket.
	h. wwwroot/css/site.css
	        The stylesheet for the client.
5. Known Issues
	a. Refreshing the page can make the accounts do wonky things, like not
	   properly informing the server of the refresh. As far as I can tell this
	   doesn't break anything, but there won't be a "user left" message in the
	   log.