using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotApi.Types.Upload
{
    /// <summary>
    /// Represents a file to be sent via URL
    /// </summary>
    public class SendFileUrl : SendFile
    {
        /// <summary>
        /// This file is to be sent as an URL
        /// </summary>
        public override SendFileType Type => SendFileType.Url;

        /// <summary>
        /// The URL of the file to send
        /// </summary>
        public string Url;

        /// <summary>
        /// Initializes a new instance of the SendFileUrl class
        /// </summary>
        /// <param name="url">The url of the file to send</param>
        public SendFileUrl(string url)
        {
            Url = url;
        }
    }
}
