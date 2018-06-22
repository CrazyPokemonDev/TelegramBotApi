using System;

namespace TelegramBotApi.Types.Exceptions
{
    /// <summary>
    /// An exception returned by the telegram bot API
    /// </summary>
    public class ApiRequestException : Exception
    {
        /// <summary>
        /// The response parameters to help resolve the error, if there are any
        /// </summary>
        public ResponseParameters Parameters { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiRequestException"/> class
        /// </summary>
        /// <param name="message">The error message</param>
        /// <param name="parameters">The parameters returned by the telegram bot API</param>
        public ApiRequestException(string message, ResponseParameters parameters = null) : base(message)
        {
            Parameters = parameters ?? new ResponseParameters();
        }
    }
}
