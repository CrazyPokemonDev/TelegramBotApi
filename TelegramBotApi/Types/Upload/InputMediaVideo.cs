using Newtonsoft.Json;

namespace TelegramBotApi.Types.Upload
{
    /// <summary>
    /// Represents a video to be sent
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class InputMediaVideo : InputMedia
    {
        /// <summary>
        /// This is a photo.
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        public override string Type { get => "photo"; }

        /// <summary>
        /// Optional. Video width
        /// </summary>
        [JsonProperty(PropertyName = "width")]
        public int Width { get; set; }

        /// <summary>
        /// Optional. Video height
        /// </summary>
        [JsonProperty(PropertyName = "height")]
        public int Height { get; set; }

        /// <summary>
        /// Optional. Video duration
        /// </summary>
        [JsonProperty(PropertyName = "duration")]
        public int Duration { get; set; }

        /// <summary>
        /// Optional. Pass true, if the video is suitable for streaming
        /// </summary>
        [JsonProperty(PropertyName = "supports_streaming")]
        public bool SupportsStreaming { get; set; }
    }
}
