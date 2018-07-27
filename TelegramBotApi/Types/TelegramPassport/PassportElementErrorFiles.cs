using Newtonsoft.Json;
using TelegramBotApi.Enums;

namespace TelegramBotApi.Types.TelegramPassport
{
    /// <summary>
    /// Represents an issue with a list of scans. The error is considered resolved when the list of files containing the scans changes.
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
    public class PassportElementErrorFiles : PassportElementError
    {
        private string _source = "files";

        /// <summary>
        /// Error source, must be files
        /// </summary>
        [JsonProperty(PropertyName = "files", Required = Required.Always)]
        public override string Source { get { return _source; } set { _source = "files"; } }

        [JsonProperty(PropertyName = "type", Required = Required.Always)]
        private string _type { get; set; }

        /// <summary>
        /// The section of the user's Telegram Passport which has the issue, 
        /// one of “utility_bill”, “bank_statement”, “rental_agreement”, “passport_registration”, “temporary_registration”
        /// </summary>
        public override EncryptedPassportElementType Type { get { return Enum.GetEncryptedPassportElementType(_type); } set { _type = Enum.GetString(Type); } }

        /// <summary>
        /// List of base64-encoded file hashes
        /// </summary>
        [JsonProperty(PropertyName = "file_hashes", Required = Required.Always)]
        public string[] FileHashes { get; set; }

        /// <summary>
        /// Error message
        /// </summary>
        [JsonProperty(PropertyName = "message", Required = Required.Always)]
        public override string Message { get; set; }
    }
}
