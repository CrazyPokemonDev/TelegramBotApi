using Newtonsoft.Json;
using TelegramBotApi.Enums;

namespace TelegramBotApi.Types.Inline
{
    /// <summary>
    /// Represents a link to an article or web page.
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
    public class InputTextMessageContent : InputMessageContent
    {
        /// <summary>
        /// Text of the message to be sent, 1-4096 characters
        /// </summary>
        [JsonProperty(PropertyName = "message_text", Required = Required.Always)]
        public string MessageText { get; set; }

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
        /// Optional. Disables link previews for links in the sent message
        /// </summary>
        [JsonProperty(PropertyName = "disable_web_page_preview")]
        public bool DisableWebPagePreview { get; set; }

    }
}
