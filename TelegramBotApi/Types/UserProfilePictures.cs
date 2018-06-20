using Newtonsoft.Json;

namespace TelegramBotApi.Types
{
    /// <summary>
    /// Represents a users profile pictures
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class UserProfilePictures
    {
        /// <summary>
        /// Total number of profile pictures the target user has
        /// </summary>
        [JsonProperty(PropertyName = "total_count")]
        public int TotalCount;

        /// <summary>
        /// Requested profile pictures (in up to 4 sizes each)
        /// </summary>
        [JsonProperty(PropertyName = "photos")]
        public PhotoSize[][] Photos;
    }
}
