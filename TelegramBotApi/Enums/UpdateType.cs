using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotApi.Enums
{
    /// <summary>
    /// Type of an update
    /// </summary>
    public enum UpdateType
    {
        /// <summary>
        /// A new incoming message
        /// </summary>
        Message,
        /// <summary>
        /// A message has been edited
        /// </summary>
        EditedMessage,
        /// <summary>
        /// A new post to a channel
        /// </summary>
        ChannelPost,
        /// <summary>
        /// A channel post has been edited
        /// </summary>
        EditedChannelPost,
        /// <summary>
        /// An inline query (typing @inlinebotname)
        /// </summary>
        InlineQuery,
        /// <summary>
        /// An inline result has been chosen
        /// </summary>
        ChosenInlineResult,
        /// <summary>
        /// A callback query from an inline keyboard button
        /// </summary>
        CallbackQuery,
        /// <summary>
        /// A shipping query
        /// </summary>
        ShippingQuery,
        /// <summary>
        /// A pre-checkout query
        /// </summary>
        PreCheckoutQuery,
        /// <summary>
        /// A Telegram Passport data
        /// </summary>
        PassportData,
        /// <summary>
        /// An update type not yet implemented
        /// </summary>
        Unknown
    }
}
