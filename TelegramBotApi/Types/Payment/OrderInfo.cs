using Newtonsoft.Json;

namespace TelegramBotApi.Types.Payment
{
    /// <summary>
    /// This object represents information about an order.
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
    public class OrderInfo
    {
        /// <summary>
        /// Optional. User name
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Optional. User's phone number
        /// </summary>
        [JsonProperty(PropertyName = "phone_number")]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Optional. User email
        /// </summary>
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        /// <summary>
        /// Optional. User shipping address
        /// </summary>
        [JsonProperty(PropertyName = "shipping_address")]
        public ShippingAddress ShippingAddress { get; set; }
    }
}
