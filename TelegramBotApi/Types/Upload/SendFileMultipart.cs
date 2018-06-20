using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotApi.Types.Upload
{
    /// <summary>
    /// Represents a file to be sent via multipart upload
    /// </summary>
    public class SendFileMultipart : SendFile
    {
        /// <summary>
        /// This file is to be sent via multipart/form data
        /// </summary>
        public override SendFileType Type => SendFileType.Multipart;

        /// <summary>
        /// The file stream associated with this file
        /// </summary>
        public Stream FileStream;

        /// <summary>
        /// Initializes a new instance of the <see cref="SendFileMultipart"/> class
        /// </summary>
        /// <param name="path">The path of the file to send</param>
        public SendFileMultipart(string path)
        {
            FileStream = File.OpenRead(path);
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
