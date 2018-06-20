using Newtonsoft.Json;

namespace TelegramBotApi.Types
{
    /// <summary>
    /// The response parameters that might be given by telegram on a failed request
    /// </summary>
    [JsonObject]
    public class ResponseParameters
    {
        /// <summary>
        /// If this isn't 0, the chat has been migrated to this id
        /// </summary>
        [JsonProperty(PropertyName = "migrate_to_chat_id")]
        public long MigrateToChatId = 0;

        /// <summary>
        /// If this isn't 0, the number of seconds to wait before the request can be repeated
        /// </summary>
        [JsonProperty(PropertyName = "retry_after")]
        public int RetryAfter = 0;
    }
}
