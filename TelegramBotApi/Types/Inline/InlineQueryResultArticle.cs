using Newtonsoft.Json;

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

        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public override string Id { get; set; }

        [JsonProperty(PropertyName = "title", Required = Required.Always)]
        public string Title;

        [JsonProperty(PropertyName = "input_message_content", Required = Required.Always)]
        public InputMessageContent InputMessageContent;
    }
}
