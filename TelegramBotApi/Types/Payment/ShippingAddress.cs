using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotApi.Types.Payment
{
    /// <summary>
    /// This object represents a shipping address.
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class ShippingAddress
    {
        /// <summary>
        /// ISO 3166-1 alpha-2 country code
        /// </summary>
        [JsonProperty(PropertyName = "country_code", Required = Required.Always)]
        public string CountryCode;

        /// <summary>
        /// State, if applicable
        /// </summary>
        [JsonProperty(PropertyName = "state")]
        public string State;

        /// <summary>
        /// City
        /// </summary>
        [JsonProperty(PropertyName = "city", Required = Required.Always)]
        public string City;

        /// <summary>
        /// First line for the address
        /// </summary>
        [JsonProperty(PropertyName = "street_line1", Required = Required.Always)]
        public string StreetLine1;

        /// <summary>
        /// Second line for the address
        /// </summary>
        [JsonProperty(PropertyName = "street_line2", Required = Required.Always)]
        public string StreetLine2;

        /// <summary>
        /// Address post code
        /// </summary>
        [JsonProperty(PropertyName = "post_code", Required = Required.Always)]
        public string PostCode;
    }
}
