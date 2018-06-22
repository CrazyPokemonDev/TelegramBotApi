using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBotApi.Types.Payment;

namespace TelegramBotApi.Types.Events
{
    /// <summary>
    /// Event args for a shipping query
    /// </summary>
    public class ShippingQueryEventArgs : EventArgs
    {
        /// <summary>
        /// The shipping query
        /// </summary>
        public ShippingQuery ShippingQuery { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShippingQueryEventArgs"/> class
        /// </summary>
        /// <param name="shippingQuery"></param>
        public ShippingQueryEventArgs(ShippingQuery shippingQuery)
        {
            ShippingQuery = shippingQuery;
        }
    }
}
