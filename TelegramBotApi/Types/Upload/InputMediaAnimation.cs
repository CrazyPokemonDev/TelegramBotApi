using Newtonsoft.Json;

namespace TelegramBotApi.Types.Upload
{
    /// <summary>
    /// Represents an animation to be sent
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
    public class InputMediaAnimation : InputMedia
    {
        /// <summary>
        /// This is an animation.
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        public override string Type { get => "animation"; }

        /// <summary>
        /// Optional. Animation thumbnail
        /// </summary>
        [JsonProperty(PropertyName = "thumb")]
        public SendFile Thumb { get; set; }

        /// <summary>
        /// Optional. Animation width
        /// </summary>
        [JsonProperty(PropertyName = "width")]
        public int Width { get; set; }

        /// <summary>
        /// Optional. Animation height
        /// </summary>
        [JsonProperty(PropertyName = "height")]
        public int Height { get; set; }

        /// <summary>
        /// Optional. Animation duration
        /// </summary>
        [JsonProperty(PropertyName = "duration")]
        public int Duration { get; set; }
    }
}
