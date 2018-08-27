using Newtonsoft.Json;
using TelegramBotApi.Enums;

namespace TelegramBotApi.Types.TelegramPassport
{
    /// <summary>
    /// Represents an issue with the translated version of a document. The error is considered resolved when a file with the document translation change.
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
    public class PassportElementErrorTranslationFiles : PassportElementError
    {
        private string _source = "translation_files";

        /// <summary>
        /// Error source, must be translation_files
        /// </summary>
        [JsonProperty(PropertyName = "source", Required = Required.Always)]
        public override string Source { get { return _source; } set { _source = "translation_files"; } }

        [JsonProperty(PropertyName = "type", Required = Required.Always)]
        private string _type { get; set; }

        /// <summary>
        /// Type of element of the user's Telegram Passport which has the issue, one of “passport”, “driver_license”, 
        /// “identity_card”, “internal_passport”, “utility_bill”, “bank_statement”, “rental_agreement”, 
        /// “passport_registration”, “temporary_registration”
        /// </summary>
        public override EncryptedPassportElementType Type { get { return Enum.GetEncryptedPassportElementType(_type); } set { _type = Enum.GetString(Type); } }

        /// <summary>
        /// List of Base64-encoded file hashes
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
