using Newtonsoft.Json;
using TelegramBotApi.Enums;
using TelegramBotApi.Types.Inline;
using TelegramBotApi.Types.Payment;

namespace TelegramBotApi.Types
{
    /// <summary>
    /// This object represents an incoming update.
    /// At most one of the optional parameters can be present in any given update.
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class Update
    {
        /// <summary>
        /// The update's unique identifier
        /// </summary>
        [JsonProperty(PropertyName = "update_id", Required = Required.Always)]
        public int Id;

        /// <summary>
        /// Optional. New incoming message of any kind — text, photo, sticker, etc.
        /// </summary>
        [JsonProperty(PropertyName = "message")]
        public Message Message;

        /// <summary>
        /// Optional. New version of a message that is known to the bot and was edited
        /// </summary>
        [JsonProperty(PropertyName = "edited_message")]
        public Message EditedMessage;

        /// <summary>
        /// Optional. New incoming channel post of any kind — text, photo, sticker, etc.
        /// </summary>
        [JsonProperty(PropertyName = "channel_post")]
        public Message ChannelPost;

        /// <summary>
        /// Optional. New version of a channel post that is known to the bot and was edited
        /// </summary>
        [JsonProperty(PropertyName = "edited_channel_post")]
        public Message EditedChannelPost;

        /// <summary>
        /// Optional. New incoming inline query
        /// </summary>
        [JsonProperty(PropertyName = "inline_query")]
        public InlineQuery InlineQuery;

        /// <summary>
        /// Optional. The result of an inline query that was chosen by a user and sent to their chat partner
        /// </summary>
        [JsonProperty(PropertyName = "chosen_inline_result")]
        public ChosenInlineResult ChosenInlineResult;

        /// <summary>
        /// Optional. New incoming callback query
        /// </summary>
        [JsonProperty(PropertyName = "callback_query")]
        public CallbackQuery CallbackQuery;

        /// <summary>
        /// Optional. New incoming shipping query. Only for invoices with flexible price
        /// </summary>
        [JsonProperty(PropertyName = "shipping_query")]
        public ShippingQuery ShippingQuery;

        /// <summary>
        /// Optional. New incoming pre-checkout query. Contains full information about checkout
        /// </summary>
        [JsonProperty(PropertyName = "pre_checkout_query")]
        public PreCheckoutQuery PreCheckoutQuery;

        /// <summary>
        /// <see cref="UpdateType"/> of this update. Only the corresponding optional property will be filled.
        /// </summary>
        public UpdateType Type
        {
            get
            {
                if (Message != null) return UpdateType.Message;
                else if (EditedMessage != null) return UpdateType.EditedMessage;
                else if (ChannelPost != null) return UpdateType.ChannelPost;
                else if (EditedChannelPost != null) return UpdateType.EditedChannelPost;
                else if (InlineQuery != null) return UpdateType.InlineQuery;
                else if (ChosenInlineResult != null) return UpdateType.ChosenInlineResult;
                else if (CallbackQuery != null) return UpdateType.CallbackQuery;
                else if (ShippingQuery != null) return UpdateType.ShippingQuery;
                else if (PreCheckoutQuery != null) return UpdateType.PreCheckoutQuery;
                else return UpdateType.Unknown;
            }
        }
    }
}
