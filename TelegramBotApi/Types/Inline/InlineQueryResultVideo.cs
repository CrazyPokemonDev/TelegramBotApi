using Newtonsoft.Json;
using TelegramBotApi.Types.Markup;
using TelegramBotApi.Enums;

namespace TelegramBotApi.Types.Inline
{
    /// <summary>
    /// Represents a link to a page containing an embedded video player or a video file. 
    /// By default, this video file will be sent by the user with an optional caption. 
    /// Alternatively, you can use input_message_content to send a message with the specified content instead of the video.
    /// 
    /// If an InlineQueryResultVideo message contains an embedded video (e.g., YouTube), you must replace its content using input_message_content.
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class InlineQueryResultVideo : InlineQueryResult
    {
        private string _type = "gif";

        /// <summary>
        /// Type of the result, must be video
        /// </summary>
        [JsonProperty(PropertyName = "type", Required = Required.Always)]
        public override string Type { get { return _type; } set { _type = "gif"; } }

        /// <summary>
        /// Unique identifier for this result, 1-64 Bytes
        /// </summary>
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public override string Id { get; set; }

        /// <summary>
        /// A valid URL for the embedded video player or video file
        /// </summary>
        [JsonProperty(PropertyName = "video_url", Required = Required.Always)]
        public string VideoUrl;

        /// <summary>
        /// Mime type of the content of video url, “text/html” or “video/mp4”
        /// </summary>
        [JsonProperty(PropertyName = "mime_type", Required = Required.Always)]
        public string MimeType;

        /// <summary>
        /// URL of the thumbnail (jpeg only) for the video
        /// </summary>
        [JsonProperty(PropertyName = "thumb_url", Required = Required.Always)]
        public string ThumbUrl;


        /// <summary>
        /// Optional. Title for the result
        /// </summary>
        [JsonProperty(PropertyName = "title")]
        public string Title;

        /// <summary>
        /// Optional. Caption of the GIF file to be sent, 0-200 characters
        /// </summary>
        [JsonProperty(PropertyName = "caption")]
        public string Caption;


        /// <summary>
        /// Optional. Send Markdown or HTML, if you want Telegram apps to show bold, italic, fixed-width text or inline URLs in your bot's message.
        /// </summary>
        [JsonProperty(PropertyName = "parse_mode")]
        private string _parseMode = null;

        /// <summary>
        /// The parse mode of the caption
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
        /// Optional. Video Width
        /// </summary>
        [JsonProperty(PropertyName = "video_width")]
        public int VideoWidth;

        /// <summary>
        /// Optional. Video Height
        /// </summary>
        [JsonProperty(PropertyName = "video_height")]
        public int VideoHeight;

        /// <summary>
        /// Optional. Video duration in seconds
        /// </summary>
        [JsonProperty(PropertyName = "video_duration")]
        public int VideoDuration;

        /// <summary>
        /// Optional. Short description of the result
        /// </summary>
        [JsonProperty(PropertyName = "description")]
        public string Description;

        /// <summary>
        /// Optional. Inline keyboard attached to the message
        /// </summary>
        [JsonProperty(PropertyName = "reply_markup")]
        public InlineKeyboardMarkup ReplyMarkup;

        /// <summary>
        /// Content of the message to be sent. 
        /// This field is required if InlineQueryResultVideo is used to send an HTML-page as a result (e.g., a YouTube video).
        /// </summary>
        [JsonProperty(PropertyName = "input_message_content")]
        public InputMessageContent InputMessageContent;
    }
}
