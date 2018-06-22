using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotApi.Types.Events
{
    /// <summary>
    /// Event args for a message that has been edited
    /// </summary>
    public class EditedMessageEventArgs : EventArgs
    {
        /// <summary>
        /// The edited message
        /// </summary>
        public Message EditedMessage { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EditedMessageEventArgs"/> class
        /// </summary>
        /// <param name="editedMessage">The edited message</param>
        public EditedMessageEventArgs(Message editedMessage)
        {
            EditedMessage = editedMessage;
        }
    }
}
