using Newtonsoft.Json;

namespace TelegramBotApi.Types.Payment
{
    /// <summary>
    /// This object represents a portion of the price for goods or services.
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class LabeledPrice
    {
        /// <summary>
        /// Portion label
        /// </summary>
        [JsonProperty(PropertyName = "label", Required = Required.Always)]
        public string Label;

        /// <summary>
        /// Price of the product in the smallest units of the currency (integer, not float/double). 
        /// For example, for a price of US$ 1.45 pass amount = 145. See the exp parameter in currencies.json, 
        /// it shows the number of digits past the decimal point for each currency (2 for the majority of currencies).
        /// </summary>
        [JsonProperty(PropertyName = "amount", Required = Required.Always)]
        public int Amount;
    }
}
