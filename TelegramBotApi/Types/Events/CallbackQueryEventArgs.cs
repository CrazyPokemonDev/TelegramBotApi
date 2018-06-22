using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotApi.Types.Events
{
    /// <summary>
    /// Event args for an incoming callback query
    /// </summary>
    public class CallbackQueryEventArgs : EventArgs
    {
        /// <summary>
        /// The incoming callback query
        /// </summary>
        public CallbackQuery CallbackQuery { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CallbackQueryEventArgs"/> class
        /// </summary>
        /// <param name="callbackQuery">The callback query</param>
        public CallbackQueryEventArgs(CallbackQuery callbackQuery)
        {
            CallbackQuery = callbackQuery;
        }
    }
}
