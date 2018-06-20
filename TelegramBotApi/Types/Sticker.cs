using Newtonsoft.Json;

namespace TelegramBotApi.Types
{
    /// <summary>
    /// This object represents a sticker.
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class Sticker
    {
        /// <summary>
        /// Unique identifier for this file
        /// </summary>
        [JsonProperty(PropertyName = "file_id", Required = Required.Always)]
        public string FileId;

        /// <summary>
        /// Sticker width
        /// </summary>
        [JsonProperty(PropertyName = "width", Required = Required.Always)]
        public int Width;

        /// <summary>
        /// Sticker height
        /// </summary>
        [JsonProperty(PropertyName = "height", Required = Required.Always)]
        public int Height;

        /// <summary>
        /// Optional. Sticker thumbnail in the .webp or .jpg format
        /// </summary>
        [JsonProperty(PropertyName = "thumb")]
        public PhotoSize Thumb;

        /// <summary>
        /// Optional. Emoji associated with the sticker
        /// </summary>
        [JsonProperty(PropertyName = "emoji")]
        public string Emoji;

        /// <summary>
        /// Optional. Name of the sticker set to which the sticker belongs
        /// </summary>
        [JsonProperty(PropertyName = "set_name")]
        public string SetName;

        /// <summary>
        /// Optional. For mask stickers, the position where the mask should be placed
        /// </summary>
        [JsonProperty(PropertyName = "mask_position")]
        public MaskPosition MaskPosition;

        /// <summary>
        /// Optional. File size
        /// </summary>
        [JsonProperty(PropertyName = "file_size")]
        public int FileSize;
    }
}
