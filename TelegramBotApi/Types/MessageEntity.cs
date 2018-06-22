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
                return Enum.GetMessageEntityType(_type);
            }
            set
            {
                _type = Enum.GetString(value);
            }
        }

        /// <summary>
        /// Offset of the entity in UTF-16 units
        /// </summary>
        [JsonProperty(PropertyName = "offset", Required = Required.Always)]
        public int Offset { get; set; }

        /// <summary>
        /// Length of the entity in UTF-16 units
        /// </summary>
        [JsonProperty(PropertyName = "length", Required = Required.Always)]
        public int Length { get; set; }

        /// <summary>
        /// Url that will be opened when clicking on the <see cref="MessageEntityType.TextLink"/>
        /// </summary>
        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }

        /// <summary>
        /// The mentioned user of a <see cref="MessageEntityType.TextMention"/>
        /// </summary>
        [JsonProperty(PropertyName = "user")]
        public User User { get; set; }

        /// <summary>
        /// The text value of this entity
        /// </summary>
        [JsonIgnore]
        public string Value { get; set; }
    }
}
