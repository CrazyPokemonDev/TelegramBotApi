using Newtonsoft.Json;
using TelegramBotApi.Types.Markup;

namespace TelegramBotApi.Types.Inline
{
    /// <summary>
    /// Represents a location on a map. By default, the location will be sent by the user. 
    /// Alternatively, you can use input_message_content to send a message with the specified content instead of the location.
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
    public class InlineQueryResultVenue : InlineQueryResult
    {
        private string _type = "venue";

        /// <summary>
        /// Type of the result, must be venue
        /// </summary>
        [JsonProperty(PropertyName = "type", Required = Required.Always)]
        public override string Type { get { return _type; } set { _type = "venue"; } }

        /// <summary>
        /// Unique identifier for this result, 1-64 Bytes
        /// </summary>
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public override string Id { get; set; }

        /// <summary>
        /// Latitude of the venue location in degrees
        /// </summary>
        [JsonProperty(PropertyName = "latitude", Required = Required.Always)]
        public float Latitude { get; set; }

        /// <summary>
        /// Longitude of the venue location in degrees
        /// </summary>
        [JsonProperty(PropertyName = "longitude", Required = Required.Always)]
        public float Longitude { get; set; }

        /// <summary>
        /// Location title
        /// </summary>
        [JsonProperty(PropertyName = "title", Required = Required.Always)]
        public string Title { get; set; }

        /// <summary>
        /// Address of the venue
        /// </summary>
        [JsonProperty(PropertyName = "address", Required = Required.Always)]
        public string Address { get; set; }

        /// <summary>
        /// Optional. Foursquare identifier of the venue if known
        /// </summary>
        [JsonProperty(PropertyName = "foursquare_id")]
        public string FoursquareId { get; set; }

        /// <summary>
        /// Optional. Inline keyboard attached to the message
        /// </summary>
        [JsonProperty(PropertyName = "reply_markup")]
        public InlineKeyboardMarkup ReplyMarkup { get; set; }

        /// <summary>
        /// Content of the message to be sent instead of the venue
        /// </summary>
        [JsonProperty(PropertyName = "input_message_content", Required = Required.Always)]
        public InputMessageContent InputMessageContent { get; set; }

        /// <summary>
        /// Optional. Url of the thumbnail for the result
        /// </summary>
        [JsonProperty(PropertyName = "thumb_url", Required = Required.Always)]
        public string ThumbUrl { get; set; }

        /// <summary>
        /// Optional. Thumbnail width
        /// </summary>
        [JsonProperty(PropertyName = "thumb_width")]
        public int ThumbWidth { get; set; }

        /// <summary>
        /// Optional. Thumbnail height
        /// </summary>
        [JsonProperty(PropertyName = "thumb_height")]
        public int ThumbHeight { get; set; }
    }
}
