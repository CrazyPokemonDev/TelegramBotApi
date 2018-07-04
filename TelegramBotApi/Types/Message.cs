using Newtonsoft.Json;
using System;
using TelegramBotApi.Enums;
using TelegramBotApi.Types.Payment;

namespace TelegramBotApi.Types
{
    /// <summary>
    /// Represents a message of any type
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
    public class Message
    {
        /// <summary>
        /// Unique identifier of the message inside its chat
        /// </summary>
        [JsonProperty(PropertyName = "message_id")]
        public int MessageId { get; set; }

        /// <summary>
        /// The user who sent this message. Empty for messages in channels.
        /// </summary>
        [JsonProperty(PropertyName = "from")]
        public User From { get; set; }

        [JsonProperty(PropertyName = "date")]
        private long _date;
        /// <summary>
        /// The exact time this message was sent
        /// </summary>
        public DateTime Date
        {
            get
            {
                return DateTimeOffset.FromUnixTimeSeconds(_date).DateTime;
            }
            set
            {
                _date = ((DateTimeOffset)value).ToUnixTimeSeconds();
            }
        }

        /// <summary>
        /// The chat this message was sent in
        /// </summary>
        [JsonProperty(PropertyName = "chat")]
        public Chat Chat { get; set; }

        /// <summary>
        /// The user this message was forwarded from, if any
        /// </summary>
        [JsonProperty(PropertyName = "forwarded_from")]
        public User ForwardFrom { get; set; }

        /// <summary>
        /// Info about the channel this message was forwarded from, if any
        /// </summary>
        [JsonProperty(PropertyName = "forward_from_chat")]
        public Chat ForwardFromChat { get; set; }

        /// <summary>
        /// The message id of the original message, if forwarded from a channel
        /// </summary>
        [JsonProperty(PropertyName = "forward_from_message_id")]
        public int ForwardFromMessageId { get; set; }

        /// <summary>
        /// The signature of the post author for messages forwarded from channels, if present
        /// </summary>
        [JsonProperty(PropertyName = "forward_signature")]
        public string ForwardSignature { get; set; }

        [JsonProperty("forward_date")]
        private long _forwardDate;
        /// <summary>
        /// For forwarded messages, the date and time the original message was sent
        /// </summary>
        public DateTime ForwardDate
        {
            get
            {
                return DateTimeOffset.FromUnixTimeSeconds(_forwardDate).DateTime;
            }
            set
            {
                _forwardDate = ((DateTimeOffset)value).ToUnixTimeSeconds();
            }
        }

        /// <summary>
        /// The message this message is a reply to, if any. Not present if this already is a ReplyToMessage.
        /// </summary>
        [JsonProperty(PropertyName = "reply_to_message")]
        public Message ReplyToMessage { get; set; }

        [JsonProperty(PropertyName = "edit_date")]
        private long _editDate;
        /// <summary>
        /// Date and time this message was last edited.
        /// </summary>
        public DateTime EditDate
        {
            get
            {
                return DateTimeOffset.FromUnixTimeSeconds(_editDate).DateTime;
            }
            set
            {
                _editDate = ((DateTimeOffset)value).ToUnixTimeSeconds();
            }
        }

        /// <summary>
        /// The unique identifier of a media message group this message belongs to, if any
        /// </summary>
        [JsonProperty(PropertyName = "media_group_id")]
        public string MediaGroupId { get; set; }

        /// <summary>
        /// Signature of the post author for messages in channels
        /// </summary>
        [JsonProperty(PropertyName = "author_signature")]
        public string AuthorSignature { get; set; }

