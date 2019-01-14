using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotApi.Helpers
{
    /// <summary>
    /// Provides Extension Methods for more user friendliness
    /// </summary>
    public static class Extensions
    {
        private static string TimeoutPropertyKey = "RequestTimeout";

        /// <summary>
        /// Sets the timeout property of a <see cref="HttpRequestMessage"/> to a certain <see cref="TimeSpan"/>
        /// </summary>
        /// <param name="request">The request that needs its timeout set</param>
        /// <param name="timeout">The timeout to set</param>
        public static void SetTimeout(this HttpRequestMessage request, TimeSpan? timeout)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            request.Properties[TimeoutPropertyKey] = timeout;
        }

        /// <summary>
        /// Gets the timeout property of a <see cref="HttpRequestMessage"/> as a <see cref="TimeSpan"/>
        /// </summary>
        /// <param name="request">The request from that the timeout is taken</param>
        /// <returns>The timeout of the request</returns>
        public static TimeSpan? GetTimeout(this HttpRequestMessage request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.Properties.TryGetValue(TimeoutPropertyKey, out var value) && value is TimeSpan timeout)
                return timeout;
            return null;
        }
    }
}
