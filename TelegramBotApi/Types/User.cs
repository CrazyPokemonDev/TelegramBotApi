using Newtonsoft.Json;

namespace TelegramBotApi.Types
{
    /// <summary>
    /// This object represents a Telegram user or bot.
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
    public class User
    {
        /// <summary>
        /// Unique identifier for this user or bot
        /// </summary>
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public int Id { get; set; }

        /// <summary>
        /// Whether the user is a bot
        /// </summary>
        [JsonProperty(PropertyName = "is_bot")]
        public bool IsBot { get; set; } = false;

        /// <summary>
        /// The first name of the user or bot
        /// </summary>
        [JsonProperty(PropertyName = "first_name", Required = Required.Always)]
        public string FirstName { get; set; }

        /// <summary>
        /// The last name of the user or bot, if any
        /// </summary>
        [JsonProperty(PropertyName = "last_name")]
        public string LastName { get; set; }

        /// <summary>
        /// First and last name (if any) concatenated using a space
        /// </summary>
        public string FullName => (FirstName + " " + LastName).Trim();

        /// <summary>
        /// The username of the user or bot, if any. Without the preceding "@".
        /// </summary>
        [JsonProperty(PropertyName = "username")]
        public string Username { get; set; }

        /// <summary>
        /// IEFT tag of the users language, if any
        /// </summary>
        [JsonProperty(PropertyName = "language_code")]
        public string LanguageCode { get; set; }
    }
}
