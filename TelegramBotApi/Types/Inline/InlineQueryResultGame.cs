using Newtonsoft.Json;
using TelegramBotApi.Types.Markup;

namespace TelegramBotApi.Types.Inline
{
    /// <summary>
    /// Represents a Game.
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
    public class InlineQueryResultGame : InlineQueryResult
    {
        private string _type = "game";

        /// <summary>
        /// Type of the result, must be game
        /// </summary>
        [JsonProperty(PropertyName = "type", Required = Required.Always)]
        public override string Type { get { return _type; } set { _type = "game"; } }

        /// <summary>
        /// Unique identifier for this result, 1-64 Bytes
        /// </summary>
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public override string Id { get; set; }

        /// <summary>
        /// Short name of the game
        /// </summary>
        [JsonProperty(PropertyName = "game_short_name", Required = Required.Always)]
        public string GameShortName { get; set; }

        /// <summary>
        /// Optional. Inline keyboard attached to the message
        /// </summary>
        [JsonProperty(PropertyName = "reply_markup")]
        public InlineKeyboardMarkup ReplyMarkup { get; set; }
    }
}
