using Newtonsoft.Json;

namespace TelegramBotApi.Types.Markup
{
    /// <summary>
    /// This object represents a custom keyboard with reply options (see Introduction to bots for details and examples). 
    /// See <see cref="ReplyMarkupMaker"/> for an easy way to create one of these
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
    public class ReplyKeyboardMarkup : ReplyMarkupBase
    {
        /// <summary>
        /// Array of button rows, each represented by an Array of KeyboardButton objects
        /// </summary>
        [JsonProperty(PropertyName = "keyboard", Required = Required.Always)]
        public KeyboardButton[][] Keyboard { get; set; }

        /// <summary>
        /// Optional. Requests clients to resize the keyboard vertically for optimal fit 
        /// (e.g., make the keyboard smaller if there are just two rows of buttons). 
        /// Defaults to false, in which case the custom keyboard is always of the same height as the app's standard keyboard.
        /// </summary>
        [JsonProperty(PropertyName = "resize_keyboard")]
        public bool ResizeKeyboard { get; set; } = false;

        /// <summary>
        /// Optional. Requests clients to hide the keyboard as soon as it's been used. 
        /// The keyboard will still be available, but clients will automatically display the usual letter-keyboard in the chat – 
        /// the user can press a special button in the input field to see the custom keyboard again. Defaults to false.
        /// </summary>
        [JsonProperty(PropertyName = "one_time_keyboard")]
        public bool OneTimeKeyboard { get; set; } = false;

        /// <summary>
        /// Optional. Use this parameter if you want to show the keyboard to specific users only. 
        /// Targets: 1) users that are @mentioned in the text of the Message object; 
        /// 2) if the bot's message is a reply (has reply_to_message_id), sender of the original message.
        ///
        /// Example: A user requests to change the bot‘s language, bot replies to the request with a keyboard to select the new language.
        /// Other users in the group don’t see the keyboard.
        /// </summary>
        [JsonProperty(PropertyName = "selective")]
        public bool Selective { get; set; }

        /// <summary>
        /// Initializes a new, empty instance of the <see cref="ReplyKeyboardMarkup"/> class
        /// </summary>
        public ReplyKeyboardMarkup()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReplyKeyboardMarkup"/> class with only one button
        /// </summary>
        /// <param name="button">The one button</param>
        public ReplyKeyboardMarkup(KeyboardButton button)
        {
            Keyboard = new[] { new[] { button } };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReplyKeyboardMarkup"/> class with only one row
        /// </summary>
        /// <param name="row">The row of KeyboardButtons</param>
        public ReplyKeyboardMarkup(KeyboardButton[] row)
        {
            Keyboard = new[] { row };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReplyKeyboardMarkup"/> class with multiple rows
        /// </summary>
        /// <param name="rows">The rows of KeyboardButtons</param>
        public ReplyKeyboardMarkup(KeyboardButton[][] rows)
        {
            Keyboard = rows;
        }

        /// <summary>
        /// Converts an InlineKeyboardButton to a markup
        /// </summary>
        /// <param name="button">The button</param>
        public static implicit operator ReplyKeyboardMarkup(KeyboardButton button)
            => new ReplyKeyboardMarkup(button);

        /// <summary>
        /// Converts an InlineKeyboardButton row to a markup
        /// </summary>
        /// <param name="row">The button row</param>
        public static implicit operator ReplyKeyboardMarkup(KeyboardButton[] row)
            => new ReplyKeyboardMarkup(row);

        /// <summary>
        /// Converts InlineKeyboardButton rows to a markup
        /// </summary>
        /// <param name="rows">The button rows</param>
        public static implicit operator ReplyKeyboardMarkup(KeyboardButton[][] rows)
            => new ReplyKeyboardMarkup(rows);
    }
}
