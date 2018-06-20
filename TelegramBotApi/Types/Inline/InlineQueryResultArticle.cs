using Newtonsoft.Json;
using TelegramBotApi.Types.Markup;

namespace TelegramBotApi.Types.Inline
{
    /// <summary>
    /// Represents a link to an article or web page.
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
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
        public string Title;

        /// <summary>
        /// Content of the message to be sent
        /// </summary>
        [JsonProperty(PropertyName = "input_message_content", Required = Required.Always)]
        public InputMessageContent InputMessageContent;

        /// <summary>
        /// Optional. Inline keyboard attached to the message
        /// </summary>
        [JsonProperty(PropertyName = "reply_markup")]
        public InlineKeyboardMarkup ReplyMarkup;

        /// <summary>
        /// Optional. URL of the result
        /// </summary>
        [JsonProperty(PropertyName = "url")]
        public string Url;

        /// <summary>
        /// Optional. Pass True, if you don't want the URL to be shown in the message
        /// </summary>
        [JsonProperty(PropertyName = "hide_url")]
        public bool HideUrl;

        /// <summary>
        /// Optional. Short description of the result
        /// </summary>
        [JsonProperty(PropertyName = "description")]
        public string Description;

        /// <summary>
        /// Optional. Url of the thumbnail for the result
        /// </summary>
        [JsonProperty(PropertyName = "thumb_url")]
        public string ThumbUrl;

        /// <summary>
        /// Optional. Thumbnail width
        /// </summary>
        [JsonProperty(PropertyName = "thumb_width")]
        public int ThumbWidth;

        /// <summary>
        /// Optional. Thumbnail height
        /// </summary>
        [JsonProperty(PropertyName = "thumb_height")]
        public int ThumbHeight;
    }
}
