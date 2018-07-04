using Newtonsoft.Json;
using System;
using System.Linq;
using TelegramBotApi.Enums;
using Enum = TelegramBotApi.Enums.Enum;

namespace TelegramBotApi.Types
{
    /// <summary>
    /// Contains information about the current status of a webhook
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
    public class WebhookInfo
    {
        /// <summary>
        /// Webhook URL, may be empty if webhook is not set up
        /// </summary>
        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }

        /// <summary>
        /// True, if a custom certificate was provided for webhook certificate checks
        /// </summary>
        [JsonProperty(PropertyName = "has_custom_certificate")]
        public bool HasCustomCertificate { get; set; }

        /// <summary>
        /// Number of updates awaiting delivery
        /// </summary>
        [JsonProperty(PropertyName = "pending_update_count")]
        public int PendingUpdateCount { get; set; }

        [JsonProperty(PropertyName = "last_error_date")]
        private long _lastErrorDate { get; set; }

        /// <summary>
        /// Optional. Unix time for the most recent error that happened when trying to deliver an update via webhook
        /// </summary>
        public DateTime LastErrorDate
        {
            get { return DateTimeOffset.FromUnixTimeSeconds(_lastErrorDate).DateTime; }
            set { _lastErrorDate = ((DateTimeOffset)value).ToUnixTimeSeconds(); }
        }

        /// <summary>
        /// Optional. Error message in human-readable format for the most recent error that happened when trying to deliver an update via webhook
        /// </summary>
        [JsonProperty(PropertyName = "last_error_message")]
        public string LastErrorMessage { get; set; }

        /// <summary>
        /// Optional. Maximum allowed number of simultaneous HTTPS connections to the webhook for update delivery
        /// </summary>
        [JsonProperty(PropertyName = "max_connections")]
        public int MaxConnections { get; set; }

        [JsonProperty(PropertyName = "allowed_updates")]
        private string[] _allowedUpdates;
        /// <summary>
        /// Optional. A list of update types the bot is subscribed to. Defaults to all update types
        /// </summary>
        public UpdateType[] AllowedUpdates
        {
            get
            {
                return _allowedUpdates.Select(x => Enum.GetUpdateType(x)).ToArray();
            }
            set
            {
                _allowedUpdates = value.Select(x => Enum.GetString(x)).ToArray();
            }
        }
    }
}
