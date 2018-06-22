using Newtonsoft.Json;
using System;
using TelegramBotApi.Enums;
using Enum = TelegramBotApi.Enums.Enum;

namespace TelegramBotApi.Types
{
    /// <summary>
    /// Represents a member of a chat
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class ChatMember
    {
        /// <summary>
        /// Information about the user
        /// </summary>
        [JsonProperty(PropertyName = "user")]
        public User User { get; set; }

        [JsonProperty(PropertyName = "status")]
        private string _status;
        /// <summary>
        /// Member's status in the chat
        /// </summary>
        public ChatMemberStatus Status
        {
            get
            {
                return Enum.GetChatMemberStatus(_status);
            }
            set
            {
                _status = Enum.GetString(value);
            }
        }

        [JsonProperty(PropertyName = "until_date")]
        private long _untilDate;
        /// <summary>
        /// Optional. Restricted and kicked only. Date when restrictions will be lifted for this user, unix time
        /// </summary>
        public DateTime UntilDate
        {
            get
            {
                return DateTimeOffset.FromUnixTimeSeconds(_untilDate).DateTime;
            }
            set
            {
                _untilDate = ((DateTimeOffset)value).ToUnixTimeSeconds();
            }
        }

        /// <summary>
        /// Administrators only. True, if the bot is allowed to edit administrator privileges of that user
        /// </summary>
        [JsonProperty(PropertyName = "can_be_edited")]
        public bool CanBeEdited { get; set; } = false;

        /// <summary>
        /// Administrators only. True, if the administrator can change the chat title, photo and other settings
        /// </summary>
        [JsonProperty(PropertyName = "can_change_info")]
        public bool CanChangeInfo { get; set; } = false;

        /// <summary>
        /// Administrators only. True, if the administrator can post in the channel, channels only
        /// </summary>
        [JsonProperty(PropertyName = "can_post_messages")]
        public bool CanPostMessages { get; set; } = false;

        /// <summary>
        /// Administrators only. True, if the administrator can edit messages of other users and can pin messages, channels only
        /// </summary>
        [JsonProperty(PropertyName = "can_edit_messages")]
        public bool CanEditMessages { get; set; } = false;

        /// <summary>
        /// Administrators only. True, if the administrator can delete messages of other users
        /// </summary>
        [JsonProperty(PropertyName = "can_delete_messages")]
        public bool CanDeleteMessages { get; set; } = false;

        /// <summary>
        /// Administrators only. True, if the administrator can invite new users to the chat
        /// </summary>
        [JsonProperty(PropertyName = "can_invite_users")]
        public bool CanInviteUsers { get; set; } = false;

        /// <summary>
        /// Administrators only. True, if the administrator can restrict, ban or unban chat members
        /// </summary>
        [JsonProperty(PropertyName = "can_restrict_users")]
        public bool CanRestrictMembers { get; set; } = false;

        /// <summary>
        /// Administrators only. True, if the administrator can pin messages, supergroups only
        /// </summary>
        [JsonProperty(PropertyName = "can_pin_messages")]
        public bool CanPinMessages { get; set; } = false;

        /// <summary>
        /// Administrators only. True, if the administrator can add new administrators with a subset of his own privileges
        /// or demote administrators that he has promoted, directly or indirectly
        /// (promoted by administrators that were appointed by the user)
        /// </summary>
        [JsonProperty(PropertyName = "can_promote_members")]
        public bool CanPromoteMembers { get; set; } = false;

        /// <summary>
        /// Restricted only. True, if the user can send text messages, contacts, locations and venues
        /// </summary>
        [JsonProperty(PropertyName = "can_send_messages")]
        public bool CanSendMessages { get; set; } = false;

        /// <summary>
        /// Restricted only. True, if the user can send audios, documents, photos, videos, 
        /// video notes and voice notes, implies CanSendMessages
        /// </summary>
        [JsonProperty(PropertyName = "can_send_media_messages")]
        public bool CanSendMediaMessages { get; set; } = false;

        /// <summary>
        /// Restricted only. True, if the user can send animations, games, stickers and use inline bots, implies CanSendMediaMessages
        /// </summary>
        [JsonProperty(PropertyName = "can_send_other_messages")]
        public bool CanSendOtherMessages { get; set; } = false;

        /// <summary>
        /// Restricted only. True, if user may add web page previews to his messages, implies CanSendMediaMessages
        /// </summary>
        [JsonProperty(PropertyName = "can_add_web_page_previews")]
        public bool CanAddWebPagePreviews { get; set; } = false;
    }
}
