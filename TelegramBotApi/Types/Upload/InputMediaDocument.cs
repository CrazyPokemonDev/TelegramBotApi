using Newtonsoft.Json;

namespace TelegramBotApi.Types.Upload
{
    /// <summary>
    /// Represents a document to be sent
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
    public class InputMediaDocument : InputMedia
    {
        /// <summary>
        /// This is a document.
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        public override string Type { get => "document"; }

        /// <summary>
        /// Optional. Document thumbnail
        /// </summary>
        [JsonProperty(PropertyName = "thumb")]
        public SendFile Thumb { get; set; }
    }
}
