using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBotApi.Types.Inline;

namespace TelegramBotApi.Types.Events
{
    /// <summary>
    /// Events arg for an incoming inline query
    /// </summary>
    public class InlineQueryEventArgs : EventArgs
    {
        /// <summary>
        /// The incoming inline query
        /// </summary>
        public InlineQuery InlineQuery { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InlineQueryEventArgs"/> class
        /// </summary>
        /// <param name="inlineQuery">The inline query</param>
        public InlineQueryEventArgs(InlineQuery inlineQuery)
        {
            InlineQuery = inlineQuery;
        }
    }
}
