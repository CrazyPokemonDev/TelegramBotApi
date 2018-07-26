using Newtonsoft.Json;

namespace TelegramBotApi.Types.Upload
{
    /// <summary>
    /// Represents an audio to be sent
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
    public class InputMediaAudio : InputMedia
    {
        /// <summary>
        /// This is an audio.
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        public override string Type { get => "audio"; }

        /// <summary>
        /// Optional. Audio thumbnail
        /// </summary>
        [JsonProperty(PropertyName = "thumb")]
        public SendFile Thumb { get; set; }

        /// <summary>
        /// Optional. Audio duration
        /// </summary>
        [JsonProperty(PropertyName = "duration")]
        public int Duration { get; set; }

        /// <summary>
        /// Optional. Performer of the audio
        /// </summary>
        [JsonProperty(PropertyName = "performer")]
        public string Performer { get; set; }

        /// <summary>
        /// Optional. Title of the audio
        /// </summary>
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }
    }
}
