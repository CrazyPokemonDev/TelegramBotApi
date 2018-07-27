using Newtonsoft.Json;
using TelegramBotApi.Enums;

namespace TelegramBotApi.Types.TelegramPassport
{
    /// <summary>
    /// Represents an issue in one of the data fields that was provided by the user. 
    /// The error is considered resolved when the field's value changes.
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
    public class PassportElementErrorDataField : PassportElementError
    {
        private string _source = "data";

        /// <summary>
        /// Error source, must be data
        /// </summary>
        [JsonProperty(PropertyName = "source", Required = Required.Always)]
        public override string Source { get { return _source; } set { _source = "data"; } }

        [JsonProperty(PropertyName = "type", Required = Required.Always)]
        private string _type { get; set; }

        /// <summary>
        /// The section of the user's Telegram Passport which has the error, 
        /// one of “personal_details”, “passport”, “driver_license”, “identity_card”, “internal_passport”, “address”
        /// </summary>
        public override EncryptedPassportElementType Type { get { return Enum.GetEncryptedPassportElementType(_type); } set { _type = Enum.GetString(Type); } }

        /// <summary>
        /// Name of the data field which has the error
        /// </summary>
        [JsonProperty(PropertyName = "field_name", Required = Required.Always)]
        public string FieldName { get; set; }

        /// <summary>
        /// Base64-encoded data hash
        /// </summary>
        [JsonProperty(PropertyName = "data_hash", Required = Required.Always)]
        public string DataHash { get; set; }

        /// <summary>
        /// Error message
        /// </summary>
        [JsonProperty(PropertyName = "message", Required = Required.Always)]
        public override string Message { get; set; }
    }
}
