using Newtonsoft.Json;
using TelegramBotApi.Enums;

namespace TelegramBotApi.Types.TelegramPassport
{
    /// <summary>
    /// Represents an issue with the selfie with a document. The error is considered resolved when the file with the selfie changes.
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
    public class PassportElementErrorSelfie : PassportElementError
    {
        private string _source = "selfie";

        /// <summary>
        /// Error source, must be selfie
        /// </summary>
        [JsonProperty(PropertyName = "source", Required = Required.Always)]
        public override string Source { get { return _source; } set { _source = "selfie"; } }

        [JsonProperty(PropertyName = "type", Required = Required.Always)]
        private string _type { get; set; }

        /// <summary>
        /// The section of the user's Telegram Passport which has the issue, 
        /// one of “passport”, “driver_license”, “identity_card”, “internal_passport”
        /// </summary>
        public override EncryptedPassportElementType Type { get { return Enum.GetEncryptedPassportElementType(_type); } set { _type = Enum.GetString(Type); } }

        /// <summary>
        /// Base64-encoded hash of the file with the selfie
        /// </summary>
        [JsonProperty(PropertyName = "file_hash", Required = Required.Always)]
        public string FileHash { get; set; }

        /// <summary>
        /// Error message
        /// </summary>
        [JsonProperty(PropertyName = "message", Required = Required.Always)]
        public override string Message { get; set; }
    }
}
