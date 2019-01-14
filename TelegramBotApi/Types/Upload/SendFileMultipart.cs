using System.IO;

namespace TelegramBotApi.Types.Upload
{
    /// <summary>
    /// Represents a file to be sent via multipart upload (a file that is present on your device)
    /// </summary>
    public class SendFileMultipart : SendFile
    {
        /// <summary>
        /// This file is to be sent via multipart/form data
        /// </summary>
        public override SendFileType Type => SendFileType.Multipart;

        /// <summary>
        /// The file stream associated with this file, if any
        /// </summary>
        public Stream FileStream { get; }

        /// <summary>
        /// The path associated with this file, if any
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// The file name
        /// </summary>
        public string FileName { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SendFileMultipart"/> class
        /// </summary>
        /// <param name="path">The path of the file to send</param>
        /// <param name="fileName">The name of the file to send</param>
        public SendFileMultipart(string path, string fileName = null)
        {
            Path = path;
            FileName = fileName ?? System.IO.Path.GetFileName(Path);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SendFileMultipart"/> class
        /// </summary>
        /// <param name="fileStream">The stream of the file to send</param>
        /// <param name="fileName">The name of the file to send</param>
        public SendFileMultipart(Stream fileStream, string fileName)
        {
            FileStream = fileStream;
            FileName = fileName;
        }
    }
}
