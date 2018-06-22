using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotApi.Types.Events
{
    /// <summary>
    /// Event args for a new incoming channel post
    /// </summary>
    public class ChannelPostEventArgs : EventArgs
    {
        /// <summary>
        /// The sent message in the channel
        /// </summary>
        public Message ChannelPost { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelPostEventArgs"/> class
        /// </summary>
        /// <param name="channelPost"></param>
        public ChannelPostEventArgs(Message channelPost)
        {
            ChannelPost = channelPost;
        }
    }
}
