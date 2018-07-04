using Newtonsoft.Json;

namespace TelegramBotApi.Types.Inline
{
    /// <summary>
    /// Represents the content of a venue message to be sent as the result of an inline query.
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
    public class InputVenueMessageContent : InputMessageContent
    {
        /// <summary>
        /// Latitude of the location in degrees
        /// </summary>
        [JsonProperty(PropertyName = "latitude", Required = Required.Always)]
        public float Latitude { get; set; }

        /// <summary>
        /// Longitude of the location in degrees
        /// </summary>
        [JsonProperty(PropertyName = "longitude", Required = Required.Always)]
        public float Longitude { get; set; }

        /// <summary>
        /// Name of the venue
        /// </summary>
        [JsonProperty(PropertyName = "title", Required = Required.Always)]
        public string Title { get; set; }

        /// <summary>
        /// Address of the venue
        /// </summary>
        [JsonProperty(PropertyName = "address", Required = Required.Always)]
        public string Address { get; set; }

        /// <summary>
        /// Optional. Foursquare identifier of the venue, if known
        /// </summary>
        [JsonProperty(PropertyName = "foursquare_id")]
        public string FoursquareId { get; set; }
    }
}
