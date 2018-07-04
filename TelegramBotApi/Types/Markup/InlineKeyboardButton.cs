using Newtonsoft.Json;
using TelegramBotApi.Types.Game;

namespace TelegramBotApi.Types.Markup
{
    /// <summary>
    /// This object represents one button of an inline keyboard. You must use exactly one of the optional fields.
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
    public class InlineKeyboardButton
    {
        /// <summary>
        /// Label text on the button
        /// </summary>
        [JsonProperty(PropertyName = "text", Required = Required.Always)]
        public string Text { get; set; }

        /// <summary>
        /// Optional. HTTP url to be opened when button is pressed
        /// </summary>
        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }

        /// <summary>
        /// Optional. Data to be sent in a callback query to the bot when button is pressed, 1-64 bytes
        /// </summary>
        [JsonProperty(PropertyName = "callback_data")]
        public string CallbackData { get; set; }

        /// <summary>
        /// Optional. If set, pressing the button will prompt the user to select one of their chats, 
        /// open that chat and insert the bot‘s username and the specified inline query in the input field. 
        /// Can be empty, in which case just the bot’s username will be inserted.
        /// </summary>
        [JsonProperty(PropertyName = "switch_inline_query")]
        public string SwitchInlineQuery { get; set; }

        /// <summary>
        /// Optional. If set, pressing the button will insert the bot‘s username and the specified inline query in the current chat's input field.
        /// Can be empty, in which case only the bot’s username will be inserted.
        ///
        /// This offers a quick way for the user to open your bot in inline mode in the same chat – good for selecting something from multiple options.
        /// </summary>
        [JsonProperty(PropertyName = "switch_inline_query_current_chat")]
        public string SwitchInlineQueryCurrentChat { get; set; }

        /// <summary>
        /// Optional. Description of the game that will be launched when the user presses the button.
        /// 
        /// NOTE: This type of button must always be the first button in the first row.
        /// </summary>
        [JsonProperty(PropertyName = "callback_game")]
        public CallbackGame CalbackGame { get; set; }

        /// <summary>
        /// Optional. Specify True, to send a Pay button.
        ///
        /// NOTE: This type of button must always be the first button in the first row.
        /// </summary>
        [JsonProperty(PropertyName = "pay")]
        public bool Pay { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InlineKeyboardButton"/> class. Has to be filled with data.
        /// </summary>
        public InlineKeyboardButton()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InlineKeyboardButton"/> class. Has to be filled with data.
        /// </summary>
        /// <param name="text">The text to show on the button</param>
        public InlineKeyboardButton(string text)
        {
            Text = text;
        }
    }
}
