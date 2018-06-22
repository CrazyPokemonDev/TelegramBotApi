using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBotApi.Types.Inline;

namespace TelegramBotApi.Types.Events
{
    /// <summary>
    /// Event args for a chosen inline result
    /// </summary>
    public class ChosenInlineResultEventArgs : EventArgs
    {
        /// <summary>
        /// The chosen inline result
        /// </summary>
        public ChosenInlineResult ChosenInlineResult { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChosenInlineResultEventArgs"/> class
        /// </summary>
        /// <param name="chosenInlineResult">The chosen inline result</param>
        public ChosenInlineResultEventArgs(ChosenInlineResult chosenInlineResult)
        {
            ChosenInlineResult = chosenInlineResult;
        }
    }
}
