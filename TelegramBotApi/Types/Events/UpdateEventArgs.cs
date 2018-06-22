using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotApi.Types.Events
{
    /// <summary>
    /// Event args for a new update received by the bot
    /// </summary>
    public class UpdateEventArgs : EventArgs
    {
        /// <summary>
        /// The new update received by the bot
        /// </summary>
        public Update Update { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateEventArgs"/> class
        /// </summary>
        /// <param name="update">The update</param>
        public UpdateEventArgs(Update update)
        {
            Update = update;
        }
    }
}
