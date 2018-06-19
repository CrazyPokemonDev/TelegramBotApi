using Newtonsoft.Json;
using TelegramBotApi.Enums;

namespace TelegramBotApi.Types
{
    /// <summary>
    /// Represents a special entity in a text message
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class MessageEntity
    {
        [JsonProperty(PropertyName = "type", Required = Required.Always)]
        private string _type;
        /// <summary>
        /// Type of this message entity
        /// </summary>
        public MessageEntityType Type
        {
            get
            {
                switch (_type)
                {
                    case "mention":
                        return MessageEntityType.Mention;
                    case "hashtag":
                        return MessageEntityType.Hashtag;
                    case "bot_command":
                        return MessageEntityType.BotCommand;
                    case "url":
                        return MessageEntityType.Url;
                    case "email":
                        return MessageEntityType.Email;
                    case "bold":
                        return MessageEntityType.Bold;
                    case "italic":
                        return MessageEntityType.Italic;
                    case "code":
                        return MessageEntityType.Code;
                    case "pre":
                        return MessageEntityType.Pre;
                    case "text_link":
                        return MessageEntityType.TextLink;
                    case "text_mention":
                        return MessageEntityType.TextMention;
                    case "phone_number":
                        return MessageEntityType.PhoneNumber;
                    default:
                        return MessageEntityType.Unknown;
                }
            }
            set
            {
                switch (value)
                {
                    case MessageEntityType.Bold:
                        _type = "bold";
                        break;
                    case MessageEntityType.BotCommand:
                        _type = "bot_command";
                        break;
                    case MessageEntityType.Code:
                        _type = "code";
                        break;
                    case MessageEntityType.Email:
                        _type = "email";
                        break;
                    case MessageEntityType.Hashtag:
                        _type = "hashtag";
                        break;
                    case MessageEntityType.Italic:
                        _type = "italic";
                        break;
                    case MessageEntityType.Mention:
                        _type = "mention";
                        break;
                    case MessageEntityType.PhoneNumber:
                        _type = "phone_number";
                        break;
                    case MessageEntityType.Pre:
                        _type = "pre";
                        break;
                    case MessageEntityType.TextLink:
                        _type = "text_link";
                        break;
                    case MessageEntityType.TextMention:
                        _type = "text_mention";
                        break;
                    case MessageEntityType.Url:
                        _type = "url";
                        break;
                    default:
                        _type = "unknown";
                        break;
                }
            }
        }

        /// <summary>
        /// Offset of the entity in UTF-16 units
        /// </summary>
        [JsonProperty(PropertyName = "offset", Required = Required.Always)]
        public int Offset;

        /// <summary>
        /// Length of the entity in UTF-16 units
        /// </summary>
        [JsonProperty(PropertyName = "length", Required = Required.Always)]
        public int Length;

        /// <summary>
        /// Url that will be opened when clicking on the <see cref="MessageEntityType.TextLink"/>
        /// </summary>
        [JsonProperty(PropertyName = "url")]
        public string Url;

        /// <summary>
        /// The mentioned user of a <see cref="MessageEntityType.TextMention"/>
        /// </summary>
        [JsonProperty(PropertyName = "user")]
        public User User;

        /// <summary>
        /// The text value of this entity
        /// </summary>
        [JsonIgnore]
        public string Value;
    }
}
