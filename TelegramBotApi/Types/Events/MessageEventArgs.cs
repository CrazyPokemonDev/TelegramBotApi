using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotApi.Types.Events
{
    /// <summary>
    /// Event args for a new message received by the bot
    /// </summary>
    public class MessageEventArgs : EventArgs
    {
        /// <summary>
        /// The message that has been received
        /// </summary>
        public Message Message { get; set; }

        /// <summary>
        /// Inititalizes a new instance of the <see cref="MessageEventArgs"/> class
        /// </summary>
        /// <param name="message"></param>
        public MessageEventArgs(Message message)
        {
            Message = message;
        }
    }
}
