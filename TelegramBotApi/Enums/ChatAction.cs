namespace TelegramBotApi.Enums
{
    /// <summary>
    /// Type of a chat action to be shown in the status bar below the bot's name
    /// </summary>
    public enum ChatAction
    {
        /// <summary>
        /// Sending a text message
        /// </summary>
        Typing,
        /// <summary>
        /// Uploading a photo
        /// </summary>
        UploadPhoto,
        /// <summary>
        /// Recording a video
        /// </summary>
        RecordVideo,
        /// <summary>
        /// Uploading a video
        /// </summary>
        UploadVideo,
        /// <summary>
        /// Recording a voice note
        /// </summary>
        RecordAudio,
        /// <summary>
        /// Uploading a voice note
        /// </summary>
        UploadAudio,
        /// <summary>
        /// Uploading a document
        /// </summary>
        UploadDocument,
        /// <summary>
        /// Finding a location
        /// </summary>
        FindLocation,
        /// <summary>
        /// Recording a video note
        /// </summary>
        RecordVideoNote,
        /// <summary>
        /// Uploading a video note
        /// </summary>
        UploadVideoNote
    }
}
