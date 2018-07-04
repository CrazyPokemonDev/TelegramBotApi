using Newtonsoft.Json;

namespace TelegramBotApi.Types
{
    /// <summary>
    /// This object represents a sticker set.
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
    public class StickerSet
    {
        /// <summary>
        /// Sticker set name
        /// </summary>
        [JsonProperty(PropertyName = "name", Required = Required.Always)]
        public string Name { get; set; }

        /// <summary>
        /// Sticker set title
        /// </summary>
        [JsonProperty(PropertyName = "title", Required = Required.Always)]
        public string Title { get; set; }

        /// <summary>
        /// True, if the sticker set contains masks
        /// </summary>
        [JsonProperty(PropertyName = "contain_masks", Required = Required.Always)]
        public bool ContainMasks { get; set; }

        /// <summary>
        /// List of all set stickers
        /// </summary>
        [JsonProperty(PropertyName = "stickers", Required = Required.Always)]
        public Sticker[] Stickers { get; set; }
    }
}
