using Newtonsoft.Json;
using System;
using TelegramBotApi.Enums;
using Enum = TelegramBotApi.Enums.Enum;

namespace TelegramBotApi.Types.Upload
{
    /// <summary>
    /// Represents a photo or video to be sent
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
    public abstract class InputMedia
    {
        /// <summary>
        /// The type of media
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        public abstract string Type { get; }

        [JsonProperty(PropertyName = "media")]
        private string _media;
        private SendFile _mediaFile;
        /// <summary>
        /// File to send. Can be <see cref="SendFileAttach"/>, <see cref="SendFileId"/> or <see cref="SendFileUrl"/>
        /// </summary>
        public SendFile Media
        {
            get
            {
                return _mediaFile;
            }
            set
            {
                switch (value.Type)
                {
                    case SendFileType.Attach:
                        Guid guid = Guid.NewGuid();
                        _media = guid.ToString();
                        _mediaFile = value;
                        ((SendFileAttach)_mediaFile).AttachName = guid.ToString();
                        break;
                    case SendFileType.FileId:
                        _media = ((SendFileId)value).FileId;
                        _mediaFile = value;
                        break;
                    case SendFileType.Url:
                        _media = ((SendFileUrl)value).Url;
                        _mediaFile = value;
                        break;
                    case SendFileType.Multipart:
                        throw new Exception("InputMedia.Media cannot be of type SendFileMultipart");
                }
            }
        }

        /// <summary>
        /// Optional. Caption of the photo or video to be sent, 0-200 characters.
        /// </summary>
        [JsonProperty(PropertyName = "caption", NullValueHandling = NullValueHandling.Ignore)]
        public string Caption { get; set; }

        [JsonProperty(PropertyName = "parse_mode")]
        private string _parseMode;
        /// <summary>
        /// Parsing mode of the caption, if any
        /// </summary>
        public ParseMode ParseMode
        {
            get
            {
                return Enum.GetParseMode(_parseMode);
            }
            set
            {
                _parseMode = Enum.GetString(value);
            }
        }
    }
}
