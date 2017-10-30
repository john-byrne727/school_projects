using System;
using System.Collections.Generic;
using System.IO;

namespace WebApplication.Models
{
    public class FlakData
    {
        private static List<User> users = new List<User>();

        /// <summary>
        /// Reads the persistent messages from a file.
        /// </summary>
        /// <returns>A string array of persistent messages</returns>
        public static String[] getPersistentMessages()
        {
            String[] lines;
            try
            {
                lines = File.ReadAllLines("messages.txt");
            }
            catch (FileNotFoundException)
            {
                lines = null;
            }

            return lines;
        }

        /// <summary>
        /// Writes a message to the file for persistance
        /// </summary>
        /// <param name="msg">The message to write</param>
        public static void writeMessageToPersist(String msg)
        {
            var line = new String[1];
            line[0] = msg;
            File.AppendAllLines("messages.txt", line);
        }

        /// <summary>
        /// Checks if a username is available or already in use.
        /// </summary>
        /// <param name="username">The username to check</param>
        /// <returns>True if the username is available, false if not</returns>
        public static bool checkUsername(String username)
        {
            bool available = true;
            foreach(User u in users)
            {
                if (u.usernameMatch(username))
                {
                    available = false;
                    break;
                }
            }
            return available;
        }

        /// <summary>
        /// Finds a user with a given username.
        /// </summary>
        /// <param name="username">The username to search for</param>
        /// <returns>The user with the provided username, or null if the username does not exist.</returns>
        public static User getUser(String username)
        {
            foreach(User u in users)
            {
                if (u.usernameMatch(username))
                {
                    return u;
                }
            }

            return null;
        }

        /// <summary>
        /// Reads user data from a file and adds User objects to the users list.
        /// </summary>
        public static void readUsersFromFile()
        {
            try
            {
                var lines = File.ReadAllLines("users.txt");
                foreach (String line in lines)
                {
                    var tokens = line.Split(' ');
                    users.Add(new User(tokens[0], tokens[1], tokens[2]));
                }
            }
            catch (FileNotFoundException)
            {

            }
        }

        /// <summary>
        /// Creates a new user and adds their data to a file.
        /// </summary>
        /// <param name="username">The username</param>
        /// <param name="password">The password</param>
        public static void makeNewUser(String username, String password)
        {

            var u = new User(username, password);
            users.Add(u);
            var line = new String[1];
            line[0] = username + " " +u.passSalt();
            File.AppendAllLines("users.txt", line);
        }
    }
}
