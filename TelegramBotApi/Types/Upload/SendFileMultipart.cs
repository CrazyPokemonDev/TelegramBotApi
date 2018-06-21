using System.IO;

namespace TelegramBotApi.Types.Upload
{
    /// <summary>
    /// Represents a file to be sent via multipart upload
    /// </summary>
    public class SendFileMultipart : SendFile, ISendFileMultipart
    {
        /// <summary>
        /// This file is to be sent via multipart/form data
        /// </summary>
        public override SendFileType Type => SendFileType.Multipart;

        /// <summary>
        /// The file stream associated with this file
        /// </summary>
        public Stream FileStream { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SendFileMultipart"/> class
        /// </summary>
        /// <param name="path">The path of the file to send</param>
        public SendFileMultipart(string path)
        {
            FileStream = System.IO.File.OpenRead(path);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SendFileMultipart"/> class
        /// </summary>
        /// <param name="fileStream">The stream of the file to send</param>
        public SendFileMultipart(Stream fileStream)
        {
            FileStream = fileStream;
        }
    }
}
