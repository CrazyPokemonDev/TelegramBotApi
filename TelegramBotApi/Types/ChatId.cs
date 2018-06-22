using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotApi.Types
{
    /// <summary>
    /// Identifier of a telegram chat
    /// </summary>
    public class ChatId
    {
        /// <summary>
        /// The username of the public channel
        /// </summary>
        public string ChannelUsername { get; set; }

        /// <summary>
        /// The identifier of the group
        /// </summary>
        public long ChatIdentifier { get; set; }

        /// <summary>
        /// Automatically converts a long into a <see cref="ChatId"/>
        /// </summary>
        /// <param name="id">The unique identifier of the target chat</param>
        public static implicit operator ChatId(long id)
        {
            return new ChatId() { ChatIdentifier = id };
        }

        /// <summary>
        /// Automatically converts a string into a <see cref="ChatId"/>
        /// </summary>
        /// <param name="channelName">The username of the target public channel</param>
        public static implicit operator ChatId(string channelName)
        {
            return new ChatId() { ChannelUsername = channelName };
        }

        /// <summary>
        /// Explicitly converts a chatId to a long value
        /// </summary>
        /// <param name="cId">The chatId</param>
        public static explicit operator long(ChatId cId)
        {
            return cId.ChatIdentifier;
        }

        /// <summary>
        /// Explicitly converts a chatId to a string value
        /// </summary>
        /// <param name="cId">The chatId</param>
        public static explicit operator string(ChatId cId)
        {
            return cId.ChannelUsername;
        }
    }
}
