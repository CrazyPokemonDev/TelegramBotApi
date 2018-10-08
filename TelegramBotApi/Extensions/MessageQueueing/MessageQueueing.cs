using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using TelegramBotApi.Enums;
using TelegramBotApi.Types;
using TelegramBotApi.Types.Markup;

namespace TelegramBotApi.Extensions.MessageQueueing
{
    /// <summary>
    /// Custom extension class, implements a basic message queue to avoid hitting 429-Errors (Too many requests)
    /// </summary>
    public static class MessageQueueing
    {
        /// <summary>
        /// Sends a text message with a queue. Using this method you should never hit 429-Errors (Too many requests).
        /// If you do, please open a GitHub issue.
        /// Note that the parameters <paramref name="disableWebPagePreview"/> and <paramref name="disableNotification"/>
        /// will be overridden by the last merged message, if you enable message merging. If you want to use this method to send messages, 
        /// you need to call the <see cref="StartQueue(TelegramBot, ParseMode, bool)"/> method to start it.
        /// </summary>
        /// <param name="Bot">The Telegram Bot object for which a message is enqueued</param>
        /// <param name="chatId">The id or channel username of the chat to send the message to</param>
        /// <param name="text">The text of the message</param>
        /// <param name="disableWebPagePreview">If this is true, no website preview will be shown</param>
        /// <param name="disableNotification">If this is true, users will not receive a notification or a silent one for this message</param>
        /// <param name="replyToMessageId">The message id of the message to reply to in this chat, if any</param>
        /// <param name="replyMarkup">A <see cref="ReplyMarkupBase"/>.</param>
        public static void SendMessageWithQueue(this TelegramBot Bot, ChatId chatId, string text,
            bool disableWebPagePreview = false, bool disableNotification = false, int replyToMessageId = -1,
            ReplyMarkupBase replyMarkup = null)
        {
            if (Bot._messageQueue.ContainsKey(chatId))
                Bot._messageQueue[chatId].Enqueue(new QueuedMessage(chatId, text, disableWebPagePreview, disableNotification, replyToMessageId, replyMarkup));

            else Bot._messageQueue.Add(chatId, new Queue<QueuedMessage>(new[] { new QueuedMessage(chatId, text, disableWebPagePreview, disableNotification, replyToMessageId, replyMarkup) }));
        }

        /// <summary>
        /// Start the message queue. Messages enqueued by <see cref="SendMessageWithQueue(TelegramBot, ChatId, string, bool, bool, int, ReplyMarkupBase)"/> will only be sent after this method was called.
        /// </summary>
        /// <param name="Bot">The Telegram Bot object for which the message queue is started</param>
        /// <param name="parseMode">All messages that are enqueued with <see cref="SendMessageWithQueue(TelegramBot, ChatId, string, bool, bool, int, ReplyMarkupBase)"/> will use this ParseMode.</param>
        /// <param name="mergeMessages">If this is true, multiple queued messages can be merged into one, seperated by two newlines, for more efficient message sending. Note that messages having a replyToMessageId or replyMarkup will never be merged.</param>
        public static void StartQueue(this TelegramBot Bot, ParseMode parseMode, bool mergeMessages = true)
        {
            if (Bot.IsMessageQueueing) return;

            Bot.IsMessageQueueing = true;
            Bot._messageQueueParseMode = parseMode;
            Bot._messageQueueMerging = mergeMessages;
            Bot._messageQueueThread = new Thread(() => Bot.MessageQueue());
            Bot._messageQueueThread.Start();
        }

        /// <summary>
        /// Stop the message queue. Messages enqueued by <see cref="SendMessageWithQueue(TelegramBot, ChatId, string, bool, bool, int, ReplyMarkupBase)"/> will no longer be sent. The current queue will be emptied.
        /// </summary>
        /// <param name="Bot">The Telegram Bot object for which the message queue is stopped</param>
        public static void StopQueue(this TelegramBot Bot)
        {
            if (!Bot.IsMessageQueueing) return;

            Bot.IsMessageQueueing = false;
            Bot._messageQueue.Clear();
            Bot._messageQueueTimeout.Clear();
        }

        private static void MessageQueue(this TelegramBot Bot)
        {
            while (Bot.IsMessageQueueing)
            {
                var chatIds = Bot._messageQueue.Keys.ToList().Where(x => !Bot._messageQueueTimeout.ContainsKey(x));
                foreach (var chatId in chatIds)
                {
                    var queue = Bot._messageQueue[chatId];
                    string final = "";
                    bool byteMax = false;
                    int i = 0;

                    bool disableWebPagePreview = false;
                    bool disableNotification = false;
                    int replyToMessageId = -1;
                    ReplyMarkupBase replyMarkup = null;

                    while (!byteMax && queue.Count > 0 && (Bot._messageQueueMerging || i == 0))
                    {
                        i++;
                        var m = queue.Peek();
                        if (i > 1 &&
                            ((replyToMessageId != -1 || m.ReplyToMessageId != -1) ||
                            replyMarkup != null || m.ReplyMarkup != null))
                            break;

                        var temp = final + m.Text + Environment.NewLine + Environment.NewLine;

                        if (Encoding.UTF8.GetByteCount(temp) > 512)
                        {
                            if (i > 1)
                            {
                                break; // we already have at least one message and yet it's below 512 bytes, so send that and keep this message for the next time.
                            }
                            else
                            {
                                Bot._messageQueueTimeout.Add(chatId, 3); // this is the first message and it's over 512 bytes, so send it and add some additional timeout.
                                byteMax = true;
                            }
                        }
                        queue.Dequeue(); //remove the message, we are sending it.
                        final += m.Text + Environment.NewLine + Environment.NewLine;
                        disableWebPagePreview = m.DisableWebPagePreview;
                        disableNotification = m.DisableNotification;
                        replyToMessageId = m.ReplyToMessageId;
                        replyMarkup = m.ReplyMarkup;
                    }

                    if (!Bot.IsMessageQueueing) return;

                    if (!string.IsNullOrEmpty(final))
                    {
                        Bot.SendMessage(chatId, final, Bot._messageQueueParseMode, disableWebPagePreview, disableNotification, replyToMessageId, replyMarkup);
                    }

                    if (queue.Count == 0) Bot._messageQueue.Remove(chatId);
                }
                if (!Bot.IsMessageQueueing) return;
                foreach (var timeout in Bot._messageQueueTimeout.Keys.ToList())
                {
                    if (!Bot.IsMessageQueueing) return;
                    Bot._messageQueueTimeout[timeout]--;
                    if (Bot._messageQueueTimeout[timeout] == 0) Bot._messageQueueTimeout.Remove(timeout);
                }
                Thread.Sleep(4000);
            }
        }
    }
}
