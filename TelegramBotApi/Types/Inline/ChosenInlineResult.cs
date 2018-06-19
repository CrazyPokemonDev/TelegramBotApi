using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotApi.Types.Inline
{
    /// <summary>
    /// Represents a result of an inline query that was chosen by the user and sent to their chat partner.
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class ChosenInlineResult
    {
        /// <summary>
        /// The unique identifier for the result that was chosen
        /// </summary>
        [JsonProperty(PropertyName = "result_id", Required = Required.Always)]
        public string ResultId;

        /// <summary>
        /// The user that chose the result
        /// </summary>
        [JsonProperty(PropertyName = "from", Required = Required.Always)]
        public User From;

        /// <summary>
        /// Optional. Sender location, only for bots that require user location
        /// </summary>
        [JsonProperty(PropertyName = "location")]
        public Location Location;

        /// <summary>
        /// Optional. Identifier of the sent inline message. Available only if there is an inline keyboard attached to the message. 
        /// Will be also received in callback queries and can be used to edit the message.
        /// </summary>
        [JsonProperty(PropertyName = "inline_message_id")]
        public string InlineMessageId;

        /// <summary>
        /// The query that was used to obtain the result
        /// </summary>
        [JsonProperty(PropertyName = "query", Required = Required.Always)]
        public string Query;
    }
}
