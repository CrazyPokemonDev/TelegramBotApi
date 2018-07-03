using Newtonsoft.Json;

namespace TelegramBotApi.Types.Markup
{
    /// <summary>
    /// This object represents an inline keyboard that appears right next to the message it belongs to.
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class InlineKeyboardMarkup : ReplyMarkupBase
    {
        /// <summary>
        /// Array of button rows, each represented by an Array of InlineKeyboardButton objects
        /// </summary>
        [JsonProperty(PropertyName = "inline_keyboard", Required = Required.Always)]
        public InlineKeyboardButton[][] InlineKeyboard { get; set; }

        /// <summary>
        /// Initializes a new, empty instance of the <see cref="InlineKeyboardMarkup"/> class
        /// </summary>
        public InlineKeyboardMarkup()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InlineKeyboardMarkup"/> class with only one button
        /// </summary>
        /// <param name="button">The one button</param>
        public InlineKeyboardMarkup(InlineKeyboardButton button)
        {
            InlineKeyboard = new[] { new[] { button } };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InlineKeyboardMarkup"/> class with only one row
        /// </summary>
        /// <param name="row">The row of inlineKeyboardButtons</param>
        public InlineKeyboardMarkup(InlineKeyboardButton[] row)
        {
            InlineKeyboard = new[] { row };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InlineKeyboardMarkup"/> class with multiple rows
        /// </summary>
        /// <param name="rows">The rows of InlineKeyboardButtons</param>
        public InlineKeyboardMarkup(InlineKeyboardButton[][] rows)
        {
            InlineKeyboard = rows;
        }

        /// <summary>
        /// Converts an InlineKeyboardButton to a markup
        /// </summary>
        /// <param name="button">The button</param>
        public static implicit operator InlineKeyboardMarkup(InlineKeyboardButton button)
            => new InlineKeyboardMarkup(button);

        /// <summary>
        /// Converts an InlineKeyboardButton row to a markup
        /// </summary>
        /// <param name="row">The button row</param>
        public static implicit operator InlineKeyboardMarkup(InlineKeyboardButton[] row)
            => new InlineKeyboardMarkup(row);

        /// <summary>
        /// Converts InlineKeyboardButton rows to a markup
        /// </summary>
        /// <param name="rows">The button rows</param>
        public static implicit operator InlineKeyboardMarkup(InlineKeyboardButton[][] rows)
            => new InlineKeyboardMarkup(rows);
    }
}
