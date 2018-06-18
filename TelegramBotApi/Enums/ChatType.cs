using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotApi.Enums
{
    /// <summary>
    /// Type of a chat
    /// </summary>
    public enum ChatType
    {
        /// <summary>
        /// A private chat
        /// </summary>
        Private,
        /// <summary>
        /// A group chat
        /// </summary>
        Group,
        /// <summary>
        /// A supergroup chat
        /// </summary>
        Supergroup,
        /// <summary>
        /// A channel
        /// </summary>
        Channel,
        /// <summary>
        /// An unimplemented type of chat
        /// </summary>
        Unknown
    }
}
