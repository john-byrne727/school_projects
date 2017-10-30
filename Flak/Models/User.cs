using System;
using System.Security.Cryptography;
using System.Text;

namespace WebApplication.Models
{
    public class User
    {
        private static SHA256 hashify = SHA256.Create();

        private String username;
        private String password;
        private String salt;

        /// <summary>
        /// A constructer that takes a username and plaintext password, then hashes the password.
        /// </summary>
        /// <param name="name">The username</param>
        /// <param name="pass">The password</param>
        public User(String name, String pass)
        {
            username = name;
            salt = User.generateSalt();
            password = User.getPasswordHash(pass + salt);
        }

        /// <summary>
        /// A constructor that takes the username, hashed password, and salt
        /// </summary>
        /// <param name="name">Username</param>
        /// <param name="pass">Hashed Password</param>
        /// <param name="salt">Salt</param>
        public User (String name, String pass, String salt)
        {
            username = name;
            password = pass;
            this.salt = salt;
        }

        /// <summary>
        /// Checks if the paramater string matches this users username
        /// </summary>
        /// <param name="str">The string to check against</param>
        /// <returns>True if the username matches the string, false otherwise</returns>
        public bool usernameMatch(String str)
        {
            if (str.CompareTo(username) == 0)
                return true;
            return false;
        }

        /// <summary>
        /// Accessor for the salt
        /// </summary>
        /// <returns>The salt</returns>
        public String getSalt()
        {
            return salt;
        }

        /// <summary>
        /// Verifies the input password and salt match this user
        /// </summary>
        /// <param name="input">The plaintext attempted password</param>
        /// <param name="salt">The attempted salt</param>
        /// <returns>True if the password and salt are correct, false otherwise</returns>
        public bool verifyPassword(String input, String salt)
        {
            return getPasswordHash(input + salt).Equals(password);
        }

        /// <summary>
        /// Returns a string of the password and salt, for filekeeping purposes
        /// </summary>
        /// <returns>String of password and salt</returns>
        public String passSalt()
        {
            return password + " " + salt;
        }

        /// <summary>
        /// Returns the password hash.
        /// </summary>
        /// <param name="password">The password to be hashed</param>
        /// <returns></returns>
        public static String getPasswordHash(String password)
        {
            byte[] hash = hashify.ComputeHash(Encoding.UTF8.GetBytes(password));
            String hashed = "";
            foreach(byte b in hash)
            {
                hashed += b.ToString("x2");
            }
            return hashed;
        }

        
        /// <summary>
        /// Generates a salt
        /// </summary>
        /// <returns>The salt</returns>
        public static String generateSalt()
        {
            var salt_b = new byte[32];
            var rand = new Random();
            rand.NextBytes(salt_b);

            var salt = "";
            foreach(byte b in salt_b)
            {
                salt += b.ToString("x2");
            }

            return salt;
        }
    }
}
