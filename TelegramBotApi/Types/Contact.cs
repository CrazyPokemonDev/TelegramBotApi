using Newtonsoft.Json;

namespace TelegramBotApi.Types
{
    /// <summary>
    /// Represents a phone contact
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class Contact
    {
        /// <summary>
        /// Contact's phone number
        /// </summary>
        [JsonProperty(PropertyName = "phone_number")]
        public string PhoneNumber;

        /// <summary>
        /// Contact's first name
        /// </summary>
        [JsonProperty(PropertyName = "first_name")]
        public string FirstName;

        /// <summary>
        /// Optional. Contact's last name
        /// </summary>
        [JsonProperty(PropertyName = "last_name")]
        public string LastName;

        /// <summary>
        /// Optional. Contact's user id on Telegram
        /// </summary>
        [JsonProperty(PropertyName = "user_id")]
        public int UserId;
    }
}
