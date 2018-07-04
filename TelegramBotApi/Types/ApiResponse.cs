using Newtonsoft.Json;

namespace TelegramBotApi.Types
{
    /// <summary>
    /// Represents a response from the telegram bot API
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
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

        /// <summary>
        /// If the request has failed, information about why it did fail
        /// </summary>
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        /// <summary>
        /// Might help to automatically handle the error
        /// </summary>
        [JsonProperty(PropertyName = "parameters")]
        public ResponseParameters Parameters { get; set; }
    }
}
