using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotApi.Types
{
    /// <summary>
    /// Represents a response from the telegram bot API
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class ApiResponse<T>
    {
        /// <summary>
        /// The response object
        /// </summary>
        [JsonProperty(PropertyName = "result")]
        public T ResponseObject { get; set; }
        /// <summary>
        /// Whether the request was successful
        /// </summary>
        [JsonProperty(PropertyName = "ok", Required = Required.Always)]
        public bool Ok { get; set; }
    }
}
