using Newtonsoft.Json;
using TelegramBotApi.Types.Markup;
using TelegramBotApi.Enums;

namespace TelegramBotApi.Types.Inline
{
    /// <summary>
    /// Represents a link to a photo. By default, this photo will be sent by the user with optional caption. 
    /// Alternatively, you can use input_message_content to send a message with the specified content instead of the photo.
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
    public class InlineQueryResultPhoto : InlineQueryResult
    {
        private string _type = "photo";

        /// <summary>
        /// Type of the result, must be photo
        /// </summary>
        [JsonProperty(PropertyName = "type", Required = Required.Always)]
        public override string Type { get { return _type; } set { _type = "photo"; } }

        /// <summary>
        /// Unique identifier for this result, 1-64 Bytes
        /// </summary>
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public override string Id { get; set; }

        /// <summary>
        /// A valid URL of the photo. Photo must be in jpeg format. Photo size must not exceed 5MB
        /// </summary>
        [JsonProperty(PropertyName = "photo_url", Required = Required.Always)]
        public string PhotoUrl { get; set; }

        /// <summary>
        /// URL of the thumbnail for the photo
        /// </summary>
        [JsonProperty(PropertyName = "thumb_url", Required = Required.Always)]
        public string ThumbUrl { get; set; }

        /// <summary>
        /// Optional. Width of the photo
        /// </summary>
        [JsonProperty(PropertyName = "photo_width")]
        public int PhotoWidth { get; set; }

        /// <summary>
        /// Optional. Height of the photo
        /// </summary>
        [JsonProperty(PropertyName = "photo_height")]
        public int PhotoHeight { get; set; }

        /// <summary>
        /// Optional. Title for the result
        /// </summary>
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        /// <summary>
        /// Optional. Short description of the result
        /// </summary>
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        /// <summary>
        /// Optional. Caption of the photo to be sent, 0-200 characters
        /// </summary>
        [JsonProperty(PropertyName = "caption")]
        public string Caption { get; set; }
        
        [JsonProperty(PropertyName = "parse_mode")]
        private string _parseMode = null;

        /// <summary>
        /// Optional. Send Markdown or HTML, if you want Telegram apps to show bold, italic, fixed-width text or inline URLs in your bot's message.
        /// </summary>
        public ParseMode ParseMode
        {
            get
            {
                return Enum.GetParseMode(_parseMode);
            }
            set
            {
                _parseMode = Enum.GetString(value);
            }
        }

        /// <summary>
        /// Optional. Inline keyboard attached to the message
        /// </summary>
        [JsonProperty(PropertyName = "reply_markup")]
        public InlineKeyboardMarkup ReplyMarkup { get; set; }

        /// <summary>
        /// Content of the message to be sent
        /// </summary>
        [JsonProperty(PropertyName = "input_message_content", Required = Required.Always)]
        public InputMessageContent InputMessageContent { get; set; }
    }
}
