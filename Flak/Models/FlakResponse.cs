using System;

namespace WebApplication.Models
{
    public class FlakResponse
    {
        public String type;
        public String message;
        public String date;

        /// <summary>
        /// Creates a response object to be sent to the view.
        /// </summary>
        /// <param name="type">The type of response</param>
        /// <param name="message">The message of the response</param>
        /// <param name="date">The date associated with the response in string format</param>
        public FlakResponse(String type, String message, String date)
        {
            this.type = type;
            this.message = message;
            this.date = date;
        }
    }
}