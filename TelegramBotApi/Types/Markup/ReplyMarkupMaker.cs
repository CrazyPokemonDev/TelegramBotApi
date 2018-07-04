using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBotApi.Types.Game;

namespace TelegramBotApi.Types.Markup
{
    /// <summary>
    /// A helper to create replyMarkups more easily 
    /// See <see cref="ReplyMarkupMaker"/> for an easy way to create one of these
    /// </summary>
    public class ReplyMarkupMaker
    {
        #region ReplyMarkupType
        /// <summary>
        /// Type of the reply markup
        /// </summary>
        public enum ReplyMarkupType
        {
            /// <summary>
            /// An <see cref="InlineKeyboardMarkup"/> (buttons below a message)
            /// </summary>
            Inline,
            /// <summary>
            /// A <see cref="ReplyKeyboardMarkup"/> (buttons instead of normal keyboard)
            /// </summary>
            Reply,
            /// <summary>
            /// A <see cref="ForceReplyMarkup"/> (forcing one or all users to reply to the message)
            /// </summary>
            Force,
            /// <summary>
            /// A <see cref="ReplyKeyboardRemove"/> (request to remove the current <see cref="ReplyKeyboardMarkup"/>)
            /// </summary>
            Remove
        }
        #endregion

        private readonly ReplyMarkupType _markupType;

        private readonly ReplyMarkupBase _markup;

        private readonly List<List<InlineKeyboardButton>> _inlineRows = new List<List<InlineKeyboardButton>>();

        private readonly List<List<KeyboardButton>> _replyRows = new List<List<KeyboardButton>>();

        #region Constructor
        /// <summary>
        /// Initializes a new <see cref="ReplyMarkupMaker"/> for the selected type
        /// </summary>
        /// <param name="type">The type of keyboard to make</param>
        public ReplyMarkupMaker(ReplyMarkupType type)
        {
            _markupType = type;
            switch (type)
            {
                case ReplyMarkupType.Force:
                    _markup = new ForceReplyMarkup();
                    break;
                case ReplyMarkupType.Inline:
                    _markup = new InlineKeyboardMarkup();
                    break;
                case ReplyMarkupType.Remove:
                    _markup = new ReplyKeyboardRemove();
                    break;
                case ReplyMarkupType.Reply:
                    _markup = new ReplyKeyboardMarkup();
                    break;
            }
        }
        #endregion
        #region Bool setters
        /// <summary>
        /// Whether the reply markup should only be available for certain users. 
        /// Targets are: users @mentioned in the message and the sender of the message replying to, if any 
        /// Returns the ReplyMarkupMaker to be able to chain calls. 
        /// Not available for <see cref="InlineKeyboardMarkup"/>s
        /// </summary>
        /// <param name="selective">True for a selective markup, false for a public one</param>
        /// <returns>The ReplyMarkupMaker to be able to chain calls.</returns>
        public ReplyMarkupMaker SetSelective(bool selective)
        {
            switch (_markupType)
            {
                case ReplyMarkupType.Force:
                    ((ForceReplyMarkup)_markup).Selective = selective;
                    break;
                case ReplyMarkupType.Inline:
                    throw new InvalidOperationException("Cannot set property 'selective' for inline keyboards");
                case ReplyMarkupType.Remove:
                    ((ReplyKeyboardRemove)_markup).Selective = selective;
                    break;
                case ReplyMarkupType.Reply:
                    ((ReplyKeyboardMarkup)_markup).Selective = selective;
                    break;
            }
            return this;
        }

        /// <summary>
        /// Requests clients to resize the keyboard vertically for optimal fit 
        /// (e.g., make the keyboard smaller if there are just two rows of buttons). 
        /// Defaults to false, in which case the custom keyboard is always of the same height as the app's standard keyboard. 
        /// Returns the ReplyMarkupMaker to be able to chain calls. 
        /// Only applicable for <see cref="ReplyKeyboardMarkup"/>
        /// </summary>
        /// <param name="resize">Whether to resize the Keyboard</param>
        /// <returns>The ReplyMarkupMaker to be able to chain calls.</returns>
        public ReplyMarkupMaker SetResizeKeyboard(bool resize)
        {
            switch (_markupType)
            {
                case ReplyMarkupType.Reply:
                    ((ReplyKeyboardMarkup)_markup).ResizeKeyboard = resize;
                    break;
                default:
                    throw new InvalidOperationException("Can only set property 'resizeKeyboard' for ReplyKeyboardMarkups");
            }
            return this;
        }

