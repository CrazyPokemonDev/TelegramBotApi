using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotApi.Types.Payment
{
    /// <summary>
    /// This object represents information about an order.
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class OrderInfo
    {
        /// <summary>
        /// Optional. User name
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name;

        /// <summary>
        /// Optional. User's phone number
        /// </summary>
        [JsonProperty(PropertyName = "phone_number")]
        public string PhoneNumber;

        /// <summary>
        /// Optional. User email
        /// </summary>
        [JsonProperty(PropertyName = "email")]
        public string Email;

        /// <summary>
        /// Optional. User shipping address
        /// </summary>
        [JsonProperty(PropertyName = "shipping_address")]
        public ShippingAddress ShippingAddress;
    }
}
