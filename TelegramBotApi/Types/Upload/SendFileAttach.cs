using System.IO;

namespace TelegramBotApi.Types.Upload
{
    /// <summary>
    /// A file to send as attached multipart/form data
    /// </summary>
    public class SendFileAttach : SendFile, ISendFileMultipart
    {
        /// <summary>
        /// This is an attached file
        /// </summary>
        public override SendFileType Type => SendFileType.Attach;

        internal const string AttachName = "attachedfile";

        /// <summary>
        /// The file stream of this file
        /// </summary>
        public Stream FileStream { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SendFileAttach"/> class
        /// </summary>
        /// <param name="filename">The path of the file to upload</param>
        public SendFileAttach(string filename)
        {
            FileStream = System.IO.File.OpenRead(filename);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SendFileAttach"/> class
        /// </summary>
        /// <param name="fileStream">The file stream of the file to upload</param>
        public SendFileAttach(Stream fileStream)
        {
            FileStream = fileStream;
        }
    }
}
