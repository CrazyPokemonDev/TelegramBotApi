using Newtonsoft.Json;
using TelegramBotApi.Types.Markup;
using TelegramBotApi.Enums;

namespace TelegramBotApi.Types.Inline
{
    /// <summary>
    /// Represents a link to a voice recording in an .ogg container encoded with OPUS. 
    /// By default, this voice recording will be sent by the user. 
    /// Alternatively, you can use input_message_content to send a message with the specified content instead of the the voice message.
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class InlineQueryResultVoice : InlineQueryResult
    {
        private string _type = "voice";

        /// <summary>
        /// Type of the result, must be voice
        /// </summary>
        [JsonProperty(PropertyName = "type", Required = Required.Always)]
        public override string Type { get { return _type; } set { _type = "voice"; } }

        /// <summary>
        /// Unique identifier for this result, 1-64 Bytes
        /// </summary>
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public override string Id { get; set; }

        /// <summary>
        /// A valid URL for the voice recording
        /// </summary>
        [JsonProperty(PropertyName = "voice_url", Required = Required.Always)]
        public string VoiceUrl;

        /// <summary>
        /// Recording title
        /// </summary>
        [JsonProperty(PropertyName = "title", Required = Required.Always)]
        public string Title;

        /// <summary>
        /// Optional. Caption, 0-200 characters
        /// </summary>
        [JsonProperty(PropertyName = "caption")]
        public string Caption;

        /// <summary>
        /// Optional. Send Markdown or HTML, if you want Telegram apps to show bold, italic, fixed-width text or inline URLs in your bot's message.
        /// </summary>
        [JsonProperty(PropertyName = "parse_mode")]
        private string _parseMode = null;

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
        /// Optional. Recording duration in seconds
        /// </summary>
        [JsonProperty(PropertyName = "voice_duration")]
        public int VoiceDuration;

        /// <summary>
        /// Optional. Inline keyboard attached to the message
        /// </summary>
        [JsonProperty(PropertyName = "reply_markup")]
        public InlineKeyboardMarkup ReplyMarkup;

        /// <summary>
        /// Content of the message to be sent instead of the voice
        /// </summary>
        [JsonProperty(PropertyName = "input_message_content", Required = Required.Always)]
        public InputMessageContent InputMessageContent;
    }
}
