using System.IO;

namespace TelegramBotApi.Types.Upload
{
    /// <summary>
    /// Represents a file to send via multipart/form data
    /// </summary>
    internal interface ISendFileMultipart
    {
        /// <summary>
        /// The file stream of this file
        /// </summary>
        Stream FileStream { get; set; }
    }
}
