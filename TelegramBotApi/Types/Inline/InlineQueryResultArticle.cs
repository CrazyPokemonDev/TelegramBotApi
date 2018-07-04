using Newtonsoft.Json;
using TelegramBotApi.Types.Markup;

namespace TelegramBotApi.Types.Inline
{
    /// <summary>
    /// Represents a link to an article or web page.
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
    public class InlineQueryResultArticle : InlineQueryResult
    {
        private string _type = "article";

        /// <summary>
        /// Type of the result, must be article
        /// </summary>
        [JsonProperty(PropertyName = "type", Required = Required.Always)]
        public override string Type { get { return _type; } set { _type = "article"; } }

        /// <summary>
        /// Unique identifier for this result, 1-64 Bytes
        /// </summary>
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public override string Id { get; set; }

        /// <summary>
        /// Title of the result
        /// </summary>
        [JsonProperty(PropertyName = "title", Required = Required.Always)]
        public string Title { get; set; }

        /// <summary>
        /// Content of the message to be sent
        /// </summary>
        [JsonProperty(PropertyName = "input_message_content", Required = Required.Always)]
        public InputMessageContent InputMessageContent { get; set; }

        /// <summary>
        /// Optional. Inline keyboard attached to the message
        /// </summary>
        [JsonProperty(PropertyName = "reply_markup")]
        public InlineKeyboardMarkup ReplyMarkup { get; set; }

        /// <summary>
        /// Optional. URL of the result
        /// </summary>
        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }

        /// <summary>
        /// Optional. Pass True, if you don't want the URL to be shown in the message
        /// </summary>
        [JsonProperty(PropertyName = "hide_url")]
        public bool HideUrl { get; set; }

        /// <summary>
        /// Optional. Short description of the result
        /// </summary>
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        /// <summary>
        /// Optional. Url of the thumbnail for the result
        /// </summary>
        [JsonProperty(PropertyName = "thumb_url")]
        public string ThumbUrl { get; set; }

        /// <summary>
        /// Optional. Thumbnail width
        /// </summary>
        [JsonProperty(PropertyName = "thumb_width")]
        public int ThumbWidth { get; set; }

        /// <summary>
        /// Optional. Thumbnail height
        /// </summary>
        [JsonProperty(PropertyName = "thumb_height")]
        public int ThumbHeight { get; set; }
    }
}
