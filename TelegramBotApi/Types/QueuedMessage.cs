using TelegramBotApi.Enums;
using TelegramBotApi.Types.Markup;

namespace TelegramBotApi.Types
{
    internal class QueuedMessage
    {
        internal ChatId ChatId { get; }
        internal string Text { get; }
        internal bool DisableWebPagePreview { get; }
        internal bool DisableNotification { get; }
        internal int ReplyToMessageId { get; }
        internal ReplyMarkupBase ReplyMarkup { get; }

        internal QueuedMessage(ChatId chatId, string text,
            bool disableWebPagePreview, bool disableNotification, int replyToMessageId, ReplyMarkupBase replyMarkup)
        {
            ChatId = chatId;
            Text = text;
            DisableWebPagePreview = disableWebPagePreview;
            DisableNotification = disableNotification;
            ReplyToMessageId = replyToMessageId;
            ReplyMarkup = replyMarkup;
        }
    }
}
