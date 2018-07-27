using Newtonsoft.Json;

namespace TelegramBotApi.Types.TelegramPassport
{
    /// <summary>
    /// This object contains information about Telegram Passport data shared with the bot by the user.
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
    public class PassportData
    {
        /// <summary>
        /// Array with information about documents and other Telegram Passport elements that was shared with the bot
        /// </summary>
        [JsonProperty(PropertyName = "data", Required = Required.Always)]
        public EncryptedPassportElement[] Data { get; set; }

        /// <summary>
        /// Encrypted credentials required to decrypt the data
        /// </summary>
        [JsonProperty(PropertyName = "credentials", Required = Required.Always)]
        public EncryptedCredentials Credentials { get; set; }
    }
}
