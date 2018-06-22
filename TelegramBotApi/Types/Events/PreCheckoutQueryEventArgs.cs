using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBotApi.Types.Payment;

namespace TelegramBotApi.Types.Events
{
    /// <summary>
    /// Event args for a pre-checkout query
    /// </summary>
    public class PreCheckoutQueryEventArgs : EventArgs
    {
        /// <summary>
        /// The pre-checkout query
        /// </summary>
        public PreCheckoutQuery PreCheckoutQuery { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PreCheckoutQueryEventArgs"/> class
        /// </summary>
        /// <param name="preCheckoutQuery">The pre-checkout query</param>
        public PreCheckoutQueryEventArgs(PreCheckoutQuery preCheckoutQuery)
        {
            PreCheckoutQuery = preCheckoutQuery;
        }
    }
}
