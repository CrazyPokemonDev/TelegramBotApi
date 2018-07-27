using Newtonsoft.Json;
using TelegramBotApi.Enums;

namespace TelegramBotApi.Types.TelegramPassport
{
    /// <summary>
    /// Represents an issue with a document scan. The error is considered resolved when the file with the document scan changes.
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
    public class PassportElementErrorFile : PassportElementError
    {
        private string _source = "file";

        /// <summary>
        /// Error source, must be file
        /// </summary>
        [JsonProperty(PropertyName = "file", Required = Required.Always)]
        public override string Source { get { return _source; } set { _source = "file"; } }

        [JsonProperty(PropertyName = "type", Required = Required.Always)]
        private string _type { get; set; }

        /// <summary>
        /// The section of the user's Telegram Passport which has the issue, 
        /// one of “utility_bill”, “bank_statement”, “rental_agreement”, “passport_registration”, “temporary_registration”
        /// </summary>
        public override EncryptedPassportElementType Type { get { return Enum.GetEncryptedPassportElementType(_type); } set { _type = Enum.GetString(Type); } }

        /// <summary>
        /// Base64-encoded file hash
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
