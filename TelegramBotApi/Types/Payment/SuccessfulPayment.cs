using Newtonsoft.Json;

namespace TelegramBotApi.Types.Payment
{
    /// <summary>
    /// This object contains basic information about a successful payment.
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
    public class SuccessfulPayment
    {
        /// <summary>
        /// Three-letter ISO 4217 currency code
        /// </summary>
        [JsonProperty(PropertyName = "currency", Required = Required.Always)]
        public string Currency { get; set; }

        /// <summary>
        /// Total price in the smallest units of the currency (integer, not float/double). 
        /// For example, for a price of US$ 1.45 pass amount = 145. See the exp parameter in currencies.json, 
        /// it shows the number of digits past the decimal point for each currency (2 for the majority of currencies).
        /// </summary>
        [JsonProperty(PropertyName = "total_amount", Required = Required.Always)]
        public int TotalAmount { get; set; }

        /// <summary>
        /// Bot specified invoice payload
        /// </summary>
        [JsonProperty(PropertyName = "invoice_payload", Required = Required.Always)]
        public string InvoicePayload { get; set; }

        /// <summary>
        /// Optional. Identifier of the shipping option chosen by the user
        /// </summary>
        [JsonProperty(PropertyName = "shipping_option_id")]
        public string ShippingOptionId { get; set; }

        /// <summary>
        /// Optional. Order info provided by the user
        /// </summary>
        [JsonProperty(PropertyName = "order_info", Required = Required.Always)]
        public OrderInfo OrderInfo { get; set; }

        /// <summary>
        /// Telegram payment identifier
        /// </summary>
        [JsonProperty(PropertyName = "telegram_payment_charge_id", Required = Required.Always)]
        public string TelegramPaymentChargeId { get; set; }

        /// <summary>
        /// Provider payment identifier
        /// </summary>
        [JsonProperty(PropertyName = "provider_payment_charge_id", Required = Required.Always)]
        public string ProviderPaymentChargeId { get; set; }
    }
}