        /// <summary>
        /// The text of the message, for text messages
        /// </summary>
        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "entities")]
        private MessageEntity[] _entities;
        /// <summary>
        /// Special entities like bot commands, mentions, links etc.
        /// </summary>
        public MessageEntity[] Entities
        {
            get { return _entities; }
            set
            {
                for (int i = 0; i < value.Length; i++)
                {
                    value[i].Value = Text.Substring(value[i].Offset, value[i].Length);
                    _entities = value;
                }
            }
        }

        [JsonProperty(PropertyName = "caption_entities")]
        private MessageEntity[] _captionEntities;
        /// <summary>
        /// Special entities like bot commands, mentions, links etc. in the caption of a media message
        /// </summary>
        public MessageEntity[] CaptionEntities
        {
            get { return _captionEntities; }
            set
            {
                for (int i = 0; i < value.Length; i++)
                {
                    value[i].Value = Caption.Substring(value[i].Offset, value[i].Length);
                    _captionEntities = value;
                }
            }
        }

        /// <summary>
        /// Information about the audio file, if this is one
        /// </summary>
        [JsonProperty(PropertyName = "audio")]
        public Audio Audio { get; set; }

        /// <summary>
        /// Information about the general file, if this is one
        /// </summary>
        [JsonProperty(PropertyName = "document")]
        public Document Document { get; set; }

        /// <summary>
        /// Information about the game, if this is a game message
        /// </summary>
        [JsonProperty(PropertyName = "game")]
        public Game.Game Game { get; set; }

        /// <summary>
        /// Available sizes of the photo, if this is one
        /// </summary>
        [JsonProperty(PropertyName = "photo")]
        public PhotoSize[] Photo { get; set; }

        /// <summary>
        /// Information about the sticker, if this is one
        /// </summary>
        [JsonProperty(PropertyName = "sticker")]
        public Sticker Sticker { get; set; }

        /// <summary>
        /// Information about the video, if this is one
        /// </summary>
        [JsonProperty(PropertyName = "video")]
        public Video Video { get; set; }

        /// <summary>
        /// Information about the voice message, if this is one
        /// </summary>
        [JsonProperty(PropertyName = "voice")]
        public Voice Voice { get; set; }

        /// <summary>
        /// Information about the video note, if this is one
        /// </summary>
        [JsonProperty(PropertyName = "video_note")]
        public VideoNote VideoNote { get; set; }

        /// <summary>
        /// Caption of the media message, if this is one
        /// </summary>
        [JsonProperty(PropertyName = "caption")]
        public string Caption { get; set; }

        /// <summary>
        /// Information about the contact, if this is one
        /// </summary>
        [JsonProperty(PropertyName = "contact")]
        public Contact Contact { get; set; }

        /// <summary>
        /// Information about the location, if this is one
        /// </summary>
        [JsonProperty(PropertyName = "location")]
        public Location Location { get; set; }

        /// <summary>
        /// Information about the venue, if this is one
        /// </summary>
        [JsonProperty(PropertyName = "venue")]
        public Venue Venue { get; set; }

        /// <summary>
        /// New member(s) added to the (super)group and information about them (the bot itself might be one of them), if any
        /// </summary>
        [JsonProperty(PropertyName = "new_chat_members")]
        public User[] NewChatMembers { get; set; }

        /// <summary>
        /// Information about a member that was removed from the group, if any
        /// </summary>
        [JsonProperty(PropertyName = "left_chat_member")]
        public User LeftChatMember { get; set; }

        /// <summary>
        /// The new chat title, if it has been changed
        /// </summary>
        [JsonProperty(PropertyName = "new_chat_title")]
        public string NewChatTitle { get; set; }

        /// <summary>
        /// Different sizes of the new chat photo, if it has been changed
        /// </summary>
        [JsonProperty(PropertyName = "new_chat_photo")]
        public PhotoSize[] NewChatPhoto { get; set; }

        /// <summary>
        /// True if this is a service message saying the chat photo has been deleted
        /// </summary>
        [JsonProperty(PropertyName = "delete_chat_photo")]
        public bool DeleteChatPhoto { get; set; } = false;

        /// <summary>
        /// True if this is a service message saying this group has been created
        /// </summary>
        [JsonProperty(PropertyName = "group_chat_created")]
        public bool GroupChatCreated { get; set; } = false;

        /// <summary>
        /// Service message that a supergroup has been created. Can only be found in ReplyToMessage.
        /// </summary>
        [JsonProperty(PropertyName = "supergroup_chat_created")]
        public bool SupergroupChatCreated { get; set; } = false;

        /// <summary>
        /// Service message that a channel has been created. Can only be found in ReplyToMessage.
        /// </summary>
        [JsonProperty(PropertyName = "channel_chat_created")]
        public bool ChannelChatCreated { get; set; } = false;

        /// <summary>
        /// If this is present, this group has been migrated to a supergroup with the specified identifier.
        /// </summary>
        [JsonProperty(PropertyName = "migrate_to_chat_id")]
        public long MigrateToChatId { get; set; } = 0;

        /// <summary>
        /// If this is present, this supergroup has been migrated from a group with the specified identifier.
        /// </summary>
        [JsonProperty(PropertyName = "migrate_from_chat_id")]
        public long MigrateFromChatId { get; set; } = 0;

        /// <summary>
        /// Message that is pinned in this chat, if any. Will not contain a ReplyToMessage.
        /// </summary>
        [JsonProperty(PropertyName = "pinned_message")]
        public Message PinnedMessage { get; set; }

        /// <summary>
        /// Information about this invoice for payment, if this is one
        /// </summary>
        [JsonProperty(PropertyName = "invoice")]
        public Invoice Invoice { get; set; }

        /// <summary>
        /// Information about a successful payment, if this is a message about one
        /// </summary>
        [JsonProperty(PropertyName = "successful_payment")]
        public SuccessfulPayment SuccessfulPayment { get; set; }

        /// <summary>
        /// The domain name of the website the user has logged in to using Telegram Login, if any
        /// </summary>
        [JsonProperty(PropertyName = "connected_website")]
        public string ConnectedWebsite { get; set; }

        /// <summary>
        /// Type of this message
        /// </summary>
        public MessageType Type
        {
            get
            {
                if (Text != null) return MessageType.Text;
                else if (Audio != null) return MessageType.Audio;
                else if (Document != null) return MessageType.Document;
                else if (Game != null) return MessageType.Game;
                else if (Photo != null) return MessageType.Photo;
                else if (Sticker != null) return MessageType.Sticker;
                else if (Video != null) return MessageType.Video;
                else if (Voice != null) return MessageType.Voice;
                else if (VideoNote != null) return MessageType.VideoNote;
                else if (Contact != null) return MessageType.Contact;
                else if (Location != null) return MessageType.Location;
                else if (Venue != null) return MessageType.Venue;
                else if (NewChatMembers != null) return MessageType.NewChatMembers;
                else if (LeftChatMember != null) return MessageType.LeftChatMember;
                else if (NewChatTitle != null) return MessageType.NewChatTitle;
                else if (NewChatPhoto != null) return MessageType.NewChatPhoto;
                else if (DeleteChatPhoto) return MessageType.DeleteChatPhoto;
                else if (GroupChatCreated) return MessageType.GroupChatCreated;
                else if (SupergroupChatCreated) return MessageType.SupergroupChatCreated;
                else if (ChannelChatCreated) return MessageType.ChannelChatCreated;
                else if (MigrateToChatId != 0 || MigrateFromChatId != 0) return MessageType.GroupMigrated;
                else if (PinnedMessage != null) return MessageType.PinnedMessage;
                else if (Invoice != null) return MessageType.Invoice;
                else if (SuccessfulPayment != null) return MessageType.SuccessfulPayment;
                else return MessageType.Unknown;
            }
        }
    }
}
