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
        public string Id;

        /// <summary>
        /// User who sent the query
        /// </summary>
        [JsonProperty(PropertyName = "from", Required = Required.Always)]
        public User From;

        /// <summary>
        /// Bot specified invoice payload
        /// </summary>
        [JsonProperty(PropertyName = "invoice_payload")]
        public string InvoicePayload;

        /// <summary>
        /// User specified shipping address
        /// </summary>
        [JsonProperty(PropertyName = "shipping_address")]
        public ShippingAddress ShippingAddress;
    }
}