        /// <summary>
        /// Requests clients to hide the keyboard as soon as it's been used. 
        /// The keyboard will still be available, but clients will automatically display the usual letter-keyboard in the chat – 
        /// the user can press a special button in the input field to see the custom keyboard again. Defaults to false. 
        /// Returns the ReplyMarkupMaker to be able to chain calls. 
        /// Only applicable for <see cref="ReplyKeyboardMarkup"/>
        /// </summary>
        /// <param name="oneTime">True if the keyboard should be one-time</param>
        /// <returns>The ReplyMarkupMaker to be able to chain calls.</returns>
        public ReplyMarkupMaker SetOneTimeKeyboard(bool oneTime)
        {
            switch (_markupType)
            {
                case ReplyMarkupType.Reply:
                    ((ReplyKeyboardMarkup)_markup).OneTimeKeyboard = oneTime;
                    break;
                default:
                    throw new InvalidOperationException("Can only set property 'oneTimeKeyboard' for ReplyKeyboardMarkups");
            }
            return this;
        }
        #endregion

        #region Keyboard making
        /// <summary>
        /// Adds a new empty row of keys.
        /// Returns the <see cref="ReplyMarkupMaker"/> to chain calls.
        /// </summary>
        /// <returns>The <see cref="ReplyMarkupMaker"/> to chain calls</returns>
        public ReplyMarkupMaker AddRow()
        {
            switch (_markupType)
            {
                case ReplyMarkupType.Inline:
                    _inlineRows.Add(new List<InlineKeyboardButton>());
                    break;
                case ReplyMarkupType.Reply:
                    _replyRows.Add(new List<KeyboardButton>());
                    break;
                default:
                    throw new InvalidOperationException("Cannot add button rows to Force or Remove ReplyMarkups");
            }
            return this;
        }

        /// <summary>
        /// Adds a pure text button to the Keyboard. 
        /// In case of an inline keyboard, it will be a CallbackButton with "." as CallbackData 
        /// Returns the <see cref="ReplyMarkupMaker"/> to chain calls.
        /// </summary>
        /// <param name="text">The text to display on the button 
        /// (and send on click, in case of a <see cref="ReplyKeyboardMarkup"/>)</param>
        /// <param name="row">The zero-based index of the row in which to add the button. 
        /// If nothing is specified, uses last added row</param>
        /// <returns>The <see cref="ReplyMarkupMaker"/> to chain calls</returns>
        public ReplyMarkupMaker AddTextButton(string text, int row = -1)
        {
            switch (_markupType)
            {
                case ReplyMarkupType.Reply:
                    if (row < 0) row = _replyRows.Count - 1;
                    _replyRows[row].Add(new KeyboardButton(text));
                    break;
                case ReplyMarkupType.Inline:
                    if (row < 0) row = _inlineRows.Count - 1;
                    _inlineRows[row].Add(new InlineKeyboardButton(text) { CallbackData = "." });
                    break;
                default:
                    throw new InvalidOperationException("Cannot add buttons to Force or Remove ReplyMarkups");
            }
            return this;
        }

        #region ReplyKeyboard only buttons
        /// <summary>
        /// Adds a button that will send the users phone number as a contact when pressed. Private chats only. 
        /// Only applicable to <see cref="ReplyKeyboardMarkup"/>. 
        /// Returns the <see cref="ReplyMarkupMaker"/> to chain calls.
        /// </summary>
        /// <param name="text">The text to be displayed on the button</param>
        /// <param name="row">The zero-based index of the row in which to add the button. 
        /// If nothing is specified, uses last added row</param>
        /// <returns>The <see cref="ReplyMarkupMaker"/> to chain calls</returns>
        public ReplyMarkupMaker AddRequestContactButton(string text, int row = -1)
        {
            switch (_markupType)
            {
                case ReplyMarkupType.Reply:
                    if (row < 0) row = _replyRows.Count - 1;
                    _replyRows[row].Add(new KeyboardButton(text) { RequestContact = true });
                    break;
                default:
                    throw new InvalidOperationException("Can only add RequestContactButtons to ReplyKeyboard Markups");
            }
            return this;
        }

