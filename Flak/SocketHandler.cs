using System;
using Microsoft.AspNetCore.Builder;
using System.Threading;
using System.Threading.Tasks;
using System.Net.WebSockets;
using System.Globalization;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WebApplication.Data;
using WebApplication.Models;
using WebApplication.Services;

namespace WebApplication
{
    public class SocketHandler
    {
        int BufferSize = 4096;

        static List<SocketHandler> sockets = new List<SocketHandler>();

        WebSocket socket;

        /// <summary>
        /// Constructor for the web socket handler
        /// </summary>
        /// <param name="socket">The socket to handle</param>
        SocketHandler(WebSocket socket)
        {
            this.socket = socket;
        }

        /// <summary>
        /// Recieves commands and performs that command.
        /// </summary>
        /// <returns></returns>
        async Task commandReceiver()
        {

            while (this.socket.State == WebSocketState.Open)
            {
                var buffer = new byte[BufferSize];
                var seg = new ArraySegment<byte>(buffer);
                var incoming = await this.socket.ReceiveAsync(seg, CancellationToken.None);

                var inString = System.Text.Encoding.ASCII.GetString(seg.Array);

                var cmd = Newtonsoft.Json.JsonConvert.DeserializeObject<FlakCommand>(inString);
                byte[] outbuffer;
                ArraySegment<byte> outgoing;
                FlakLogin loginInfo;
                if (cmd == null)
                    continue;
                switch (cmd.command)
                {
                    case "Message":
                        //TODO: Write to the file to persist messages
                        FlakData.writeMessageToPersist(cmd.message);
                        outbuffer = System.Text.Encoding.ASCII.GetBytes(cmd.message);
                        outgoing = new ArraySegment<byte>(outbuffer, 0, outbuffer.Length);
                        foreach (SocketHandler s in sockets)
                        {
                            if (s.socket != null && s.socket.State == WebSocketState.Open)
                                await s.sendMessage(outgoing);
                        }
                        break;
                    case "Close":
                        await this.socket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                        FlakData.writeMessageToPersist(cmd.message);
                        outbuffer = System.Text.Encoding.ASCII.GetBytes(cmd.message);
                        outgoing = new ArraySegment<byte>(outbuffer, 0, outbuffer.Length);
                        foreach (SocketHandler s in sockets)
                        {
                            if (s.socket != null && s.socket.State == WebSocketState.Open)
                                await s.sendMessage(outgoing);
                        }
                        break;
                    case "Register":
                        loginInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<FlakLogin>(cmd.message);
                        if (FlakData.checkUsername(loginInfo.username))
                        {
                            FlakData.makeNewUser(loginInfo.username, loginInfo.password);
                            var response = new FlakResponse("RegisterSuccess", loginInfo.username, null);
                            var responseString = Newtonsoft.Json.JsonConvert.SerializeObject(response);
                            outbuffer = System.Text.Encoding.ASCII.GetBytes(responseString);
                            outgoing = new ArraySegment<byte>(outbuffer, 0, outbuffer.Length);
                            await this.sendMessage(outgoing);

                            var loginNotification = new FlakResponse("LoginNote", loginInfo.username, loginInfo.date);
                            var loginString = Newtonsoft.Json.JsonConvert.SerializeObject(loginNotification);
                            FlakData.writeMessageToPersist(loginString);
                            outbuffer = System.Text.Encoding.ASCII.GetBytes(loginString);
                            outgoing = new ArraySegment<byte>(outbuffer, 0, outbuffer.Length);
                            foreach (SocketHandler s in sockets)
                            {
                                if (s.socket != null && s.socket.State == WebSocketState.Open)
                                    await s.sendMessage(outgoing);
                            }
                        }
                        else
                        {
                            var response = new FlakResponse("RegisterFail", "That Username is already taken", null);
                            var responseString = Newtonsoft.Json.JsonConvert.SerializeObject(response);
                            outbuffer = System.Text.Encoding.ASCII.GetBytes(responseString);
                            outgoing = new ArraySegment<byte>(outbuffer, 0, outbuffer.Length);
                            await this.sendMessage(outgoing);
                        }
                        break;
                    case "Login":
                        loginInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<FlakLogin>(cmd.message);
                        var user = FlakData.getUser(loginInfo.username);
                        if (user!=null)
                        {
                            if (user.verifyPassword(loginInfo.password, user.getSalt()))
                            {
                                var response = new FlakResponse("LoginSuccess", loginInfo.username, null);
                                var responseString = Newtonsoft.Json.JsonConvert.SerializeObject(response);
                                outbuffer = System.Text.Encoding.ASCII.GetBytes(responseString);
                                outgoing = new ArraySegment<byte>(outbuffer, 0, outbuffer.Length);
                                await this.sendMessage(outgoing);

                                var loginNotification = new FlakResponse("LoginNote", loginInfo.username, loginInfo.date);
                                var loginString = Newtonsoft.Json.JsonConvert.SerializeObject(loginNotification);
                                FlakData.writeMessageToPersist(loginString);
                                outbuffer = System.Text.Encoding.ASCII.GetBytes(loginString);
                                outgoing = new ArraySegment<byte>(outbuffer, 0, outbuffer.Length);
                                foreach (SocketHandler s in sockets)
                                {
                                    if (s.socket != null && s.socket.State == WebSocketState.Open)
                                        await s.sendMessage(outgoing);
                                }
                            }
                            else
                            {
                                var response = new FlakResponse("LoginFail", "Incorrect Password", null);
                                var responseString = Newtonsoft.Json.JsonConvert.SerializeObject(response);
                                outbuffer = System.Text.Encoding.ASCII.GetBytes(responseString);
                                outgoing = new ArraySegment<byte>(outbuffer, 0, outbuffer.Length);
                                await this.sendMessage(outgoing);
                            }
                        }
                        else
                        {
                            var response = new FlakResponse("LoginFail", "Username not found", null);
                            var responseString = Newtonsoft.Json.JsonConvert.SerializeObject(response);
                            outbuffer = System.Text.Encoding.ASCII.GetBytes(responseString);
                            outgoing = new ArraySegment<byte>(outbuffer, 0, outbuffer.Length);
                            await this.sendMessage(outgoing);
                        }
                        break;


                }
            }
        }

        /// <summary>
        /// Sends a message over the web socket to the view
        /// </summary>
        /// <param name="seg">The byte array to send</param>
        /// <returns></returns>
        async Task sendMessage(ArraySegment<byte> seg)
        {
            await this.socket.SendAsync(seg, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        /// <summary>
        /// Accepts the web socket connection and sends the view the persistant messages.
        /// </summary>
        /// <param name="hc"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        static async Task Acceptor(HttpContext hc, Func<Task> n)
        {
            if (!hc.WebSockets.IsWebSocketRequest)
                return;

            var socket = await hc.WebSockets.AcceptWebSocketAsync();
            var h = new SocketHandler(socket);
            sockets.Add(h);
            var messages = FlakData.getPersistentMessages();
            if (messages != null)
            {
                foreach (String s in messages)
                {
                    var outbuffer = System.Text.Encoding.ASCII.GetBytes(s);
                    var outgoing = new ArraySegment<byte>(outbuffer, 0, outbuffer.Length);
                    await h.sendMessage(outgoing);
                }
            }
            await h.commandReceiver();
        }

        public static void Map(IApplicationBuilder app)
        {
            app.UseWebSockets();
            app.Use(SocketHandler.Acceptor);
        }
        
    }
}
