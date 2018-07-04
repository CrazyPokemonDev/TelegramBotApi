using Newtonsoft.Json;

namespace TelegramBotApi.Types
{
    /// <summary>
    /// This object represents a point on the map.
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
    public class Location
    {
        /// <summary>
        /// Longitude as defined by sender
        /// </summary>
        [JsonProperty(PropertyName = "longtitude", Required = Required.Always)]
        public float Longtitude { get; set; }

        /// <summary>
        /// Latitude as defined by sender
        /// </summary>
        [JsonProperty(PropertyName = "latitude", Required = Required.Always)]
        public float Latitude { get; set; }
    }
}
