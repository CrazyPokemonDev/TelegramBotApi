using Newtonsoft.Json;
using TelegramBotApi.Enums;

namespace TelegramBotApi.Types
{
    /// <summary>
    /// Represents a chat.
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
    public class Chat
    {
        /// <summary>
        /// Unique identifier of this chat
        /// </summary>
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public long Id { get; set; }

        [JsonProperty(PropertyName = "type")]
        private string _type;
        /// <summary>
        /// Type of the chat
        /// </summary>
        public ChatType Type
        {
            get
            {
                return Enum.GetChatType(_type);
            }
            set
            {
                _type = Enum.GetString(value);
            }
        }

        /// <summary>
        /// The title of the chat for groups, supergroups, or channels.
        /// </summary>
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        /// <summary>
        /// The username of the private chat, supergroup or channel if available
        /// </summary>
        [JsonProperty(PropertyName = "username")]
        public string Username { get; set; }

        /// <summary>
        /// The first name of the other party in a private chat
        /// </summary>
        [JsonProperty(PropertyName = "first_name")]
        public string FirstName { get; set; }

        /// <summary>
        /// The last name of the other party in a private chat
        /// </summary>
        [JsonProperty(PropertyName = "last_name")]
        public string LastName { get; set; }

        /// <summary>
        /// True if "All members are admins" is enabled in this chat
        /// </summary>
        [JsonProperty(PropertyName = "all_members_are_administrators")]
        public bool AllMembersAreAdministrators { get; set; }

        /// <summary>
        /// Chat photo. ONLY returned in getChat.
        /// </summary>
        [JsonProperty(PropertyName = "photo")]
        public ChatPhoto Photo { get; set; }

        /// <summary>
        /// Description of a supergroup or channel. ONLY returned in getChat.
        /// </summary>
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        /// <summary>
        /// Chat invite link, for supergroups and channels if available. ONLY returned in getChat.
        /// </summary>
        [JsonProperty(PropertyName = "invite_link")]
        public string InviteLink { get; set; }

        /// <summary>
        /// The pinned message of a supergroup or channel. ONLY returned in getChat.
        /// </summary>
        [JsonProperty(PropertyName = "pinned_message")]
        public Message PinnedMessage { get; set; }

        /// <summary>
        /// Name of sticker set for supergroups. ONLY returned in getChat.
        /// </summary>
        [JsonProperty(PropertyName = "sticker_set_name")]
        public string StickerSetName { get; set; }

        /// <summary>
        /// Whether the bot is able to set the supergroups sticker set. ONLY returned in getChat.
        /// </summary>
        [JsonProperty(PropertyName = "can_set_sticker_set")]
        public bool CanSetStickerSet { get; set; }
    }
}
