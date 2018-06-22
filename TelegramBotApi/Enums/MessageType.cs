using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotApi.Enums
{
    /// <summary>
    /// Type of a message
    /// </summary>
    public enum MessageType
    {
        /// <summary>
        /// A text message
        /// </summary>
        Text,
        /// <summary>
        /// An audio message (treated as music, as opposed to <see cref="Voice"/>)
        /// </summary>
        Audio,
        /// <summary>
        /// A general file
        /// </summary>
        Document,
        /// <summary>
        /// A telegram embedded game
        /// </summary>
        Game,
        /// <summary>
        /// A photo
        /// </summary>
        Photo,
        /// <summary>
        /// A sticker
        /// </summary>
        Sticker,
        /// <summary>
        /// A video
        /// </summary>
        Video,
        /// <summary>
        /// A voice note (as opposed to <see cref="Audio"/>)
        /// </summary>
        Voice,
        /// <summary>
        /// A video note
        /// </summary>
        VideoNote,
        /// <summary>
        /// A phone contact
        /// </summary>
        Contact,
        /// <summary>
        /// A geographic location
        /// </summary>
        Location,
        /// <summary>
        /// A venue
        /// </summary>
        Venue,
        /// <summary>
        /// New members or the bot itself have been added to a chat
        /// </summary>
        NewChatMembers,
        /// <summary>
        /// A member was removed from a chat
        /// </summary>
        LeftChatMember,
        /// <summary>
        /// The chat title has been changed
        /// </summary>
        NewChatTitle,
        /// <summary>
        /// The chat photo has been changed
        /// </summary>
        NewChatPhoto,
        /// <summary>
        /// The chat photo has been deleted
        /// </summary>
        DeleteChatPhoto,
        /// <summary>
        /// A group chat has been created
        /// </summary>
        GroupChatCreated,
        /// <summary>
        /// A supergroup chat has been created (only found in messages replied to)
        /// </summary>
        SupergroupChatCreated,
        /// <summary>
        /// A channel has been created (only found in messages replied to)
        /// </summary>
        ChannelChatCreated,
        /// <summary>
        /// A group has been migrated to a supergroup
        /// </summary>
        GroupMigrated,
        /// <summary>
        /// A message has been pinned
        /// </summary>
        PinnedMessage,
        /// <summary>
        /// An invoice
        /// </summary>
        Invoice,
        /// <summary>
        /// Confirmation of a successful payment
        /// </summary>
        SuccessfulPayment,
        /// <summary>
        /// A message type that hasn't been implemented yet
        /// </summary>
        Unknown
    }
}