        /// <summary>
        /// Adds a button that will send the users location when pressed. Private chats only. 
        /// Only applicable to <see cref="ReplyKeyboardMarkup"/>. 
        /// Returns the <see cref="ReplyMarkupMaker"/> to chain calls.
        /// </summary>
        /// <param name="text">The text to be displayed on the button</param>
        /// <param name="row">The zero-based index of the row in which to add the button. 
        /// If nothing is specified, uses last added row</param>
        /// <returns>The <see cref="ReplyMarkupMaker"/> to chain calls</returns>
        public ReplyMarkupMaker AddRequestLocationButton(string text, int row = -1)
        {
            switch (_markupType)
            {
                case ReplyMarkupType.Reply:
                    if (row < 0) row = _replyRows.Count - 1;
                    _replyRows[row].Add(new KeyboardButton(text) { RequestLocation = true });
                    break;
                default:
                    throw new InvalidOperationException("Can only add RequestLocationButtons to ReplyKeyboard Markups");
            }
            return this;
        }
        #endregion
        #region InlineKeyboard only Buttons
        /// <summary>
        /// Adds a button that opens a URL when clicked to a keyboard. Only for <see cref="InlineKeyboardMarkup"/>s. 
        /// Returns the <see cref="ReplyMarkupMaker"/> to chain calls.
        /// </summary>
        /// <param name="text">The text to display on the button</param>
        /// <param name="url">The URL to open on clicking the button</param>
        /// <param name="row">The zero-based index of the row in which to add the button. 
        /// If nothing is specified, uses last added row</param>
        /// <returns>The <see cref="ReplyMarkupMaker"/> to chain calls</returns>
        public ReplyMarkupMaker AddUrlButton(string text, string url, int row = -1)
        {
            switch (_markupType)
            {
                case ReplyMarkupType.Inline:
                    if (row < 0) row = _inlineRows.Count;
                    _inlineRows[row].Add(new InlineKeyboardButton(text) { Url = url });
                    break;
                default:
                    throw new InvalidOperationException("Can only add URL buttons to inline keyboards");
            }
            return this;
        }

        /// <summary>
        /// Adds a button that sends a CallbackQuery when clicked to a keyboard. Only for <see cref="InlineKeyboardMarkup"/>s. 
        /// Returns the <see cref="ReplyMarkupMaker"/> to chain calls.
        /// </summary>
        /// <param name="text">The text to display on the button</param>
        /// <param name="callback">Custom string to recognize the callback for your bot. 
        /// Will be returned in <see cref="CallbackQuery.Data"/> when the button is clicked.</param>
        /// <param name="row">The zero-based index of the row in which to add the button. 
        /// If nothing is specified, uses last added row</param>
        /// <returns>The <see cref="ReplyMarkupMaker"/> to chain calls</returns>
        public ReplyMarkupMaker AddCallbackButton(string text, string callback, int row = -1)
        {
            switch (_markupType)
            {
                case ReplyMarkupType.Inline:
                    if (row < 0) row = _inlineRows.Count;
                    _inlineRows[row].Add(new InlineKeyboardButton(text) { CallbackData = callback });
                    break;
                default:
                    throw new InvalidOperationException("Can only add callback buttons to inline keyboards");
            }
            return this;
        }

        /// <summary>
        /// Adds a button that opens an inline query to your bot when clicked to a keyboard. Only for <see cref="InlineKeyboardMarkup"/>s. 
        /// Returns the <see cref="ReplyMarkupMaker"/> to chain calls.
        /// </summary>
        /// <param name="text">The text to display on the button</param>
        /// <param name="query">The query to be typed in the chat. Can be empty.</param>
        /// <param name="switchChat">If true, requests the user to choose a chat to send the query to</param>
        /// <param name="row">The zero-based index of the row in which to add the button. 
        /// If nothing is specified, uses last added row</param>
        /// <returns>The <see cref="ReplyMarkupMaker"/> to chain calls</returns>
        public ReplyMarkupMaker AddInlineQueryButton(string text, string query, bool switchChat = true, int row = -1)
        {
            switch (_markupType)
            {
                case ReplyMarkupType.Inline:
                    if (row < 0) row = _inlineRows.Count;
                    if (switchChat) _inlineRows[row].Add(new InlineKeyboardButton(text) { SwitchInlineQuery = query });
                    else _inlineRows[row].Add(new InlineKeyboardButton(text) { SwitchInlineQueryCurrentChat = query });
                    break;
                default:
                    throw new InvalidOperationException("Can only add inline query buttons to inline keyboards");
            }
            return this;
        }

