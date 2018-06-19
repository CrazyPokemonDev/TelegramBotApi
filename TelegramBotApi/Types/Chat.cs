using Newtonsoft.Json;
using TelegramBotApi.Enums;

namespace TelegramBotApi.Types
{
    /// <summary>
    /// Represents a chat.
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class Chat
    {
        /// <summary>
        /// Unique identifier of this chat
        /// </summary>
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public long Id;

        [JsonProperty(PropertyName = "type")]
        private string _type;
        /// <summary>
        /// Type of the chat
        /// </summary>
        public ChatType Type
        {
            get
            {
                switch (_type)
                {
                    case "private":
                        return ChatType.Private;
                    case "group":
                        return ChatType.Group;
                    case "supergroup":
                        return ChatType.Supergroup;
                    case "channel":
                        return ChatType.Channel;
                    default:
                        return ChatType.Unknown;
                }
            }
            set
            {
                switch (value)
                {
                    case ChatType.Channel:
                        _type = "channel";
                        break;
                    case ChatType.Group:
                        _type = "group";
                        break;
                    case ChatType.Private:
                        _type = "private";
                        break;
                    case ChatType.Supergroup:
                        _type = "supergroup";
                        break;
                    default:
                        _type = "unknown";
                        break;
                }
            }
        }

        /// <summary>
        /// The title of the chat for groups, supergroups, or channels.
        /// </summary>
        [JsonProperty(PropertyName = "title")]
        public string Title;

        /// <summary>
        /// The username of the private chat, supergroup or channel if available
        /// </summary>
        [JsonProperty(PropertyName = "username")]
        public string Username;

        /// <summary>
        /// The first name of the other party in a private chat
        /// </summary>
        [JsonProperty(PropertyName = "first_name")]
        public string FirstName;

        /// <summary>
        /// The last name of the other party in a private chat
        /// </summary>
        [JsonProperty(PropertyName = "last_name")]
        public string LastName;

        /// <summary>
        /// True if "All members are admins" is enabled in this chat
        /// </summary>
        [JsonProperty(PropertyName = "all_members_are_administrators")]
        public bool AllMembersAreAdministrators;

        /// <summary>
        /// Chat photo. ONLY returned in getChat.
        /// </summary>
        [JsonProperty(PropertyName = "photo")]
        public ChatPhoto Photo;

        /// <summary>
        /// Description of a supergroup or channel. ONLY returned in getChat.
        /// </summary>
        [JsonProperty(PropertyName = "description")]
        public string Description;

        /// <summary>
        /// Chat invite link, for supergroups and channels if available. ONLY returned in getChat.
        /// </summary>
        [JsonProperty(PropertyName = "invite_link")]
        public string InviteLink;

        /// <summary>
        /// The pinned message of a supergroup or channel. ONLY returned in getChat.
        /// </summary>
        [JsonProperty(PropertyName = "pinned_message")]
        public Message PinnedMessage;

        /// <summary>
        /// Name of sticker set for supergroups. ONLY returned in getChat.
        /// </summary>
        [JsonProperty(PropertyName = "sticker_set_name")]
        public string StickerSetName;

        /// <summary>
        /// Whether the bot is able to set the supergroups sticker set. ONLY returned in getChat.
        /// </summary>
        [JsonProperty(PropertyName = "can_set_sticker_set")]
        public bool CanSetStickerSet;
    }
}
