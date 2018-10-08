using System;
using System.Linq;
using TelegramBotApi.Enums;
using TelegramBotApi.Types;

namespace TelegramBotApi.Extensions.FormattedTextExtraction
{
    /// <summary>
    /// Custom extension class, implements methods to extract HTML- or Markdown-formatted text from messages, using their message entities
    /// </summary>
    public static class FormattedTextExtraction
    {
        #region HTML
        /// <summary>
        /// Extracts the HTML-formatted text from a message, using its message entities. Calling this method will not result in an API call.
        /// </summary>
        /// <param name="message">The message to extract the HTML-formatted text</param>
        /// <returns>HTML-formatted text of the given <paramref name="message"/></returns>
        public static string ExtractHTML(this Message message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            if (!string.IsNullOrEmpty(message.Text)) return message.Text.ExtractHTML(message.Entities);
            if (!string.IsNullOrEmpty(message.Caption)) return message.Caption.ExtractHTML(message.CaptionEntities);
            return null;
        }

        /// <summary>
        /// Extracts the HTML-formatted text from a message text, using the message's entities. Calling this method will not result in an API call.
        /// </summary>
        /// <param name="messageText">The message text to be HTML-formatted</param>
        /// <param name="messageEntities">The entities of the message, with which the <paramref name="messageText"/> is formatted</param>
        /// <returns>The <paramref name="messageText"/>, HTML-formatted with the given <paramref name="messageEntities"/></returns>
        public static string ExtractHTML(this string messageText, MessageEntity[] messageEntities)
        {
            if (string.IsNullOrEmpty(messageText)) return null;
            if (messageEntities == null || messageEntities.Length == 0) return messageText;

            if (messageEntities.Any(x => x.Offset + x.Length > messageText.Length))
                throw new IndexOutOfRangeException("At least one entity does not fit in the given text");

            // just to make sure they are ordered...
            messageEntities = messageEntities.OrderBy(x => x.Offset).ToArray();
            string final = messageText;

            for (int i = messageEntities.Length - 1; i >= 0; i--) // go through them in descending order,
            {                                                     // so the offsets are not modified when inserting
                MessageEntity e = messageEntities[i];             // HTML tags in the string
                switch (e.Type)
                {
                    case MessageEntityType.Bold:
                        final = final.Insert(e.Offset + e.Length, "</b>");
                        final = final.Insert(e.Offset, "<b>");
                        break;

                    case MessageEntityType.Italic:
                        final = final.Insert(e.Offset + e.Length, "</i>");
                        final = final.Insert(e.Offset, "<i>");
                        break;

                    case MessageEntityType.Code:
                        final = final.Insert(e.Offset + e.Length, "</code>");
                        final = final.Insert(e.Offset, "<code>");
                        break;

                    case MessageEntityType.Pre:
                        final = final.Insert(e.Offset + e.Length, "</pre>");
                        final = final.Insert(e.Offset, "<pre>");
                        break;

                    case MessageEntityType.TextLink:
                        final = final.Insert(e.Offset + e.Length, "</a>");
                        final = final.Insert(e.Offset, "<a href=\"" + e.Url + "\">");
                        break;

                    case MessageEntityType.TextMention:
                        final = final.Insert(e.Offset + e.Length, "</a>");
                        final = final.Insert(e.Offset, "<a href=\"tg://user?id=" + e.User.Id + "\">");
                        break;
                }
            }

            return final;
        }
        #endregion

        #region Markdown
        /// <summary>
        /// Extracts the Markdown-formatted text from a message, using its message entities. Calling this method will not result in an API call.
        /// </summary>
        /// <param name="message">The message to extract the Markdown-formatted text</param>
        /// <returns>Markdown-formatted text of the given <paramref name="message"/></returns>
        public static string ExtractMarkdown(this Message message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            if (!string.IsNullOrEmpty(message.Text)) return message.Text.ExtractMarkdown(message.Entities);
            if (!string.IsNullOrEmpty(message.Caption)) return message.Caption.ExtractMarkdown(message.CaptionEntities);
            return null;
        }

        /// <summary>
        /// Extracts the Markdown-formatted text from a message text, using the message's entities. Calling this method will not result in an API call.
        /// </summary>
        /// <param name="messageText">The message text to be Markdown-formatted</param>
        /// <param name="messageEntities">The entities of the message, with which the <paramref name="messageText"/> is formatted</param>
        /// <returns>The <paramref name="messageText"/>, Markdown-formatted with the given <paramref name="messageEntities"/></returns>
        public static string ExtractMarkdown(this string messageText, MessageEntity[] messageEntities)
        {
            if (string.IsNullOrEmpty(messageText)) return null;
            if (messageEntities == null || messageEntities.Length == 0) return messageText;

            if (messageEntities.Any(x => x.Offset + x.Length > messageText.Length))
                throw new IndexOutOfRangeException("At least one entity does not fit in the given text");

            // just to make sure they are ordered...
            messageEntities = messageEntities.OrderBy(x => x.Offset).ToArray();
            string final = messageText;

            for (int i = messageEntities.Length - 1; i >= 0; i--) // go through them in descending order,
            {                                                     // so the offsets are not modified when inserting
                MessageEntity e = messageEntities[i];             // Markdown tags in the string
                switch (e.Type)
                {
                    case MessageEntityType.Bold:
                        final = final.Insert(e.Offset + e.Length, "*");
                        final = final.Insert(e.Offset, "*");
                        break;

                    case MessageEntityType.Italic:
                        final = final.Insert(e.Offset + e.Length, "_");
                        final = final.Insert(e.Offset, "_");
                        break;

                    case MessageEntityType.Code:
                        final = final.Insert(e.Offset + e.Length, "`");
                        final = final.Insert(e.Offset, "`");
                        break;

                    case MessageEntityType.Pre:
                        final = final.Insert(e.Offset + e.Length, "```");
                        final = final.Insert(e.Offset, "```");
                        break;

                    case MessageEntityType.TextLink:
                        final = final.Insert(e.Offset + e.Length, "](" + e.Url + ")");
                        final = final.Insert(e.Offset, "[");
                        break;

                    case MessageEntityType.TextMention:
                        final = final.Insert(e.Offset + e.Length, "](tg://user?id=" + e.User.Id + ")");
                        final = final.Insert(e.Offset, "[");
                        break;
                }
            }

            return final;
        }
        #endregion
    }
}