        /// <summary>
        /// Adds a button that sends a callback query to your bot when clicked to a keyboard. You should answer the query with a game. 
        /// Must be the first button in the first row. 
        /// Only for <see cref="InlineKeyboardMarkup"/>s. 
        /// Returns the <see cref="ReplyMarkupMaker"/> to chain calls.
        /// </summary>
        /// <param name="text">The text to display on the button</param>
        /// <param name="callbackData">Custom string to recognize this button by. Will be sent in <see cref="CallbackQuery.Data"/>.</param>
        /// <param name="row">The zero-based index of the row in which to add the button. 
        /// If nothing is specified, uses last added row</param>
        /// <returns>The <see cref="ReplyMarkupMaker"/> to chain calls</returns>
        public ReplyMarkupMaker AddCallbackGameButton(string text, string callbackData, int row = -1)
        {
            switch (_markupType)
            {
                case ReplyMarkupType.Inline:
                    if (row < 0) row = _inlineRows.Count;
                    if (row != 0 || _inlineRows[row].Count > 0)
                        throw new InvalidOperationException("A callback game button must always be the first button in the first row");
                    _inlineRows[row].Add(new InlineKeyboardButton(text) { CallbackData = callbackData, CalbackGame = new CallbackGame() });
                    break;
                default:
                    throw new InvalidOperationException("Can only add callback game buttons to inline keyboards");
            }
            return this;
        }

        /// <summary>
        /// Adds a button to pay an invoice to the keyboard. Does only work when sending an invoice.
        /// Must be the first button in the first row. 
        /// Only for <see cref="InlineKeyboardMarkup"/>s. 
        /// Returns the <see cref="ReplyMarkupMaker"/> to chain calls.
        /// </summary>
        /// <param name="text">The text to display on the button</param>
        /// <param name="row">The zero-based index of the row in which to add the button. 
        /// If nothing is specified, uses last added row</param>
        /// <returns>The <see cref="ReplyMarkupMaker"/> to chain calls</returns>
        public ReplyMarkupMaker AddPayButton(string text, int row = -1)
        {
            switch (_markupType)
            {
                case ReplyMarkupType.Inline:
                    if (row < 0) row = _inlineRows.Count;
                    if (row != 0 || _inlineRows[row].Count > 0)
                        throw new InvalidOperationException("A pay button must always be the first button in the first row");
                    _inlineRows[row].Add(new InlineKeyboardButton(text) { Pay = true });
                    break;
                default:
                    throw new InvalidOperationException("Can only add pay buttons to inline keyboards");
            }
            return this;
        }
        #endregion
        #endregion

        /// <summary>
        /// Returns the finished keyboard.
        /// </summary>
        /// <returns>The finished keyboard</returns>
        public ReplyMarkupBase Finish()
        {
            switch (_markupType)
            {
                case ReplyMarkupType.Inline:
                    ((InlineKeyboardMarkup)_markup).InlineKeyboard = _inlineRows.Select(x => x.ToArray()).ToArray();
                    return _markup;
                case ReplyMarkupType.Reply:
                    ((ReplyKeyboardMarkup)_markup).Keyboard = _replyRows.Select(x => x.ToArray()).ToArray();
                    return _markup;
                default:
                    return _markup;
            }
        }

        /// <summary>
        /// Implicitly creates the keyboard from a ReplyKeyboardMaker
        /// </summary>
        /// <param name="maker">The maker</param>
        public static implicit operator ReplyMarkupBase(ReplyMarkupMaker maker)
        {
            return maker.Finish();
        }
    }
}
