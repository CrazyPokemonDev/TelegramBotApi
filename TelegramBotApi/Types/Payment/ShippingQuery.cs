using Newtonsoft.Json;

namespace TelegramBotApi.Types.Payment
{
    /// <summary>
    /// This object contains information about an incoming shipping query.
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class ShippingQuery
    {
        /// <summary>
        /// Unique query identifier
        /// </summary>
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public string Id { get; set; }

        /// <summary>
        /// User who sent the query
        /// </summary>
        [JsonProperty(PropertyName = "from", Required = Required.Always)]
        public User From { get; set; }

        /// <summary>
        /// Bot specified invoice payload
        /// </summary>
        [JsonProperty(PropertyName = "invoice_payload")]
        public string InvoicePayload { get; set; }

        /// <summary>
        /// User specified shipping address
        /// </summary>
        [JsonProperty(PropertyName = "shipping_address")]
        public ShippingAddress ShippingAddress { get; set; }
    }
}
