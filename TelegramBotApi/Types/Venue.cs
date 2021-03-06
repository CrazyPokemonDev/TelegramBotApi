﻿using Newtonsoft.Json;

namespace TelegramBotApi.Types
{
    /// <summary>
    /// Represents a venue
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
    public class Venue
    {
        /// <summary>
        /// Venue location
        /// </summary>
        [JsonProperty(PropertyName = "location")]
        public Location Location { get; set; }

        /// <summary>
        /// Name of the venue
        /// </summary>
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        /// <summary>
        /// Address of the venue
        /// </summary>
        [JsonProperty(PropertyName = "address")]
        public string Address { get; set; }

        /// <summary>
        /// Optional. Foursquare identifier of the venue
        /// </summary>
        [JsonProperty(PropertyName = "foursquare_id")]
        public string FoursquareId { get; set; }

        /// <summary>
        /// Optional. Foursquare type of the venue. (For example, “arts_entertainment/default”, “arts_entertainment/aquarium” or “food/icecream”.)
        /// </summary>
        [JsonProperty(PropertyName = "foursquare_type")]
        public string FoursquareType { get; set; }
    }
}
