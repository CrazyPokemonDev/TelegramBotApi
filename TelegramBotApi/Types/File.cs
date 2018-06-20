using Newtonsoft.Json;

namespace TelegramBotApi.Types
{
    /// <summary>
    /// Represents a file ready to be downloaded. Valid for at least one hour.
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class File
    {
        /// <summary>
        /// Unique identifier for this file
        /// </summary>
        [JsonProperty(PropertyName = "file_id")]
        public string FileId;

        /// <summary>
        /// File size, if known
        /// </summary>
        [JsonProperty(PropertyName = "file_size")]
        public int FileSize;

        /// <summary>
        /// File path. Use TelegramBot.DownloadFile(<see cref="FilePath"/>) to download the file.
        /// </summary>
        [JsonProperty(PropertyName = "file_path")]
        public string FilePath;
    }
}
