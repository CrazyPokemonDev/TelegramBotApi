﻿using Newtonsoft.Json;

namespace TelegramBotApi.Types.Payment
{
    /// <summary>
    /// This object contains basic information about an invoice.
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class Invoice
    {
        /// <summary>
        /// Product name
        /// </summary>
        [JsonProperty(PropertyName = "title", Required = Required.Always)]
        public string Title { get; set; }

        /// <summary>
        /// Product description
        /// </summary>
        [JsonProperty(PropertyName = "description", Required = Required.Always)]
        public string Description { get; set; }

        /// <summary>
        /// Unique bot deep-linking parameter that can be used to generate this invoice
        /// </summary>
        [JsonProperty(PropertyName = "start_parameter", Required = Required.Always)]
        public string StartParameter { get; set; }

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
    }
}
