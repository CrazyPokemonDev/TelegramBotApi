using Newtonsoft.Json;

namespace TelegramBotApi.Types
{
    /// <summary>
    /// This object represents an incoming callback query from a callback button in an inline keyboard. 
    /// If the button that originated the query was attached to a message sent by the bot, the field message will be present. 
    /// If the button was attached to a message sent via the bot (in inline mode), the field inline_message_id will be present. 
    /// Exactly one of the fields data or game_short_name will be present.
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class CallbackQuery
    {
        /// <summary>
        /// Unique identifier for this query
        /// </summary>
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public string Id;

        /// <summary>
        /// Sender
        /// </summary>
        [JsonProperty(PropertyName = "from", Required = Required.Always)]
        public User From;

        /// <summary>
        /// Optional. Message with the callback button that originated the query. 
        /// Note that message content and message date will not be available if the message is too old
        /// </summary>
        [JsonProperty(PropertyName = "message")]
        public Message Message;

        /// <summary>
        /// Optional. Identifier of the message sent via the bot in inline mode, that originated the query.
        /// </summary>
        [JsonProperty(PropertyName = "inline_message_id")]
        public string InlineMessageId;

        /// <summary>
        /// Global identifier, uniquely corresponding to the chat to which the message with the callback button was sent. 
        /// Useful for high scores in games.
        /// </summary>
        [JsonProperty(PropertyName = "chat_instance")]
        public string ChatInstance;

        /// <summary>
        /// Optional. Data associated with the callback button. Be aware that a bad client can send arbitrary data in this field.
        /// </summary>
        [JsonProperty(PropertyName = "data")]
        public string Data;

        /// <summary>
        /// Optional. Short name of a Game to be returned, serves as the unique identifier for the game
        /// </summary>
        [JsonProperty(PropertyName = "game_short_name")]
        public string GameShortName;
    }
}
