var socket;
var uri = "ws://localhost:5000/ws";
var text = "test echo";
var user="";
var output;
var beepSound;

/**
* Called when the register button is clicked, displays register box
*/
function register() {
    $('#registerBox').css('display', 'inherit');
}

/**
* Called when cancel register is clicked, hides register box
*/
function cancelRegister() {
    $('#newUsername').val('');
    $('#newPassword').val('');
    $('#repeatPassword').val('');
    $('#registerError').text('');
    $('#registerBox').css('display', 'none');
}

/**
* Formats register information for sending over the websocket. Does some preliminary
* error checking.
*/
function submitRegister() {
    var username = $('#newUsername').val();
    var password = $('#newPassword').val();
    var repeat = $('#repeatPassword').val();
    
    if (username.length == 0 || password == 0) {
        registerError('Usernames and passwords may not be blank');
        return;
    }

    if (username.includes(' ') || password.includes(' ')) {
        registerError('Usernames and passwords may not have an empty space character');
        return;
    }

    if (password != repeat) {
        registerError('Password does not match');
        return;
    }

    sendCommand('Register', {
        "username": username,
        "date": new Date().toString(),
        "password": password
    })
}

/**
* Displays a registration error to the screen
*/
function registerError(msg) {
    $('#registerError').text(msg);
}

/**
* Handles a successful registration.
*/
function registerSuccess(username) {
    output.append('<p class="message"><b>Congratulations! You\'ve been registered with Flak as <i>' + username + '</i>!</b></p>');
    cancelRegister();
    loginSuccess(username);
}

/**
* Formats login information to send over the websocket.
*/
function submitLogin() {
    var name = $('#username').val();
    var pass = $('#password').val();

    sendCommand('Login', {
        "username": name,
        "password": pass,
        "date": new Date().toString()
    });
}

/**
* Displays a login error to the screen
*/
function loginError(msg) {
    $('#loginError').text(msg);
}

/**
* Handles a successful login
*/
function loginSuccess(username) {
    user = username;
    $('#loginLegend').text('User');
    $('#login').children().css('display', 'none');
    $('#loginLegend').css('display', 'inherit');
    $('#login').append('<p>You are Logged In as</br><b>' + user + '</b></p>');
    $('#textIn').prop('disabled', false);
    $('#textIn').val("");
    $('#sendButton').prop('disabled', false);
    retroactiveUsernameHighlight();
}

/**
* Highlights the user's username in any messages printed before logging in
*/
function retroactiveUsernameHighlight() {
    var messages = output.children();
    for (var i = 0; i < messages.length; i++) {
        var m = $(messages[i]).html();
        if (m.includes('@'+user)) {
            var newString ="";
            var newMessage = [];
            while (m.includes('@'+user)) {
                var at = m.indexOf('@'+user)
                newMessage.push(m.substr(0, at));
                m = m.substr(at, m.length);
                var length = user.length + 1;
                newMessage.push(m.substr(0, length));
                m = m.substr(length, m.length);
            }
            if (m.length != 0)
                newMessage.push(m);
            for (var j = 0; j < newMessage.length; j++) {
                if (newMessage[j].charAt(0) == '@') {
                    if (newMessage[j].substr(1, newMessage[j].length) == user)
                        newString = newString + '<i class="userTag">' + newMessage[j] + '</i>';
                    else
                        newString = newString + newMessage[j];
                }
                else {
                    newString = newString + newMessage[j];
                }
            }
            $(messages[i]).html(newString);
        }
    }
}

/**
* Displays a message to the screen in the output box, highlights and beeps if this user's username
* was tagged in it.
*/
function printMessage(s) {
    var date = new Date(s.date);
    var appendString = '<p class="message"><b>' + s.user + ': </b>';
    var message = [];
    var beep = false;
    while (s.message.includes('@'+user)) {
        var at = s.message.indexOf('@'+user)
        message.push(s.message.substr(0, at));
        s.message = s.message.substr(at, s.message.length);
        var length = user.length + 1;
        message.push(s.message.substr(0, length));
        s.message = s.message.substr(length, s.message.length);
    }
    if (s.message.length != 0)
        message.push(s.message);
    for (var i = 0; i < message.length; i++) {
        if (message[i].charAt(0) == '@') {
            if (user!=""&&message[i].substr(1, message[i].length) == user) {
                appendString = appendString + '<i class="userTag">' + message[i] + '</i>';
                beep = true;
            }
            else
                appendString = appendString + message[i];
        }
        else {
            appendString = appendString + message[i];
        }
    }
    appendString=appendString+' <i class="date">'+date.toLocaleString()+'</i></p>';
    output.append(appendString);
    output[0].scrollTop = output[0].scrollHeight;

    if (beep) {
        beepSound.play();
    }
}

/**
* Displays a message about a user logging in
*/
function loginNote(mes) {
    var date = new Date(mes.date);
    output.append('<p class="message"><b>Heads Up! <i>' + mes.message + '</i> just logged into Flak!</b> <i class="date">' + date.toLocaleString() + '</i></p>');
}

/**
* Displays a message about a user exiting out
*/
function printClose(s) {
    if (s.user == null) {
        return;
    }
    var date = new Date(s.date);
    output.append('<p class="message"><b>' + s.user + ' has left Flak. </b><i class="date">' + date.toLocaleString() + '</i></p>');
}

/**
* Parses incoming messages of the websocket and performs the proper tasks.
*/
function handleMessage(messageString) {
    var mes = JSON.parse(messageString);
    switch(mes.type){
        case "Message":
            printMessage(mes);
            break;
        case "Close":
            printClose(mes);
            break;
        case "RegisterSuccess":
            registerSuccess(mes.message);
            break;
        case "RegisterFail":
            registerError(mes.message);
            break;
        case "LoginFail":
            loginError(mes.message);
            break;
        case "LoginSuccess":
            loginSuccess(mes.message);
            break;
        case "LoginNote":
            loginNote(mes);
            break;
    }
}

/**
* Connects to the websocket
*/
function doConnect() {
    socket = new WebSocket(uri);
    socket.onopen = function (e) {};
    socket.onclose = function (e) { sendClose() };
    socket.onmessage = function (e) {
        handleMessage(e.data);
    };
    socket.onerror = function (e) { sendClose() };
}

/**
* Formats information to inform the websocket about the browser closing
*/
function sendClose() {
    sendCommand("Close", {
        "user": user,
        "date": new Date().toString(),
        "type": "Close"
    });
}

/**
* The function called upon initialization.
*/
function onInit() {
    output = $('#output');
    beepSound = $('#sound1')[0];
    doConnect();
}

/**
* Formats a message to be sent over the web socket
*/
function sendMessage() {
    var myMessage=$('#textIn').val();
    $('#textIn').val("");

    sendCommand("Message", {
        "message": myMessage,
        "user": user,
        "date": new Date().toString(),
        "type": "Message"
    });
}

/**
* Packages and sends data over the web socket
*/
function sendCommand(type, data) {
    var cmd = {
        "command": type,
        "message": JSON.stringify(data)
    }
    socket.send(JSON.stringify(cmd));
}

/**
* Detects when the user presses the enter key while typing a message, and sends the message.
*/
function textInKeyUp(event) {
    if (event.keyCode == 13)
        sendMessage();
}

window.onload = onInit;
window.onunload = function () {
    sendClose();
}