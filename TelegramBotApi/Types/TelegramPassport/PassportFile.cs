using Newtonsoft.Json;
using System;

namespace TelegramBotApi.Types.TelegramPassport
{
    /// <summary>
    /// This object represents a file uploaded to Telegram Passport. 
    /// Currently all Telegram Passport files are in JPEG format when decrypted and don't exceed 10MB.
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
    public class PassportFile
    {
        /// <summary>
        /// Unique identifier for this file
        /// </summary>
        [JsonProperty(PropertyName = "file_id", Required = Required.Always)]
        public string FileId { get; set; }

        /// <summary>
        /// File size
        /// </summary>
        [JsonProperty(PropertyName = "file_size", Required = Required.Always)]
        public int FileSize { get; set; }

        [JsonProperty(PropertyName = "date", Required = Required.Always)]
        private long _date;
        /// <summary>
        /// Unix time when the file was uploaded
        /// </summary>
        public DateTime Date
        {
            get
            {
                return DateTimeOffset.FromUnixTimeSeconds(_date).DateTime;
            }
            set
            {
                _date = ((DateTimeOffset)value).ToUnixTimeSeconds();
            }
        }
    }
}
