using Newtonsoft.Json;
using TelegramBotApi.Enums;

namespace TelegramBotApi.Types.Upload
{
    /// <summary>
    /// Represents a photo or video to be sent
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public abstract class InputMedia
    {
        /// <summary>
        /// The type of media
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        public abstract string Type { get; }

        /// <summary>
        /// File to send. Can be <see cref="SendFileAttach"/>, <see cref="SendFileId"/> or <see cref="SendFileUrl"/>
        /// </summary>
        [JsonProperty(PropertyName = "media")]
        public SendFile Media;

        /// <summary>
        /// Optional. Caption of the photo or video to be sent, 0-200 characters.
        /// </summary>
        [JsonProperty(PropertyName = "caption", NullValueHandling = NullValueHandling.Ignore)]
        public string Caption;

        [JsonProperty(PropertyName = "parse_mode")]
        private string _parseMode;
        /// <summary>
        /// Parsing mode of the caption, if any
        /// </summary>
        public ParseMode ParseMode
        {
            get
            {
                switch (_parseMode.ToLower())
                {
                    case "markdown":
                        return ParseMode.Markdown;
                    case "html":
                        return ParseMode.Html;
                    default:
                        return ParseMode.None;
                }
            }
            set
            {
                switch (value)
                {
                    case ParseMode.Markdown:
                        _parseMode = "Markdown";
                        break;
                    case ParseMode.Html:
                        _parseMode = "Html";
                        break;
                    default:
                        _parseMode = null;
                        break;
                }
            }
        }
    }
}
