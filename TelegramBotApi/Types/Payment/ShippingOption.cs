using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotApi.Types.Payment
{
    /// <summary>
    /// This object contains information about an incoming pre-checkout query.
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class ShippingOption
    {
        /// <summary>
        /// Shipping option identifier
        /// </summary>
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public string Id;

        /// <summary>
        /// Option title
        /// </summary>
        [JsonProperty(PropertyName = "title", Required = Required.Always)]
        public string Title;

        /// <summary>
        /// List of price portions
        /// </summary>
        [JsonProperty(PropertyName = "prices, Required = Required.Always")]
        public LabeledPrice[] Prices;
    }
}
