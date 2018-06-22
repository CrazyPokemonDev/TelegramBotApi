namespace TelegramBotApi.Types.Upload
{
    /// <summary>
    /// A file that has already been uploaded to telegram to be sent using a telegram file identifier
    /// </summary>

    public class SendFileId : SendFile
    {
        /// <summary>
        /// This sends a file via file id
        /// </summary>
        public override SendFileType Type => SendFileType.FileId;

        /// <summary>
        /// Telegrams unique identifier of the file to send
        /// </summary>
        public string FileId { get; set; }

        /// <summary>
        /// Initializes a new instance of the SendFileId class
        /// </summary>
        /// <param name="fileId">The telegram identifier of the file to send</param>
        public SendFileId(string fileId)
        {
            FileId = fileId;
        }
    }
}
