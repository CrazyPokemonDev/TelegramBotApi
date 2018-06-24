using System;

namespace TelegramBotApi.Types.Upload
{
    /// <summary>
    /// A file to be sent by the bot. Usually one of <see cref="SendFileId"/>, <see cref="SendFileUrl"/> and <see cref="SendFileMultipart"/>
    /// </summary>
    public abstract class SendFile
    {
        /// <summary>
        /// The way to send the file
        /// </summary>
        public abstract SendFileType Type { get; }

        /// <summary>
        /// Converts a string into a file to send.
        /// </summary>
        /// <param name="urlFileIdOrPath">The url or telegram file id of the file</param>
        public static implicit operator SendFile(string urlFileIdOrPath)
        {
            if (System.IO.File.Exists(urlFileIdOrPath)) return new SendFileMultipart(urlFileIdOrPath);
            if (Uri.IsWellFormedUriString(urlFileIdOrPath, UriKind.RelativeOrAbsolute)) return new SendFileUrl(urlFileIdOrPath);
            else return new SendFileId(urlFileIdOrPath);
        }
    }

    /// <summary>
    /// Type of file to send
    /// </summary>
    public enum SendFileType
    {
        /// <summary>
        /// Send via telegram file id
        /// </summary>
        FileId,
        /// <summary>
        /// Upload as multipart/form data
        /// </summary>
        Multipart,
        /// <summary>
        /// Send via URL
        /// </summary>
        Url,
        /// <summary>
        /// An attached file (for media groups)
        /// </summary>
        Attach
    }
}
