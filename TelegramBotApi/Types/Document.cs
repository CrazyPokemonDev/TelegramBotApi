using Newtonsoft.Json;

namespace TelegramBotApi.Types
{
    /// <summary>
    /// Represents a general file
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class Document
    {
        /// <summary>
        /// Unique identifier for this file
        /// </summary>
        [JsonProperty(PropertyName = "file_id", Required = Required.Always)]
        public string FileId;

        /// <summary>
        /// Optional. Document thumbnail as defined by sender
        /// </summary>
        [JsonProperty(PropertyName = "thumb")]
        public PhotoSize Thumb;

        /// <summary>
        /// Optional. Original filename as defined by sender
        /// </summary>
        [JsonProperty(PropertyName = "file_name")]
        public string FileName;

        /// <summary>
        /// Optional. MIME type of the file as defined by sender
        /// </summary>
        [JsonProperty(PropertyName = "mime_type")]
        public string MimeType;

        /// <summary>
        /// Optional. File size
        /// </summary>
        [JsonProperty(PropertyName = "file_size")]
        public int FileSize;
    }
}
