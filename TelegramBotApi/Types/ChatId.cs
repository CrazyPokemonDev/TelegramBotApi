using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBotApi.Enums;

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
            return new ChatId() { ChannelUsername = channelName.StartsWith("@") ? channelName : "@" + channelName };
        }

        /// <summary>
        /// Automatically converts a <see cref="Chat"/> object into a <see cref="ChatId"/>
        /// </summary>
        /// <param name="chat">The <see cref="Chat"/> to which the desired id belongs</param>
        public static implicit operator ChatId(Chat chat)
        {
            if (chat.Type == ChatType.Channel && chat.Username != null) return new ChatId() { ChannelUsername = chat.Username };
            else return new ChatId() { ChatIdentifier = chat.Id };
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

        /// <summary>
        /// Find out whether this <see cref="ChatId"/> is equal to another object. This is true if the other object is a <see cref="ChatId"/> and has the same <see cref="ChatIdentifier"/> and <see cref="ChannelUsername"/>.
        /// </summary>
        /// <param name="obj">The object to compare this <see cref="ChatId"/> with.</param>
        /// <returns>True if <paramref name="obj"/> is equal to this <see cref="ChatId"/>.</returns>
        public override bool Equals(object obj)
        {
            return obj is ChatId cId && ChatIdentifier == cId.ChatIdentifier && ChannelUsername == cId.ChannelUsername;
        }

        /// <summary>
        /// Get the HashCode for this <see cref="ChatId"/> object
        /// </summary>
        /// <returns>The <see cref="ChatId"/>'s HashCode</returns>
        public override int GetHashCode()
        {
            return $"{ChannelUsername}{ChatIdentifier}".GetHashCode();
        }
    }
}
