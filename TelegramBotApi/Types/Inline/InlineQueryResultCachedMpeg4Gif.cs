using Newtonsoft.Json;
using TelegramBotApi.Types.Markup;
using TelegramBotApi.Enums;

namespace TelegramBotApi.Types.Inline
{
    /// <summary>
    /// Represents a link to a video animation (H.264/MPEG-4 AVC video without sound) stored on the Telegram servers.
    /// By default, this animated MPEG-4 file will be sent by the user with an optional caption. 
    /// Alternatively, you can use input_message_content to send a message with the specified content instead of the animation.
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
    public class InlineQueryResultCachedMpeg4Gif : InlineQueryResult
    {
        private string _type = "mpeg4_gif";

        /// <summary>
        /// Type of the result, must be mpeg4_gif
        /// </summary>
        [JsonProperty(PropertyName = "type", Required = Required.Always)]
        public override string Type { get { return _type; } set { _type = "mpeg4_gif"; } }

        /// <summary>
        /// Unique identifier for this result, 1-64 Bytes
        /// </summary>
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public override string Id { get; set; }

        /// <summary>
        /// A valid file identifier for the MP4 file
        /// </summary>
        [JsonProperty(PropertyName = "mpeg4_file_id", Required = Required.Always)]
        public string Mpeg4FileId { get; set; }

        /// <summary>
        /// Optional. Title for the result
        /// </summary>
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        /// <summary>
        /// Optional. Caption of the MPEG-4 file to be sent, 0-200 characters
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
        /// Optional. Content of the message to be sent instead of the video animation
        /// </summary>
        [JsonProperty(PropertyName = "input_message_content", Required = Required.Always)]
        public InputMessageContent InputMessageContent { get; set; }
    }
}
