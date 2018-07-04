using Newtonsoft.Json;

namespace TelegramBotApi.Types.Upload
{
    /// <summary>
    /// Represents a photo to be sent
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
    public class InputMediaPhoto : InputMedia
    {
        /// <summary>
        /// This is a photo.
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        public override string Type { get => "photo"; }
    }
}
