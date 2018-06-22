using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotApi.Types.Events
{
    /// <summary>
    /// Event args for an edited channel post
    /// </summary>
    public class EditedChannelPostEventArgs : EventArgs
    {
        /// <summary>
        /// The edited message in the channel
        /// </summary>
        public Message EditedChannelPost { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EditedChannelPostEventArgs"/> class
        /// </summary>
        /// <param name="editedChannelPost">The edited message in the channel</param>
        public EditedChannelPostEventArgs(Message editedChannelPost)
        {
            EditedChannelPost = editedChannelPost;
        }
    }
}
