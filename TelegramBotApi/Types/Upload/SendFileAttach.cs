using System.IO;

namespace TelegramBotApi.Types.Upload
{
    /// <summary>
    /// A file to send as attached multipart/form data
    /// </summary>
    public class SendFileAttach : SendFile
    {
        /// <summary>
        /// The file stream of this file
        /// </summary>
        public Stream FileStream { get; }

        /// <summary>
        /// This is an attached media for media groups.
        /// </summary>
        public override SendFileType Type => SendFileType.Attach;

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

        internal string AttachName { get; set; }
    }
}
