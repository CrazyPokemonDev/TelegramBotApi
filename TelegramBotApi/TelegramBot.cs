using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TelegramBotApi.Enums;
using TelegramBotApi.Types;
using TelegramBotApi.Types.Events;
using TelegramBotApi.Types.Exceptions;
using TelegramBotApi.Types.Game;
using TelegramBotApi.Types.Inline;
using TelegramBotApi.Types.Markup;
using TelegramBotApi.Types.Payment;
using TelegramBotApi.Types.TelegramPassport;
using TelegramBotApi.Types.Upload;
using Enum = TelegramBotApi.Enums.Enum;
using File = TelegramBotApi.Types.File;

namespace TelegramBotApi
{
    /// <summary>
    /// Represents an interface to the telegram bot api.
    /// </summary>
    public class TelegramBot
    {
        #region Constants
        private const string BaseUrl = "https://api.telegram.org/bot";
        private readonly string ApiUrl;
        private readonly string _token;
        private const string fileDownloadLink = "https://api.telegram.org/file/bot{token}/{path}";
        #endregion
        #region Properties
        /// <summary>
        /// Timeout in seconds for long polling. Defaults to 100. Set this to 0 if you want to use short polling.
        /// </summary>
        public int LongPollingTimeout { get; set; } = 100;

        /// <summary>
        /// Whether the bot is currently trying to recieve messages
        /// </summary>
        public bool IsReceiving { get; set; } = false;

        private int _lastUpdateReceived = 0;
        #endregion

        #region Events
        /// <summary>
        /// This event will be fired on every new update after <see cref="StartReceiving(UpdateType[])"/> has been called
        /// </summary>
        public event EventHandler<UpdateEventArgs> OnUpdate;

        /// <summary>
        /// This event will be fired on every incoming message after <see cref="StartReceiving(UpdateType[])"/> has been called
        /// </summary>
        public event EventHandler<MessageEventArgs> OnMessage;

        /// <summary>
        /// This event will be fired on every edited message after <see cref="StartReceiving(UpdateType[])"/> has been called
        /// </summary>
        public event EventHandler<EditedMessageEventArgs> OnMessageEdited;

        /// <summary>
        /// This event will be fired on every channel post after <see cref="StartReceiving(UpdateType[])"/> has been called
        /// </summary>
        public event EventHandler<ChannelPostEventArgs> OnChannelPost;

        /// <summary>
        /// This event will be fired on every edited channel post after <see cref="StartReceiving(UpdateType[])"/> has been called
        /// </summary>
        public event EventHandler<EditedChannelPostEventArgs> OnChannelPostEdited;

        /// <summary>
        /// This event will be fired on every inline query after <see cref="StartReceiving(UpdateType[])"/> has been called
        /// </summary>
        public event EventHandler<InlineQueryEventArgs> OnInlineQuery;

        /// <summary>
        /// This event will be fired on every inline query after <see cref="StartReceiving(UpdateType[])"/> has been called
        /// </summary>
        public event EventHandler<ChosenInlineResultEventArgs> OnChosenInlineResult;

        /// <summary>
        /// This event will be fired on every callback query after <see cref="StartReceiving(UpdateType[])"/> has been called
        /// </summary>
        public event EventHandler<CallbackQueryEventArgs> OnCallbackQuery;

        /// <summary>
        /// This event will be fired on every shipping query after <see cref="StartReceiving(UpdateType[])"/> has been called
        /// </summary>
        public event EventHandler<ShippingQueryEventArgs> OnShippingQuery;

        /// <summary>
        /// This event will be fired on every pre-checkout query after <see cref="StartReceiving(UpdateType[])"/> has been called
        /// </summary>
        public event EventHandler<PreCheckoutQueryEventArgs> OnPreCheckoutQuery;
        #endregion
        #region Control Methods
        /// <summary>
        /// Starts polling to get updates. Updates will be sent to <see cref="OnUpdate"/> and their respective events
        /// </summary>
        public void StartReceiving(UpdateType[] allowedUpdates = null)
        {
            if (IsReceiving) return;
            IsReceiving = true;
#pragma warning disable CS4014 //I intentionally don't await this
            PollAsync(allowedUpdates);
#pragma warning restore CS4014
        }

        /// <summary>
        /// Stops polling to get updates.
        /// </summary>
        public void StopReceiving()
        {
            IsReceiving = false;
        }

        private async Task PollAsync(UpdateType[] allowedUpdates)
        {
            while (IsReceiving)
            {
                Update[] updates = await GetUpdatesAsync(allowedUpdates, LongPollingTimeout, _lastUpdateReceived + 1);
                foreach (Update update in updates)
                {
                    _lastUpdateReceived = update.Id;
                    switch (update.Type)
                    {
                        case UpdateType.Message:
                            OnMessage?.Invoke(this, new MessageEventArgs(update.Message));
                            break;
                        case UpdateType.EditedMessage:
                            OnMessageEdited?.Invoke(this, new EditedMessageEventArgs(update.EditedMessage));
                            break;
                        case UpdateType.ChannelPost:
                            OnChannelPost?.Invoke(this, new ChannelPostEventArgs(update.ChannelPost));
                            break;
                        case UpdateType.EditedChannelPost:
                            OnChannelPostEdited?.Invoke(this, new EditedChannelPostEventArgs(update.EditedChannelPost));
                            break;
                        case UpdateType.InlineQuery:
                            OnInlineQuery?.Invoke(this, new InlineQueryEventArgs(update.InlineQuery));
                            break;
                        case UpdateType.ChosenInlineResult:
                            OnChosenInlineResult?.Invoke(this, new ChosenInlineResultEventArgs(update.ChosenInlineResult));
                            break;
                        case UpdateType.CallbackQuery:
                            OnCallbackQuery?.Invoke(this, new CallbackQueryEventArgs(update.CallbackQuery));
                            break;
                        case UpdateType.ShippingQuery:
                            OnShippingQuery?.Invoke(this, new ShippingQueryEventArgs(update.ShippingQuery));
                            break;
                        case UpdateType.PreCheckoutQuery:
                            OnPreCheckoutQuery?.Invoke(this, new PreCheckoutQueryEventArgs(update.PreCheckoutQuery));
                            break;
                    }
                    OnUpdate?.Invoke(this, new UpdateEventArgs(update));
                }
            }
        }

        /// <summary>
        /// Marks all pending updates as read. Don't use this while the bot is receiving.
        /// </summary>
        public void ClearUpdates()
        {
            if (IsReceiving) throw new Exception("Cannot clear updates while bot is receiving");
            Update[] updates = GetUpdatesAsync(timeout: 0, offset: -1).Result;
            if (updates.Length > 0) GetUpdatesAsync(timeout: 0, offset: updates[0].Id + 1).Wait();
        }
        #endregion

        #region Constructor(s)
        /// <summary>
        /// Initializes a new interface to the telegram bot api.
        /// </summary>
        /// <param name="token">The token you can obtain from <a href="http://t.me/BotFather">@BotFather</a></param>
        /// <exception cref="ArgumentException">Throws if the <paramref name="token"/> is invalid</exception>
        public TelegramBot(string token)
        {
            if (!Regex.IsMatch(token, @"\d+:[\w-]{35}")) throw new ArgumentException("Invalid token");
            _token = token;
            ApiUrl = BaseUrl + token + "/";
        }
        #endregion
        #region Api Call Execution
        /// <summary>
        /// Executes a call to the telegram bot API. Mainly used by the other methods, 
        /// but you can use it for non-implemented methods as well.
        /// </summary>
        /// <typeparam name="T">Type of object being returned</typeparam>
        /// <param name="method">The name of the telegram bot API method to call</param>
        /// <param name="args">The args for the method, their names as keys and their values as values</param>
        /// <param name="timeout">The timeout of the http request</param>
        /// <returns>The response object</returns>
        public T ApiMethod<T>(string method, Dictionary<string, object> args = null, int timeout = 100)
            => ApiMethodAsync<T>(method, args, timeout).Result;
        /// <summary>
        /// Executes a call to the telegram bot API. Mainly used by the other methods, 
        /// but you can use it for non-implemented methods as well.
        /// </summary>
        /// <typeparam name="T">Type of object being returned</typeparam>
        /// <param name="method">The name of the telegram bot API method to call</param>
        /// <param name="args">The args for the method, their names as keys and their values as values</param>
        /// <param name="timeout">The timeout of the http request</param>
        /// <returns>The response object</returns>
        public async Task<T> ApiMethodAsync<T>(string method, Dictionary<string, object> args = null, int timeout = 100)
        {
            string url = ApiUrl + method + "?";
            if (args != null)
                foreach (var kvp in args)
                {
                    if (kvp.Value is SendFileMultipart sfm)
                    {
                        args.Remove(kvp.Key);
                        return await ApiMethodMultipartAsync<T>(method, args, kvp.Key, sfm, timeout);
                    }
                    if (kvp.Value is InputMedia[] ims && ims.Any(x => x.Media.Type == SendFileType.Attach))
                    {
                        return await ApiMethodManyFileAsync<T>(method, args, timeout);
                    }
                    url += $"{kvp.Key}={Serialize(kvp.Value)}&";
                }
            HttpWebRequest request = WebRequest.CreateHttp(url);
            if (timeout != 0) request.Timeout = timeout;
            WebResponse response = await request.GetResponseAsync();
            return DeserializeResponse<T>(response);
        }

        private async Task<T> ApiMethodMultipartAsync<T>(string method, Dictionary<string, object> args,
            string multipartObjectKey, SendFileMultipart multipartFile, int timeout = 100)
        {
            HttpClient httpClient = new HttpClient();
            if (timeout != 0) httpClient.Timeout = TimeSpan.FromSeconds(timeout);
            MultipartFormDataContent form = new MultipartFormDataContent();
            foreach (var kvp in args)
            {
                form.Add(new StringContent(Serialize(kvp.Value)), kvp.Key);
            }
            form.Add(new StreamContent(multipartFile.FileStream), multipartObjectKey);
            string url = ApiUrl + method;
            return DeserializeResponse<T>(await httpClient.PostAsync(url, form).Result.Content.ReadAsStringAsync());
        }

        private async Task<T> ApiMethodManyFileAsync<T>(string method, Dictionary<string, object> args, int timeout = 100)
        {
            HttpClient httpClient = new HttpClient();
            if (timeout != 0) httpClient.Timeout = TimeSpan.FromSeconds(timeout);
            MultipartFormDataContent form = new MultipartFormDataContent();
            foreach (var kvp in args)
            {
                if (kvp.Value is InputMedia[] ims)
                {
                    foreach (var im in ims)
                    {
                        if (im.Media.Type == SendFileType.Attach)
                        {
                            var sfa = (SendFileAttach)im.Media;
                            form.Add(new StreamContent(sfa.FileStream), sfa.AttachName);
                        }
                    }
                }
                form.Add(new StringContent(Serialize(kvp.Value)), kvp.Key);
            }
            string url = ApiUrl + method;
            return DeserializeResponse<T>(await httpClient.PostAsync(url, form).Result.Content.ReadAsStringAsync());
        }

        private string Serialize(object obj)
        {
            switch (obj)
            {
                case SendFileId sfi:
                    return WebUtility.UrlEncode(sfi.FileId);
                case SendFileUrl sfu:
                    return WebUtility.UrlEncode(sfu.Url);
                case ChatId cId:
                    return WebUtility.UrlEncode(cId.ChannelUsername ?? cId.ChatIdentifier.ToString());
                case string s:
                    return WebUtility.UrlEncode(s);
            }
            return WebUtility.UrlEncode(JsonConvert.SerializeObject(obj));
        }

        private T DeserializeResponse<T>(WebResponse response)
        {
            using (StreamReader sr = new StreamReader(response.GetResponseStream()))
            {
                string responseData = sr.ReadToEnd();
                return DeserializeResponse<T>(responseData);
            }
        }

        private T DeserializeResponse<T>(string response)
        {
            var apiRes = JsonConvert.DeserializeObject<ApiResponse<T>>(response);
            if (!apiRes.Ok) throw GenerateApiException(apiRes);
            return apiRes.ResponseObject;
        }

        private static ApiRequestException GenerateApiException<T>(ApiResponse<T> apiRes)
        {
            //TODO: add more specific exceptions later on
            return new ApiRequestException(apiRes.Description, apiRes.Parameters);
        }
        #endregion

        #region Getting Updates
        /// <summary>
        /// Use this method to receive incoming updates
        /// </summary>
        /// <param name="allowedUpdates">Types of allowed updates</param>
        /// <param name="timeout">The timeout in seconds. To use short polling, set this to 0.</param>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <returns>The new updates</returns>
        public Update[] GetUpdates(UpdateType[] allowedUpdates = null, int timeout = 0, int offset = 0, int limit = 100)
            => GetUpdatesAsync(allowedUpdates, timeout, offset, limit).Result;
        /// <summary>
        /// Use this method to receive incoming updates
        /// </summary>
        /// <param name="allowedUpdates">Types of allowed updates</param>
        /// <param name="timeout">The timeout in seconds. To use short polling, set this to 0.</param>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <returns>The new updates</returns>
        public async Task<Update[]> GetUpdatesAsync(UpdateType[] allowedUpdates = null, int timeout = 0, int offset = 0, int limit = 100)
        {
            Dictionary<string, object> args = new Dictionary<string, object>();
            if (timeout != 0) args.Add("timeout", timeout);
            if (offset != 0) args.Add("offset", offset);
            if (limit != 100) args.Add("limit", limit);
            if (allowedUpdates != null && allowedUpdates.Length > 0) args.Add("allowed_updates",
                allowedUpdates.Select(x => Enum.GetString(x)).ToArray());

            return await ApiMethodAsync<Update[]>("getUpdates", args, timeout + 1);
        }

        /// <summary>
        /// Use this method to specify a url and receive incoming updates via an outgoing webhook.
        /// </summary>
        /// <param name="url">HTTPS url to send updates to. Use an empty string to remove webhook integration</param>
        /// <param name="certificate">Upload your public key certificate so that the root certificate in use can be checked.</param>
        /// <param name="maxConnections">Maximum allowed number of simultaneous HTTPS connections to the webhook for update delivery, 1-100. 
        /// Defaults to 40. Use lower values to limit the load on your bot‘s server, and higher values to increase your bot’s throughput.</param>
        /// <param name="allowedUpdates">The types of updates you want your bot to receive</param>
        /// <returns>True on success</returns>
        public bool SetWebhook(string url, SendFileMultipart certificate = null, int maxConnections = 40, UpdateType[] allowedUpdates = null)
            => SetWebhookAsync(url, certificate, maxConnections, allowedUpdates).Result;
        /// <summary>
        /// Use this method to specify a url and receive incoming updates via an outgoing webhook.
        /// </summary>
        /// <param name="url">HTTPS url to send updates to. Use an empty string to remove webhook integration</param>
        /// <param name="certificate">Upload your public key certificate so that the root certificate in use can be checked.</param>
        /// <param name="maxConnections">Maximum allowed number of simultaneous HTTPS connections to the webhook for update delivery, 1-100. 
        /// Defaults to 40. Use lower values to limit the load on your bot‘s server, and higher values to increase your bot’s throughput.</param>
        /// <param name="allowedUpdates">The types of updates you want your bot to receive</param>
        /// <returns>True on success</returns>
        public async Task<bool> SetWebhookAsync(string url, SendFileMultipart certificate = null,
            int maxConnections = 40, UpdateType[] allowedUpdates = null)
        {
            Dictionary<string, object> args = new Dictionary<string, object>() { { "url", url } };
            if (certificate != null) args.Add("certificate", certificate);
            if (maxConnections != 40) args.Add("max_connections", maxConnections);
            if (allowedUpdates != null && allowedUpdates.Length > 0) args.Add("allowed_updates",
                allowedUpdates.Select(x => Enum.GetString(x)).ToArray());

            return await ApiMethodAsync<bool>("setWebhook", args);
        }

        /// <summary>
        /// Use this method to remove webhook integration if you decide to switch back to getUpdates. Returns True on success. Requires no parameters.
        /// </summary>
        /// <returns>True on success</returns>
        public bool DeleteWebhook() => DeleteWebhookAsync().Result;
        /// <summary>
        /// Use this method to remove webhook integration if you decide to switch back to getUpdates. Returns True on success. Requires no parameters.
        /// </summary>
        /// <returns>True on success</returns>
        public async Task<bool> DeleteWebhookAsync()
        {
            return await ApiMethodAsync<bool>("deleteWebhook");
        }

        /// <summary>
        /// Use this method to get current webhook status. Requires no parameters. 
        /// On success, returns a WebhookInfo object. If the bot is using getUpdates, will return an object with the url field empty.
        /// </summary>
        /// <returns>A <see cref="WebhookInfo"/> object</returns>
        public WebhookInfo GetWebhookInfo() => GetWebhookInfoAsync().Result;
        /// <summary>
        /// Use this method to get current webhook status. Requires no parameters. 
        /// On success, returns a WebhookInfo object. If the bot is using getUpdates, will return an object with the url field empty.
        /// </summary>
        /// <returns>A <see cref="WebhookInfo"/> object</returns>
        public async Task<WebhookInfo> GetWebhookInfoAsync()
        {
            return await ApiMethodAsync<WebhookInfo>("getWebhookInfo");
        }
        #endregion
        #region Available Methods
        #region GetMe
        /// <summary>
        /// A simple method for testing your bot's auth token. Requires no parameters. 
        /// Returns basic information about the bot in form of a User object.
        /// </summary>
        /// <returns>Returns basic information about the bot in form of a User object.</returns>
        public User GetMe() => GetMeAsync().Result;
        /// <summary>
        /// A simple method for testing your bot's auth token. Requires no parameters. 
        /// Returns basic information about the bot in form of a User object.
        /// </summary>
        /// <returns>Returns basic information about the bot in form of a User object.</returns>
        public async Task<User> GetMeAsync()
        {
            return await ApiMethodAsync<User>("getMe");
        }
        #endregion
        #region Sending messages
        /// <summary>
        /// Use this method to send text messages. On success, the sent Message is returned.
        /// </summary>
        /// <param name="chatId">The id or channel username of the chat to send the message to</param>
        /// <param name="text">The text of the message</param>
        /// <param name="parseMode">The parse mode of the message (see <see cref="ParseMode"/>)</param>
        /// <param name="disableWebPagePreview">If this is true, no website preview will be shown</param>
        /// <param name="disableNotification">If this is true, users will not receive a notification or a silent one for this message</param>
        /// <param name="replyToMessageId">The message id of the message to reply to in this chat, if any</param>
        /// <param name="replyMarkup">A <see cref="ReplyMarkupBase"/>.</param>
        /// <returns>The sent message</returns>
        public Message SendTextMessage(ChatId chatId, string text, ParseMode parseMode = ParseMode.None,
            bool disableWebPagePreview = false, bool disableNotification = false, int replyToMessageId = -1,
            ReplyMarkupBase replyMarkup = null)
            => SendTextMessageAsync(chatId, text, parseMode, disableWebPagePreview,
                disableNotification, replyToMessageId, replyMarkup).Result;
        /// <summary>
        /// Use this method to send text messages. On success, the sent Message is returned.
        /// </summary>
        /// <param name="chatId">The id or channel username of the chat to send the message to</param>
        /// <param name="text">The text of the message</param>
        /// <param name="parseMode">The parse mode of the message (see <see cref="ParseMode"/>)</param>
        /// <param name="disableWebPagePreview">If this is true, no website preview will be shown</param>
        /// <param name="disableNotification">If this is true, users will not receive a notification or a silent one for this message</param>
        /// <param name="replyToMessageId">The message id of the message to reply to in this chat, if any</param>
        /// <param name="replyMarkup">A <see cref="ReplyMarkupBase"/>.</param>
        /// <returns>The sent message</returns>
        public async Task<Message> SendTextMessageAsync(ChatId chatId, string text, ParseMode parseMode = ParseMode.None,
            bool disableWebPagePreview = false, bool disableNotification = false, int replyToMessageId = -1,
            ReplyMarkupBase replyMarkup = null)
        {
            Dictionary<string, object> args = new Dictionary<string, object>() { { "chat_id", chatId }, { "text", text } };
            if (parseMode != ParseMode.None) args.Add("parse_mode", Enum.GetString(parseMode));
            if (disableWebPagePreview) args.Add("disable_web_page_preview", true);
            if (disableNotification) args.Add("disable_notification", true);
            if (replyToMessageId != -1) args.Add("reply_to_message_id", replyToMessageId);
            if (replyMarkup != null) args.Add("reply_markup", replyMarkup);

            return await ApiMethodAsync<Message>("sendMessage", args);
        }

        /// <summary>
        /// Use this method to forward messages of any kind. On success, the sent Message is returned.
        /// </summary>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel (in the format @channelusername)</param>
        /// <param name="fromChatId">Unique identifier for the chat where the original message was sent (or channel username in the format @channelusername)</param>
        /// <param name="messageId">Message identifier in the chat specified in <paramref name="fromChatId"/></param>
        /// <param name="disableNotification">Sends the message silently. Users will receive a notification with no sound.</param>
        /// <returns>The sent message</returns>
        public Message ForwardMessage(ChatId chatId, ChatId fromChatId, int messageId, bool disableNotification = false)
            => ForwardMessageAsync(chatId, fromChatId, messageId, disableNotification).Result;
        /// <summary>
        /// Use this method to forward messages of any kind. On success, the sent Message is returned.
        /// </summary>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel (in the format @channelusername)</param>
        /// <param name="fromChatId">Unique identifier for the chat where the original message was sent (or channel username in the format @channelusername)</param>
        /// <param name="messageId">Message identifier in the chat specified in <paramref name="fromChatId"/></param>
        /// <param name="disableNotification">Sends the message silently. Users will receive a notification with no sound.</param>
        /// <returns>The sent message</returns>
        public async Task<Message> ForwardMessageAsync(ChatId chatId, ChatId fromChatId, int messageId, bool disableNotification = false)
        {
            Dictionary<string, object> args = new Dictionary<string, object>()
            { { "chat_id", chatId }, { "from_chat_id", fromChatId }, { "message_id", messageId } };
            if (disableNotification) args.Add("disable_notification", true);

            return await ApiMethodAsync<Message>("forwardMessage", args);
        }

        /// <summary>
        /// Use this method to send photos. On success, the sent Message is returned.
        /// </summary>
        /// <param name="chatId">The chat id or channel username of the chat to send the photo to</param>
        /// <param name="photo">The photo to send. Either one of <see cref="SendFileId"/>, 
        /// <see cref="SendFileUrl"/> or <see cref="SendFileMultipart"/>. An implicit operator from string exists.</param>
        /// <param name="caption">The caption of the photo, if any</param>
        /// <param name="parseMode">The parse mode of the caption, if any</param>
        /// <param name="disableNotification">If this is true, the user will receive a notification with no sound</param>
        /// <param name="replyToMessageId">The messageId of the message to reply to, if any</param>
        /// <param name="replyMarkup">The reply markup. Additional interface options.</param>
        /// <returns>The sent message on success</returns>
        public Message SendPhoto(ChatId chatId, SendFile photo, string caption = null,
            ParseMode parseMode = ParseMode.None, bool disableNotification = false,
            int replyToMessageId = -1, ReplyMarkupBase replyMarkup = null)
            => SendPhotoAsync(chatId, photo, caption, parseMode, disableNotification, replyToMessageId, replyMarkup).Result;
        /// <summary>
        /// Use this method to send photos. On success, the sent Message is returned.
        /// </summary>
        /// <param name="chatId">The chat id or channel username of the chat to send the photo to</param>
        /// <param name="photo">The photo to send. Either one of <see cref="SendFileId"/>, 
        /// <see cref="SendFileUrl"/> or <see cref="SendFileMultipart"/>. An implicit operator from string exists.</param>
        /// <param name="caption">The caption of the photo, if any</param>
        /// <param name="parseMode">The parse mode of the caption, if any</param>
        /// <param name="disableNotification">If this is true, the user will receive a notification with no sound</param>
        /// <param name="replyToMessageId">The messageId of the message to reply to, if any</param>
        /// <param name="replyMarkup">The reply markup. Additional interface options.</param>
        /// <returns>The sent message on success</returns>
        public async Task<Message> SendPhotoAsync(ChatId chatId, SendFile photo, string caption = null,
            ParseMode parseMode = ParseMode.None, bool disableNotification = false,
            int replyToMessageId = -1, ReplyMarkupBase replyMarkup = null)
        {
            Dictionary<string, object> args = new Dictionary<string, object>() { { "chat_id", chatId }, { "photo", photo } };
            if (!string.IsNullOrWhiteSpace(caption)) args.Add("caption", caption);
            if (parseMode != ParseMode.None) args.Add("parse_mode", Enum.GetString(parseMode));
            if (disableNotification) args.Add("disable_notification", true);
            if (replyToMessageId != -1) args.Add("reply_to_message_id", replyToMessageId);
            if (replyMarkup != null) args.Add("reply_markup", replyMarkup);

            return await ApiMethodAsync<Message>("sendPhoto", args);
        }

        /// <summary>
        /// Use this method to send audio files, if you want Telegram clients to display them in the music player. 
        /// Your audio must be in the .mp3 format. On success, the sent Message is returned.
        /// </summary>
        /// <param name="chatId">The chat id or channel username of the chat to send the message to</param>
        /// <param name="audio">The audio file. Either one of <see cref="SendFileId"/>, 
        /// <see cref="SendFileUrl"/> or <see cref="SendFileMultipart"/></param>
        /// <param name="caption">The caption of the audio file, if any</param>
        /// <param name="parseMode">The parse mode of the caption, if any</param>
        /// <param name="duration">Duration of the audio in seconds</param>
        /// <param name="performer">Performer</param>
        /// <param name="title">Track name</param>
        /// <param name="thumb">Thumbnail of the file sent. The thumbnail should be in JPEG format and less than 200 kB in size. 
        /// A thumbnail‘s width and height should not exceed 90. Ignored if the file is not uploaded using multipart/form-data. 
        /// Thumbnails can’t be reused and can be only uploaded as a new file, so you can pass “attach://&lt;file_attach_name&gt;” 
        /// if the thumbnail was uploaded using multipart/form-data under &lt;file_attach_name&gt;.</param>
        /// <param name="disableNotification">Sends the message silently. Users will receive a notification with no sound.</param>
        /// <param name="replyToMessageId">The messageId of the message to reply to, if any</param>
        /// <param name="replyMarkup">The reply markup. Additional interface options.</param>
        /// <returns>The sent message on success</returns>
        public Message SendAudio(ChatId chatId, SendFile audio, string caption = null,
            ParseMode parseMode = ParseMode.None, int duration = 0, string performer = null, string title = null, SendFile thumb = null,
            bool disableNotification = false, int replyToMessageId = -1, ReplyMarkupBase replyMarkup = null)
            => SendAudioAsync(chatId, audio, caption, parseMode, duration, performer, title, thumb, disableNotification, replyToMessageId,
                replyMarkup).Result;
        /// <summary>
        /// Use this method to send audio files, if you want Telegram clients to display them in the music player. 
        /// Your audio must be in the .mp3 format. On success, the sent Message is returned.
        /// </summary>
        /// <param name="chatId">The chat id or channel username of the chat to send the message to</param>
        /// <param name="audio">The audio file. Either one of <see cref="SendFileId"/>, 
        /// <see cref="SendFileUrl"/> or <see cref="SendFileMultipart"/></param>
        /// <param name="caption">The caption of the audio file, if any</param>
        /// <param name="parseMode">The parse mode of the caption, if any</param>
        /// <param name="duration">Duration of the audio in seconds</param>
        /// <param name="performer">Performer</param>
        /// <param name="title">Track name</param>
        /// /// <param name="thumb">Thumbnail of the file sent. The thumbnail should be in JPEG format and less than 200 kB in size. 
        /// A thumbnail‘s width and height should not exceed 90. Ignored if the file is not uploaded using multipart/form-data. 
        /// Thumbnails can’t be reused and can be only uploaded as a new file, so you can pass “attach://&lt;file_attach_name&gt;” 
        /// if the thumbnail was uploaded using multipart/form-data under &lt;file_attach_name&gt;.</param>
        /// <param name="disableNotification">Sends the message silently. Users will receive a notification with no sound.</param>
        /// <param name="replyToMessageId">The messageId of the message to reply to, if any</param>
        /// <param name="replyMarkup">The reply markup. Additional interface options.</param>
        /// <returns>The sent message on success</returns>
        public async Task<Message> SendAudioAsync(ChatId chatId, SendFile audio, string caption = null,
            ParseMode parseMode = ParseMode.None, int duration = 0, string performer = null, string title = null, SendFile thumb = null,
            bool disableNotification = false, int replyToMessageId = -1, ReplyMarkupBase replyMarkup = null)
        {
            Dictionary<string, object> args = new Dictionary<string, object>() { { "chat_id", chatId }, { "audio", audio } };
            if (!string.IsNullOrWhiteSpace(caption)) args.Add("caption", caption);
            if (parseMode != ParseMode.None) args.Add("parse_mode", Enum.GetString(parseMode));
            if (duration != 0) args.Add("duration", duration);
            if (!string.IsNullOrWhiteSpace(performer)) args.Add("performer", performer);
            if (!string.IsNullOrWhiteSpace(title)) args.Add("title", title);
            if (thumb != null) args.Add("thumb", thumb);
            if (disableNotification) args.Add("disable_notification", true);
            if (replyToMessageId != -1) args.Add("reply_to_message_id", replyToMessageId);
            if (replyMarkup != null) args.Add("reply_markup", replyMarkup);

            return await ApiMethodAsync<Message>("sendAudio", args);
        }

        /// <summary>
        /// Use this method to send general files. On success, the sent Message is returned. 
        /// Bots can currently send files of any type of up to 50 MB in size, this limit may be changed in the future.
        /// </summary>
        /// <param name="chatId">The chat id or channel username of the chat to send the message to</param>
        /// <param name="document">The document to send. One of <see cref="SendFileId"/>, <see cref="SendFileUrl"/>
        ///  or <see cref="SendFileMultipart"/></param>
        /// <param name="thumb">Thumbnail of the file sent. The thumbnail should be in JPEG format and less than 200 kB in size. 
        /// A thumbnail‘s width and height should not exceed 90. Ignored if the file is not uploaded using multipart/form-data. 
        /// Thumbnails can’t be reused and can be only uploaded as a new file, so you can pass “attach://&lt;file_attach_name&gt;” 
        /// if the thumbnail was uploaded using multipart/form-data under &lt;file_attach_name&gt;.</param>
        /// <param name="caption">The caption of the document, if any</param>
        /// <param name="parseMode">The parse mode of the message, if any</param>
        /// <param name="disableNotification">If this is true, the user will receive a notification without sound</param>
        /// <param name="replyToMessageId">The message id of the message to reply to in the target chat, if any</param>
        /// <param name="replyMarkup">The reply markup. Additional interface options.</param>
        /// <returns>The sent message on success</returns>
        public Message SendDocument(ChatId chatId, SendFile document, SendFile thumb = null, string caption = null,
            ParseMode parseMode = ParseMode.None, bool disableNotification = false, int replyToMessageId = -1,
            ReplyMarkupBase replyMarkup = null)
            => SendDocumentAsync(chatId, document, thumb, caption, parseMode, disableNotification, replyToMessageId, replyMarkup).Result;
        /// <summary>
        /// Use this method to send general files. On success, the sent Message is returned. 
        /// Bots can currently send files of any type of up to 50 MB in size, this limit may be changed in the future.
        /// </summary>
        /// <param name="chatId">The chat id or channel username of the chat to send the message to</param>
        /// <param name="document">The document to send. One of <see cref="SendFileId"/>, <see cref="SendFileUrl"/>
        ///  or <see cref="SendFileMultipart"/></param>
        /// <param name="thumb">Thumbnail of the file sent. The thumbnail should be in JPEG format and less than 200 kB in size. 
        /// A thumbnail‘s width and height should not exceed 90. Ignored if the file is not uploaded using multipart/form-data. 
        /// Thumbnails can’t be reused and can be only uploaded as a new file, so you can pass “attach://&lt;file_attach_name&gt;” 
        /// if the thumbnail was uploaded using multipart/form-data under &lt;file_attach_name&gt;.</param>
        /// <param name="caption">The caption of the document, if any</param>
        /// <param name="parseMode">The parse mode of the message, if any</param>
        /// <param name="disableNotification">If this is true, the user will receive a notification without sound</param>
        /// <param name="replyToMessageId">The message id of the message to reply to in the target chat, if any</param>
        /// <param name="replyMarkup">The reply markup. Additional interface options.</param>
        /// <returns>The sent message on success</returns>
        public async Task<Message> SendDocumentAsync(ChatId chatId, SendFile document, SendFile thumb = null, string caption = null,
            ParseMode parseMode = ParseMode.None, bool disableNotification = false, int replyToMessageId = -1,
            ReplyMarkupBase replyMarkup = null)
        {
            Dictionary<string, object> args = new Dictionary<string, object>() { { "chat_id", chatId }, { "document", document } };
            if (thumb != null) args.Add("thumb", thumb);
            if (!string.IsNullOrWhiteSpace(caption)) args.Add("caption", caption);
            if (parseMode != ParseMode.None) args.Add("parse_mode", Enum.GetString(parseMode));
            if (disableNotification) args.Add("disable_notification", true);
            if (replyToMessageId != -1) args.Add("reply_to_message_id", replyToMessageId);
            if (replyMarkup != null) args.Add("reply_markup", replyMarkup);

            return await ApiMethodAsync<Message>("sendDocument", args);
        }

        /// <summary>
        /// Use this method to send video files, Telegram clients support mp4 videos (other formats may be sent as Document). On success, the sent Message is returned. 
        /// Bots can currently send video files of up to 50 MB in size, this limit may be changed in the future.
        /// </summary>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel 
        /// (in the format @channelusername)</param>
        /// <param name="video">The video to send. One of <see cref="SendFileId"/>, <see cref="SendFileUrl"/>
        ///  or <see cref="SendFileMultipart"/></param>
        /// <param name="duration">Optional. The duration of the video in seconds</param>
        /// <param name="width">Optional. Width of the video</param>
        /// <param name="height">Optional. Height of the video</param>
        /// <param name="thumb">Thumbnail of the file sent. The thumbnail should be in JPEG format and less than 200 kB in size. 
        /// A thumbnail‘s width and height should not exceed 90. Ignored if the file is not uploaded using multipart/form-data. 
        /// Thumbnails can’t be reused and can be only uploaded as a new file, so you can pass “attach://&lt;file_attach_name&gt;” 
        /// if the thumbnail was uploaded using multipart/form-data under &lt;file_attach_name&gt;.</param>
        /// <param name="caption">Optional. Caption for the video message</param>
        /// <param name="parseMode">Optional. Parse mode for the message caption</param>
        /// <param name="supportsStreaming">Pass true if the uploaded video is suitable for streaming</param>
        /// <param name="disableNotification">If this is true, users will receive a silent notification</param>
        /// <param name="replyToMessageId">The message id of the message to reply to, if any</param>
        /// <param name="replyMarkup">The reply markup. Additional interface options.</param>
        /// <returns>The sent message on success</returns>
        public Message SendVideo(ChatId chatId, SendFile video, int duration = 0, int width = 0, int height = 0, SendFile thumb = null,
            string caption = null, ParseMode parseMode = ParseMode.None, bool supportsStreaming = false,
            bool disableNotification = false, int replyToMessageId = -1, ReplyMarkupBase replyMarkup = null)
            => SendVideoAsync(chatId, video, duration, width, height, thumb, caption, parseMode, supportsStreaming, disableNotification,
                replyToMessageId, replyMarkup).Result;
        /// <summary>
        /// Use this method to send video files, Telegram clients support mp4 videos (other formats may be sent as Document). On success, the sent Message is returned. 
        /// Bots can currently send video files of up to 50 MB in size, this limit may be changed in the future.
        /// </summary>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel 
        /// (in the format @channelusername)</param>
        /// <param name="video">The video to send. One of <see cref="SendFileId"/>, <see cref="SendFileUrl"/>
        ///  or <see cref="SendFileMultipart"/></param>
        /// <param name="duration">Optional. The duration of the video in seconds</param>
        /// <param name="width">Optional. Width of the video</param>
        /// <param name="height">Optional. Height of the video</param>
        /// <param name="caption">Optional. Caption for the video message</param>
        /// <param name="parseMode">Optional. Parse mode for the message caption</param>
        /// <param name="supportsStreaming">Pass true if the uploaded video is suitable for streaming</param>
        /// <param name="disableNotification">If this is true, users will receive a silent notification</param>
        /// <param name="replyToMessageId">The message id of the message to reply to, if any</param>
        /// <param name="replyMarkup">The reply markup. Additional interface options.</param>
        /// <returns>The sent message on success</returns>
        public async Task<Message> SendVideoAsync(ChatId chatId, SendFile video, int duration = 0, int width = 0, int height = 0, SendFile thumb = null,
            string caption = null, ParseMode parseMode = ParseMode.None, bool supportsStreaming = false,
            bool disableNotification = false, int replyToMessageId = -1, ReplyMarkupBase replyMarkup = null)
        {
            Dictionary<string, object> args = new Dictionary<string, object>() { { "chat_id", chatId }, { "video", video } };
            if (duration != 0) args.Add("duration", duration);
            if (width != 0) args.Add("width", width);
            if (height != 0) args.Add("height", height);
            if (thumb != null) args.Add("thumb", thumb);
            if (!string.IsNullOrWhiteSpace(caption)) args.Add("caption", caption);
            if (parseMode != ParseMode.None) args.Add("parse_mode", Enum.GetString(parseMode));
            if (supportsStreaming) args.Add("supports_streaming", true);
            if (disableNotification) args.Add("disable_notification", true);
            if (replyToMessageId != -1) args.Add("reply_to_message_id", replyToMessageId);
            if (replyMarkup != null) args.Add("reply_markup", replyMarkup);

            return await ApiMethodAsync<Message>("sendVideo", args);
        }

        /// <summary>
        /// Use this method to send animation files (GIF or H.264/MPEG-4 AVC video without sound). On success, the sent Message is returned. 
        /// Bots can currently send animation files of up to 50 MB in size, this limit may be changed in the future.
        /// </summary>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel 
        /// (in the format @channelusername)</param>
        /// <param name="animation">The animation to send. One of <see cref="SendFileId"/>, <see cref="SendFileUrl"/>
        ///  or <see cref="SendFileMultipart"/></param>
        /// <param name="duration">Optional. The duration of the animation in seconds</param>
        /// <param name="width">Optional. Width of the animation</param>
        /// <param name="height">Optional. Height of the animation</param>
        /// <param name="thumb">Thumbnail of the file sent. The thumbnail should be in JPEG format and less than 200 kB in size. 
        /// A thumbnail‘s width and height should not exceed 90. Ignored if the file is not uploaded using multipart/form-data. 
        /// Thumbnails can’t be reused and can be only uploaded as a new file, so you can pass “attach://&lt;file_attach_name&gt;” 
        /// if the thumbnail was uploaded using multipart/form-data under &lt;file_attach_name&gt;.</param>
        /// <param name="caption">Optional. Caption for the animation message</param>
        /// <param name="parseMode">Optional. Parse mode for the animation caption</param>
        /// <param name="disableNotification">If this is true, users will receive a silent notification</param>
        /// <param name="replyToMessageId">The message id of the message to reply to, if any</param>
        /// <param name="replyMarkup">The reply markup. Additional interface options.</param>
        /// <returns>The sent message on success</returns>
        public Message SendAnimation(ChatId chatId, SendFile animation, int duration = 0, int width = 0, int height = 0, SendFile thumb = null,
            string caption = null, ParseMode parseMode = ParseMode.None, bool disableNotification = false,
            int replyToMessageId = -1, ReplyMarkupBase replyMarkup = null)
            => SendAnimationAsync(chatId, animation, duration, width, height, thumb, caption, parseMode, disableNotification,
                replyToMessageId, replyMarkup).Result;
        /// <summary>
        /// Use this method to send animation files (GIF or H.264/MPEG-4 AVC video without sound). On success, the sent Message is returned. 
        /// Bots can currently send animation files of up to 50 MB in size, this limit may be changed in the future.
        /// </summary>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel 
        /// (in the format @channelusername)</param>
        /// <param name="animation">The animation to send. One of <see cref="SendFileId"/>, <see cref="SendFileUrl"/>
        ///  or <see cref="SendFileMultipart"/></param>
        /// <param name="duration">Optional. The duration of the animation in seconds</param>
        /// <param name="width">Optional. Width of the animation</param>
        /// <param name="height">Optional. Height of the animation</param>
        /// <param name="thumb">Thumbnail of the file sent. The thumbnail should be in JPEG format and less than 200 kB in size. 
        /// A thumbnail‘s width and height should not exceed 90. Ignored if the file is not uploaded using multipart/form-data. 
        /// Thumbnails can’t be reused and can be only uploaded as a new file, so you can pass “attach://&lt;file_attach_name&gt;” 
        /// if the thumbnail was uploaded using multipart/form-data under &lt;file_attach_name&gt;.</param>
        /// <param name="caption">Optional. Caption for the animation message</param>
        /// <param name="parseMode">Optional. Parse mode for the animation caption</param>
        /// <param name="disableNotification">If this is true, users will receive a silent notification</param>
        /// <param name="replyToMessageId">The message id of the message to reply to, if any</param>
        /// <param name="replyMarkup">The reply markup. Additional interface options.</param>
        /// <returns>The sent message on success</returns>
        public async Task<Message> SendAnimationAsync(ChatId chatId, SendFile animation, int duration = 0, int width = 0, int height = 0, SendFile thumb = null,
            string caption = null, ParseMode parseMode = ParseMode.None, bool disableNotification = false,
            int replyToMessageId = -1, ReplyMarkupBase replyMarkup = null)
        {
            Dictionary<string, object> args = new Dictionary<string, object>() { { "chat_id", chatId }, { "animation", animation } };
            if (duration != 0) args.Add("duration", duration);
            if (width != 0) args.Add("width", width);
            if (height != 0) args.Add("height", height);
            if (thumb != null) args.Add("thumb", thumb);
            if (!string.IsNullOrWhiteSpace(caption)) args.Add("caption", caption);
            if (parseMode != ParseMode.None) args.Add("parse_mode", Enum.GetString(parseMode));
            if (disableNotification) args.Add("disable_notification", true);
            if (replyToMessageId != -1) args.Add("reply_to_message_id", replyToMessageId);
            if (replyMarkup != null) args.Add("reply_markup", replyMarkup);

            return await ApiMethodAsync<Message>("sendAnimation", args);
        }

        /// <summary>
        /// Use this method to send audio files, if you want Telegram clients to display the file as a playable voice message. 
        /// For this to work, your audio must be in an .ogg file encoded with OPUS (other formats may be sent as Audio or Document). 
        /// On success, the sent Message is returned. 
        /// Bots can currently send voice messages of up to 50 MB in size, this limit may be changed in the future.
        /// </summary>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel 
        /// (in the format @channelusername)</param>
        /// <param name="voice">The voice message to send. One of <see cref="SendFileId"/>, <see cref="SendFileUrl"/>
        ///  or <see cref="SendFileMultipart"/></param>
        /// <param name="caption">Optional. Caption for the video message</param>
        /// <param name="parseMode">Optional. Parse mode for the message caption</param>
        /// <param name="duration">Optional. Duration of the voice message in seconds.</param>
        /// <param name="disableNotification">If this is true, users will receive a silent notification</param>
        /// <param name="replyToMessageId">The message id of the message to reply to, if any</param>
        /// <param name="replyMarkup">The reply markup. Additional interface options.</param>
        /// <returns>The sent message on success</returns>
        public Message SendVoice(ChatId chatId, SendFile voice, string caption = null,
            ParseMode parseMode = ParseMode.None, int duration = 0, bool disableNotification = false,
            int replyToMessageId = -1, ReplyMarkupBase replyMarkup = null)
            => SendVoiceAsync(chatId, voice, caption, parseMode, duration, disableNotification, replyToMessageId, replyMarkup).Result;
        /// <summary>
        /// Use this method to send audio files, if you want Telegram clients to display the file as a playable voice message. 
        /// For this to work, your audio must be in an .ogg file encoded with OPUS (other formats may be sent as Audio or Document). 
        /// On success, the sent Message is returned. 
        /// Bots can currently send voice messages of up to 50 MB in size, this limit may be changed in the future.
        /// </summary>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel 
        /// (in the format @channelusername)</param>
        /// <param name="voice">The voice message to send. One of <see cref="SendFileId"/>, <see cref="SendFileUrl"/>
        ///  or <see cref="SendFileMultipart"/></param>
        /// <param name="caption">Optional. Caption for the video message</param>
        /// <param name="parseMode">Optional. Parse mode for the message caption</param>
        /// <param name="duration">Optional. Duration of the voice message in seconds.</param>
        /// <param name="disableNotification">If this is true, users will receive a silent notification</param>
        /// <param name="replyToMessageId">The message id of the message to reply to, if any</param>
        /// <param name="replyMarkup">The reply markup. Additional interface options.</param>
        /// <returns>The sent message on success</returns>
        public async Task<Message> SendVoiceAsync(ChatId chatId, SendFile voice, string caption = null,
            ParseMode parseMode = ParseMode.None, int duration = 0, bool disableNotification = false,
            int replyToMessageId = -1, ReplyMarkupBase replyMarkup = null)
        {
            Dictionary<string, object> args = new Dictionary<string, object>() { { "chat_id", chatId }, { "voice", voice } };
            if (!string.IsNullOrWhiteSpace(caption)) args.Add("caption", caption);
            if (parseMode != ParseMode.None) args.Add("parse_mode", Enum.GetString(parseMode));
            if (duration != 0) args.Add("duration", duration);
            if (disableNotification) args.Add("disable_notification", true);
            if (replyToMessageId != -1) args.Add("reply_to_message_id", replyToMessageId);
            if (replyMarkup != null) args.Add("reply_markup", replyMarkup);

            return await ApiMethodAsync<Message>("sendVoice", args);
        }

        /// <summary>
        /// As of v.4.0, Telegram clients support rounded square mp4 videos of up to 1 minute long. 
        /// Use this method to send video messages. On success, the sent Message is returned.
        /// </summary>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel 
        /// (in the format @channelusername)</param>
        /// <param name="videoNote">The video note to send. One of <see cref="SendFileId"/> or <see cref="SendFileMultipart"/>.
        /// <see cref="SendFileUrl"/> is currently unsupported for video notes.</param>
        /// <param name="duration">Optional. Duration of the video note in seconds</param>
        /// <param name="length">Optional. Height and width of the video</param>
        /// <param name="thumb">Thumbnail of the file sent. The thumbnail should be in JPEG format and less than 200 kB in size. 
        /// A thumbnail‘s width and height should not exceed 90. Ignored if the file is not uploaded using multipart/form-data. 
        /// Thumbnails can’t be reused and can be only uploaded as a new file, so you can pass “attach://&lt;file_attach_name&gt;” 
        /// if the thumbnail was uploaded using multipart/form-data under &lt;file_attach_name&gt;.</param>
        /// <param name="disableNotification">If this is true, users will receive a silent notification</param>
        /// <param name="replyToMessageId">The message id of the message to reply to, if any</param>
        /// <param name="replyMarkup">The reply markup. Additional interface options.</param>
        /// <returns>The sent message on success</returns>
        public Message SendVideoNote(ChatId chatId, SendFile videoNote, int duration = 0, int length = 0, SendFile thumb = null,
            bool disableNotification = false, int replyToMessageId = -1, ReplyMarkupBase replyMarkup = null)
            => SendVideoNoteAsync(chatId, videoNote, duration, length, thumb, disableNotification, replyToMessageId, replyMarkup).Result;
        /// <summary>
        /// As of v.4.0, Telegram clients support rounded square mp4 videos of up to 1 minute long. 
        /// Use this method to send video messages. On success, the sent Message is returned.
        /// </summary>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel 
        /// (in the format @channelusername)</param>
        /// <param name="videoNote">The video note to send. One of <see cref="SendFileId"/> or <see cref="SendFileMultipart"/>.
        /// <see cref="SendFileUrl"/> is currently unsupported for video notes.</param>
        /// <param name="duration">Optional. Duration of the video note in seconds</param>
        /// <param name="length">Optional. Height and width of the video</param>
        /// <param name="thumb">Thumbnail of the file sent. The thumbnail should be in JPEG format and less than 200 kB in size. 
        /// A thumbnail‘s width and height should not exceed 90. Ignored if the file is not uploaded using multipart/form-data. 
        /// Thumbnails can’t be reused and can be only uploaded as a new file, so you can pass “attach://&lt;file_attach_name&gt;” 
        /// if the thumbnail was uploaded using multipart/form-data under &lt;file_attach_name&gt;.</param>
        /// <param name="disableNotification">If this is true, users will receive a silent notification</param>
        /// <param name="replyToMessageId">The message id of the message to reply to, if any</param>
        /// <param name="replyMarkup">The reply markup. Additional interface options.</param>
        /// <returns>The sent message on success</returns>
        public async Task<Message> SendVideoNoteAsync(ChatId chatId, SendFile videoNote, int duration = 0, int length = 0, SendFile thumb = null,
            bool disableNotification = false, int replyToMessageId = -1, ReplyMarkupBase replyMarkup = null)
        {
            Dictionary<string, object> args = new Dictionary<string, object>() { { "chat_id", chatId }, { "video_note", videoNote } };
            if (duration != 0) args.Add("duration", duration);
            if (length != 0) args.Add("length", length);
            if (thumb != null) args.Add("thumb", thumb);
            if (disableNotification) args.Add("disable_notification", true);
            if (replyToMessageId != -1) args.Add("reply_to_message_id", replyToMessageId);
            if (replyMarkup != null) args.Add("reply_markup", replyMarkup);

            return await ApiMethodAsync<Message>("sendVideoNote", args);
        }

        /// <summary>
        /// Use this method to send a group of photos or videos as an album. On success, an array of the sent Messages is returned.
        /// </summary>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel
        /// (in the format @channelusername)</param>
        /// <param name="media">Array of <see cref="InputMediaPhoto"/> and <see cref="InputMediaVideo"/>. 
        /// Must contain 2-10 elements.</param>
        /// <param name="disableNotification">If this is true, users will receive a silent notification</param>
        /// <param name="replyToMessageId">The message id of the message to reply to, if any</param>
        /// <returns>The sent messages on success</returns>
        public Message[] SendMediaGroup(ChatId chatId, InputMedia[] media, bool disableNotification = false,
            int replyToMessageId = -1)
            => SendMediaGroupAsync(chatId, media, disableNotification, replyToMessageId).Result;
        /// <summary>
        /// Use this method to send a group of photos or videos as an album. On success, an array of the sent Messages is returned.
        /// </summary>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel
        /// (in the format @channelusername)</param>
        /// <param name="media">Array of <see cref="InputMediaPhoto"/> and <see cref="InputMediaVideo"/>. 
        /// Must contain 2-10 elements.</param>
        /// <param name="disableNotification">If this is true, users will receive a silent notification</param>
        /// <param name="replyToMessageId">The message id of the message to reply to, if any</param>
        /// <returns>The sent messages on success</returns>
        public async Task<Message[]> SendMediaGroupAsync(ChatId chatId, InputMedia[] media, bool disableNotification = false,
            int replyToMessageId = -1)
        {
            Dictionary<string, object> args = new Dictionary<string, object>() { { "chat_id", chatId }, { "media", media } };
            if (disableNotification) args.Add("disable_notification", true);
            if (replyToMessageId != -1) args.Add("reply_to_message_id", replyToMessageId);

            return await ApiMethodAsync<Message[]>("sendMediaGroup", args);
        }

        /// <summary>
        /// Use this method to send point on the map. On success, the sent Message is returned.
        /// </summary>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel
        /// (in the format @channelusername)</param>
        /// <param name="latitude">Latitude for the location</param>
        /// <param name="longitude">Longitude for the location</param>
        /// <param name="livePeriod">Period in seconds for which the location will be updated (should be between 60 and 86400).</param>
        /// <param name="disableNotification">If this is true, users will receive a silent notification</param>
        /// <param name="replyToMessageId">The message id of the message to reply to, if any</param>
        /// <param name="replyMarkup">The reply markup. Additional interface options.</param>
        /// <returns>The sent message on success</returns>
        public Message SendLocation(ChatId chatId, double latitude, double longitude, int livePeriod = 0,
            bool disableNotification = false, int replyToMessageId = -1, ReplyMarkupBase replyMarkup = null)
            => SendLocationAsync(chatId, latitude, longitude, livePeriod, disableNotification, replyToMessageId, replyMarkup).Result;
        /// <summary>
        /// Use this method to send point on the map. On success, the sent Message is returned.
        /// </summary>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel
        /// (in the format @channelusername)</param>
        /// <param name="latitude">Latitude for the location</param>
        /// <param name="longitude">Longitude for the location</param>
        /// <param name="livePeriod">Period in seconds for which the location will be updated (should be between 60 and 86400).</param>
        /// <param name="disableNotification">If this is true, users will receive a silent notification</param>
        /// <param name="replyToMessageId">The message id of the message to reply to, if any</param>
        /// <param name="replyMarkup">The reply markup. Additional interface options.</param>
        /// <returns>The sent message on success</returns>
        public async Task<Message> SendLocationAsync(ChatId chatId, double latitude, double longitude, int livePeriod = 0,
            bool disableNotification = false, int replyToMessageId = -1, ReplyMarkupBase replyMarkup = null)
        {
            Dictionary<string, object> args = new Dictionary<string, object>() { { "chat_id", chatId },
                { "latitude", latitude }, { "longitude", longitude } };
            if (livePeriod != 0) args.Add("live_period", livePeriod);
            if (disableNotification) args.Add("disable_notification", true);
            if (replyToMessageId != -1) args.Add("reply_to_message_id", replyToMessageId);
            if (replyMarkup != null) args.Add("reply_markup", replyMarkup);

            return await ApiMethodAsync<Message>("sendLocation", args);
        }

        /// <summary>
        /// Use this method to send information about a venue. On success, the sent Message is returned.
        /// </summary>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel (in the format @channelusername)</param>
        /// <param name="latitude">Latitude of the venue</param>
        /// <param name="longitude">Longitude of the venue</param>
        /// <param name="title">Name of the venue</param>
        /// <param name="address">Address of the venue</param>
        /// <param name="foursquareId">Foursquare identifier of the venue</param>
        /// <param name="foursquareType">Foursquare type of the venue, if known. 
        /// (For example, “arts_entertainment/default”, “arts_entertainment/aquarium” or “food/icecream”.)</param>
        /// <param name="disableNotification">If this is true, users will receive a silent notification</param>
        /// <param name="replyToMessageId">The message id of the message to reply to, if any</param>
        /// <param name="replyMarkup">The reply markup. Additional interface options.</param>
        /// <returns>The sent message on success</returns>
        public Message SendVenue(ChatId chatId, double latitude, double longitude, string title,
            string address, string foursquareId = null, string foursquareType = null, bool disableNotification = false, 
            int replyToMessageId = -1, ReplyMarkupBase replyMarkup = null)
            => SendVenueAsync(chatId, latitude, longitude, title, address, foursquareId, foursquareType, disableNotification, replyToMessageId,
                replyMarkup).Result;
        /// <summary>
        /// Use this method to send information about a venue. On success, the sent Message is returned.
        /// </summary>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel (in the format @channelusername)</param>
        /// <param name="latitude">Latitude of the venue</param>
        /// <param name="longitude">Longitude of the venue</param>
        /// <param name="title">Name of the venue</param>
        /// <param name="address">Address of the venue</param>
        /// <param name="foursquareId">Foursquare identifier of the venue</param>
        /// <param name="foursquareType">Foursquare type of the venue, if known. 
        /// (For example, “arts_entertainment/default”, “arts_entertainment/aquarium” or “food/icecream”.)</param>
        /// <param name="disableNotification">If this is true, users will receive a silent notification</param>
        /// <param name="replyToMessageId">The message id of the message to reply to, if any</param>
        /// <param name="replyMarkup">The reply markup. Additional interface options.</param>
        /// <returns>The sent message on success</returns>
        public async Task<Message> SendVenueAsync(ChatId chatId, double latitude, double longitude, string title,
            string address, string foursquareId = null, string foursquareType = null, bool disableNotification = false, 
            int replyToMessageId = -1, ReplyMarkupBase replyMarkup = null)
        {
            Dictionary<string, object> args = new Dictionary<string, object>()
            {
                { "chat_id", chatId }, { "latitude", latitude },
                { "longitude", longitude }, { "title", title }, { "address", address }
            };
            if (!string.IsNullOrWhiteSpace(foursquareId)) args.Add("foursquare_id", foursquareId);
            if (!string.IsNullOrWhiteSpace(foursquareType)) args.Add("foursquare_type", foursquareType);
            if (disableNotification) args.Add("disable_notification", true);
            if (replyToMessageId != -1) args.Add("reply_to_message_id", replyToMessageId);
            if (replyMarkup != null) args.Add("reply_markup", replyMarkup);

            return await ApiMethodAsync<Message>("sendVenue", args);
        }

        /// <summary>
        /// Use this method to send phone contacts. On success, the sent Message is returned.
        /// </summary>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel 
        /// (in the format @channelusername)</param>
        /// <param name="phoneNumber">Contact's phone number</param>
        /// <param name="firstName">Contact's first name</param>
        /// <param name="lastName">Contact's last name</param>
        /// <param name="vCard">Additional data about the contact in the form of a vCard, 0-2048 bytes</param>
        /// <param name="disableNotification">If this is true, users will receive a silent notification</param>
        /// <param name="replyToMessageId">The message id of the message to reply to, if any</param>
        /// <param name="replyMarkup">The reply markup. Additional interface options.</param>
        /// <returns>The sent message on success</returns>
        public Message SendContact(ChatId chatId, string phoneNumber, string firstName, string lastName = null, string vCard = null,
            bool disableNotification = false, int replyToMessageId = -1, ReplyMarkupBase replyMarkup = null)
            => SendContactAsync(chatId, phoneNumber, firstName, lastName, vCard, disableNotification, replyToMessageId, replyMarkup).Result;
        /// <summary>
        /// Use this method to send phone contacts. On success, the sent Message is returned.
        /// </summary>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel 
        /// (in the format @channelusername)</param>
        /// <param name="phoneNumber">Contact's phone number</param>
        /// <param name="firstName">Contact's first name</param>
        /// <param name="lastName">Contact's last name</param>
        /// <param name="vCard">Additional data about the contact in the form of a vCard, 0-2048 bytes</param>
        /// <param name="disableNotification">If this is true, users will receive a silent notification</param>
        /// <param name="replyToMessageId">The message id of the message to reply to, if any</param>
        /// <param name="replyMarkup">The reply markup. Additional interface options.</param>
        /// <returns>The sent message on success</returns>
        public async Task<Message> SendContactAsync(ChatId chatId, string phoneNumber, string firstName, string lastName = null, string vCard = null,
            bool disableNotification = false, int replyToMessageId = -1, ReplyMarkupBase replyMarkup = null)
        {
            Dictionary<string, object> args = new Dictionary<string, object>() { { "chat_id", chatId }, { "phoneNumber", phoneNumber },
                { "first_name", firstName }};
            if (!string.IsNullOrWhiteSpace(lastName)) args.Add("last_name", lastName);
            if (!string.IsNullOrWhiteSpace(vCard)) args.Add("vcard", vCard);
            if (disableNotification) args.Add("disable_notification", true);
            if (replyToMessageId != -1) args.Add("reply_to_message_id", replyToMessageId);
            if (replyMarkup != null) args.Add("reply_markup", replyMarkup);

            return await ApiMethodAsync<Message>("sendContact", args);
        }

        /// <summary>
        /// Use this method when you need to tell the user that something is happening on the bot's side. 
        /// The status is set for 5 seconds or less (when a message arrives from your bot, Telegram clients clear its typing status). 
        /// 
        /// <para>Example: The ImageBot needs some time to process a request and upload the image. 
        /// Instead of sending a text message along the lines of “Retrieving image, please wait…”, 
        /// the bot may use sendChatAction with action = upload_photo. 
        /// The user will see a “sending photo” status for the bot.</para>
        /// 
        /// <para>We only recommend using this method when a response from the bot will take a noticeable amount of time to arrive.</para>
        /// </summary>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel (in the format @channelusername)</param>
        /// <param name="action">Type of action to broadcast. Choose one, depending on what the user is about to receive:
        /// <see cref="ChatAction.Typing"/> for text messages, 
        /// <see cref="ChatAction.UploadPhoto"/> for photos, 
        /// <see cref="ChatAction.RecordVideo"/> or <see cref="ChatAction.UploadVideo"/> for videos, 
        /// <see cref="ChatAction.RecordAudio"/> or <see cref="ChatAction.UploadAudio"/> for audio files, 
        /// <see cref="ChatAction.UploadDocument"/> for general files, <see cref="ChatAction.FindLocation"/> for location data, 
        /// <see cref="ChatAction.RecordVideoNote"/> or <see cref="ChatAction.UploadVideoNote"/> for video notes.</param>
        /// <returns>True on success</returns>
        public bool SendChatAction(ChatId chatId, ChatAction action)
            => SendChatActionAsync(chatId, action).Result;
        /// <summary>
        /// Use this method when you need to tell the user that something is happening on the bot's side. 
        /// The status is set for 5 seconds or less (when a message arrives from your bot, Telegram clients clear its typing status). 
        /// 
        /// <para>Example: The ImageBot needs some time to process a request and upload the image. 
        /// Instead of sending a text message along the lines of “Retrieving image, please wait…”, 
        /// the bot may use sendChatAction with action = upload_photo. 
        /// The user will see a “sending photo” status for the bot.</para>
        /// 
        /// <para>We only recommend using this method when a response from the bot will take a noticeable amount of time to arrive.</para>
        /// </summary>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel (in the format @channelusername)</param>
        /// <param name="action">Type of action to broadcast. Choose one, depending on what the user is about to receive:
        /// <see cref="ChatAction.Typing"/> for text messages, 
        /// <see cref="ChatAction.UploadPhoto"/> for photos, 
        /// <see cref="ChatAction.RecordVideo"/> or <see cref="ChatAction.UploadVideo"/> for videos, 
        /// <see cref="ChatAction.RecordAudio"/> or <see cref="ChatAction.UploadAudio"/> for audio files, 
        /// <see cref="ChatAction.UploadDocument"/> for general files, <see cref="ChatAction.FindLocation"/> for location data, 
        /// <see cref="ChatAction.RecordVideoNote"/> or <see cref="ChatAction.UploadVideoNote"/> for video notes.</param>
        /// <returns>True on success</returns>
        public async Task<bool> SendChatActionAsync(ChatId chatId, ChatAction action)
        {
            Dictionary<string, object> args = new Dictionary<string, object>() { { "chat_id", chatId }, { "action", Enum.GetString(action) } };

            return await ApiMethodAsync<bool>("sendChatAction", args);
        }
        #endregion
        #region Live locations
        /// <summary>
        /// Use this method to edit live location messages sent by the bot. 
        /// A location can be edited until its live_period expires or editing is explicitly disabled by a call to stopMessageLiveLocation. 
        /// On success, the edited Message is returned.
        /// </summary>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel
        /// (in the format @channelusername)</param>
        /// <param name="messageId">Identifier of the sent message</param>
        /// <param name="latitude">New latitude of the location</param>
        /// <param name="longitude">New longitude of the location</param>
        /// <param name="replyMarkup">The reply markup. Additional interface options.</param>
        /// <returns>The edited message</returns>
        public Message EditMessageLiveLocation(ChatId chatId, int messageId, double latitude, double longitude,
            InlineKeyboardMarkup replyMarkup = null)
            => EditMessageLiveLocationAsync(chatId, messageId, latitude, longitude, replyMarkup).Result;
        /// <summary>
        /// Use this method to edit live location messages sent by the bot. 
        /// A location can be edited until its live_period expires or editing is explicitly disabled by a call to stopMessageLiveLocation. 
        /// On success, the edited Message is returned.
        /// </summary>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel
        /// (in the format @channelusername)</param>
        /// <param name="messageId">Identifier of the sent message</param>
        /// <param name="latitude">New latitude of the location</param>
        /// <param name="longitude">New longitude of the location</param>
        /// <param name="replyMarkup">The reply markup. Additional interface options.</param>
        /// <returns>The edited message</returns>
        public async Task<Message> EditMessageLiveLocationAsync(ChatId chatId, int messageId, double latitude, double longitude,
            InlineKeyboardMarkup replyMarkup = null)
        {
            Dictionary<string, object> args = new Dictionary<string, object>()
            {
                { "chat_id", chatId }, { "message_id", messageId },
                { "latitude", latitude }, { "longitude", longitude }
            };
            if (replyMarkup != null) args.Add("reply_markup", replyMarkup);

            return await ApiMethodAsync<Message>("editMessageLiveLocation", args);
        }

        /// <summary>
        /// Use this method to edit live location messages sent via the bot (for inline bots). 
        /// A location can be edited until its live_period expires or editing is explicitly disabled by a call to stopMessageLiveLocation. 
        /// On success, True is returned.
        /// </summary>
        /// <param name="inlineMessageId">Identifier of the inline message</param>
        /// <param name="latitude">New latitude of the location</param>
        /// <param name="longitude">New longitude of the location</param>
        /// <param name="replyMarkup">The reply markup. Additional interface options.</param>
        /// <returns>True on success</returns>
        public bool EditMessageLiveLocation(string inlineMessageId, double latitude, double longitude,
            InlineKeyboardMarkup replyMarkup = null)
            => EditMessageLiveLocationAsync(inlineMessageId, latitude, longitude, replyMarkup).Result;
        /// <summary>
        /// Use this method to edit live location messages sent via the bot (for inline bots). 
        /// A location can be edited until its live_period expires or editing is explicitly disabled by a call to stopMessageLiveLocation. 
        /// On success, True is returned.
        /// </summary>
        /// <param name="inlineMessageId">Identifier of the inline message</param>
        /// <param name="latitude">New latitude of the location</param>
        /// <param name="longitude">New longitude of the location</param>
        /// <param name="replyMarkup">The reply markup. Additional interface options.</param>
        /// <returns>True on success</returns>
        public async Task<bool> EditMessageLiveLocationAsync(string inlineMessageId, double latitude, double longitude,
            InlineKeyboardMarkup replyMarkup = null)
        {
            Dictionary<string, object> args = new Dictionary<string, object>()
            {
                { "inline_message_id", inlineMessageId },
                { "latitude", latitude }, { "longitude", longitude }
            };
            if (replyMarkup != null) args.Add("reply_markup", replyMarkup);

            return await ApiMethodAsync<bool>("editMessageLiveLocation", args);
        }

        /// <summary>
        /// Use this method to stop updating a live location message sent by the bot before live_period expires. 
        /// On success, the sent Message is returned.
        /// </summary>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel
        /// (in the format @channelusername)</param>
        /// <param name="messageId">Identifier of the sent message</param>
        /// <param name="replyMarkup">The reply markup. Additional interface options.</param>
        /// <returns>The edited message</returns>
        public Message StopMessageLiveLocation(ChatId chatId, int messageId, InlineKeyboardMarkup replyMarkup = null)
            => StopMessageLiveLocationAsync(chatId, messageId, replyMarkup).Result;
        /// <summary>
        /// Use this method to stop updating a live location message sent by the bot before live_period expires. 
        /// On success, the sent Message is returned.
        /// </summary>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel
        /// (in the format @channelusername)</param>
        /// <param name="messageId">Identifier of the sent message</param>
        /// <param name="replyMarkup">The reply markup. Additional interface options.</param>
        /// <returns>The edited message</returns>
        public async Task<Message> StopMessageLiveLocationAsync(ChatId chatId, int messageId, InlineKeyboardMarkup replyMarkup = null)
        {
            Dictionary<string, object> args = new Dictionary<string, object>()
            {
                { "chat_id", chatId }, { "message_id", messageId }
            };
            if (replyMarkup != null) args.Add("reply_markup", replyMarkup);

            return await ApiMethodAsync<Message>("stopMessageLiveLocation", args);
        }

        /// <summary>
        /// Use this method to stop updating a live location message sent via the bot (for inline bots) before live_period expires. 
        /// On success, True is returned.
        /// </summary>
        /// <param name="inlineMessageId">Identifier of the inline message</param>
        /// <param name="replyMarkup">The reply markup. Additional interface options.</param>
        /// <returns>True on success</returns>
        public bool StopMessageLiveLocation(string inlineMessageId, InlineKeyboardMarkup replyMarkup = null)
            => StopMessageLiveLocationAsync(inlineMessageId, replyMarkup).Result;
        /// <summary>
        /// Use this method to stop updating a live location message sent via the bot (for inline bots) before live_period expires. 
        /// On success, True is returned.
        /// </summary>
        /// <param name="inlineMessageId">Identifier of the inline message</param>
        /// <param name="replyMarkup">The reply markup. Additional interface options.</param>
        /// <returns>True on success</returns>
        public async Task<bool> StopMessageLiveLocationAsync(string inlineMessageId, InlineKeyboardMarkup replyMarkup = null)
        {
            Dictionary<string, object> args = new Dictionary<string, object>()
            {
                { "inline_message_id", inlineMessageId }
            };
            if (replyMarkup != null) args.Add("reply_markup", replyMarkup);

            return await ApiMethodAsync<bool>("stopMessageLiveLocation", args);
        }
        #endregion
        #region File and photo getting methods
        /// <summary>
        /// Use this method to get a list of profile pictures for a user. Returns a UserProfilePhotos object.
        /// </summary>
        /// <param name="userId">Unique identifier of the target user</param>
        /// <param name="offset">Sequential number of the first photo to be returned. By default, all photos are returned.</param>
        /// <param name="limit">Limits the number of photos to be retrieved. Values between 1—100 are accepted. Defaults to 100.</param>
        /// <returns>The users profile photos</returns>
        public UserProfilePictures GetUserProfilePhotos(int userId, int offset = 0, int limit = 100)
            => GetUserProfilePhotosAsync(userId, offset, limit).Result;
        /// <summary>
        /// Use this method to get a list of profile pictures for a user. Returns a UserProfilePhotos object.
        /// </summary>
        /// <param name="userId">Unique identifier of the target user</param>
        /// <param name="offset">Sequential number of the first photo to be returned. By default, all photos are returned.</param>
        /// <param name="limit">Limits the number of photos to be retrieved. Values between 1—100 are accepted. Defaults to 100.</param>
        /// <returns>The users profile photos</returns>
        public async Task<UserProfilePictures> GetUserProfilePhotosAsync(int userId, int offset = 0, int limit = 100)
        {
            Dictionary<string, object> args = new Dictionary<string, object>() { { "user_id", userId } };
            if (offset != 0) args.Add("offset", offset);
            if (limit != 100) args.Add("limit", limit);

            return await ApiMethodAsync<UserProfilePictures>("getUserProfilePhotos", args);
        }

        /// <summary>
        /// Use this method to get basic info about a file and prepare it for downloading. 
        /// For the moment, bots can download files of up to 20MB in size. On success, a File object is returned. 
        /// The file can then be downloaded via the link https://api.telegram.org/file/bot&lt;token&gt;/&lt;file_path&gt;, 
        /// where &lt;file_path&gt; is taken from the response. It is guaranteed that the link will be valid for at least 1 hour. 
        /// When the link expires, a new one can be requested by calling getFile again. 
        /// Note: This function may not preserve the original file name and MIME type. 
        /// You should save the file's MIME type and name (if available) when the File object is received.
        /// </summary>
        /// <param name="fileId">File identifier to get info about</param>
        /// <returns>The file object</returns>
        public File GetFile(string fileId) => GetFileAsync(fileId).Result;
        /// <summary>
        /// Use this method to get basic info about a file and prepare it for downloading. 
        /// For the moment, bots can download files of up to 20MB in size. On success, a File object is returned. 
        /// The file can then be downloaded via the link https://api.telegram.org/file/bot&lt;token&gt;/&lt;file_path&gt;, 
        /// where &lt;file_path&gt; is taken from the response. It is guaranteed that the link will be valid for at least 1 hour. 
        /// When the link expires, a new one can be requested by calling getFile again. 
        /// Note: This function may not preserve the original file name and MIME type. 
        /// You should save the file's MIME type and name (if available) when the File object is received.
        /// </summary>
        /// <param name="fileId">File identifier to get info about</param>
        /// <returns>The file object</returns>
        public async Task<File> GetFileAsync(string fileId)
        {
            Dictionary<string, object> args = new Dictionary<string, object>() { { "file_id", fileId } };

            return await ApiMethodAsync<File>("getFile", args);
        }

        /// <summary>
        /// Downloads a file to a given path
        /// </summary>
        /// <param name="fileId">The telegram identifier of the file</param>
        /// <param name="destinationPath">The path to save the file to</param>
        public void DownloadFile(string fileId, string destinationPath) => DownloadFileAsync(fileId, destinationPath).Wait();
        /// <summary>
        /// Downloads a file to a given path
        /// </summary>
        /// <param name="fileId">The telegram identifier of the file</param>
        /// <param name="destinationPath">The path to save the file to</param>
        public async Task DownloadFileAsync(string fileId, string destinationPath)
        {
            File file = await GetFileAsync(fileId);
            await DownloadFileAsync(file, destinationPath);
        }

        /// <summary>
        /// Downloads a file to a given path
        /// </summary>
        /// <param name="file">The file object obtained by <see cref="GetFileAsync(string)"/></param>
        /// <param name="destinationPath">The path to send the file to</param>
        public void DownloadFile(File file, string destinationPath)
            => DownloadFileAsync(file, destinationPath).Wait();
        /// <summary>
        /// Downloads a file to a given path
        /// </summary>
        /// <param name="file">The file object obtained by <see cref="GetFileAsync(string)"/></param>
        /// <param name="destinationPath">The path to send the file to</param>
        public async Task DownloadFileAsync(File file, string destinationPath)
        {
            WebClient wc = new WebClient();
            string url = fileDownloadLink.Replace("{token}", _token).Replace("{path}", file.FilePath);
            await wc.DownloadFileTaskAsync(url, destinationPath);
        }

        /// <summary>
        /// Downloads a file and returns its stream
        /// </summary>
        /// <param name="fileId">The telegram file id</param>
        /// <returns>A stream containing the file</returns>
        public Stream DownloadFile(string fileId) => DownloadFileAsync(fileId).Result;
        /// <summary>
        /// Downloads a file and returns its stream
        /// </summary>
        /// <param name="fileId">The telegram file id</param>
        /// <returns>A stream containing the file</returns>
        public async Task<Stream> DownloadFileAsync(string fileId)
        {
            File file = await GetFileAsync(fileId);
            return await DownloadFileAsync(file);
        }

        /// <summary>
        /// Downloads a file and returns its stream
        /// </summary>
        /// <param name="file">The File object obtained by <see cref="GetFileAsync(string)"/></param>
        /// <returns>A stream containing the file</returns>
        public Stream DownloadFile(File file) => DownloadFileAsync(file).Result;
        /// <summary>
        /// Downloads a file and returns its stream
        /// </summary>
        /// <param name="file">The File object obtained by <see cref="GetFileAsync(string)"/></param>
        /// <returns>A stream containing the file</returns>
        public async Task<Stream> DownloadFileAsync(File file)
        {
            string url = fileDownloadLink.Replace("{token}", _token).Replace("{path}", file.FilePath);
            WebClient wc = new WebClient();
            return await wc.OpenReadTaskAsync(url);
        }
        #endregion
        #region Chat Administration Methods
        /// <summary>
        /// Kicks a member from a chat so that they can instantly rejoin.
        /// </summary>
        /// <param name="chatId">The chats id or the channels username to kick the user from</param>
        /// <param name="userId">The user id of the user to kick</param>
        /// <returns>True on success</returns>
        public bool KickChatMember(ChatId chatId, int userId) => KickChatMemberAsync(chatId, userId).Result;
        /// <summary>
        /// Kicks a member from a chat so that they can instantly rejoin.
        /// </summary>
        /// <param name="chatId">The chats id or the channels username to kick the user from</param>
        /// <param name="userId">The user id of the user to kick</param>
        /// <returns>True on success</returns>
        public async Task<bool> KickChatMemberAsync(ChatId chatId, int userId)
        {
            Chat chat = await GetChatAsync(chatId);
            if (await BanChatMemberAsync(chatId, userId))
            {
                if (chat.Type == ChatType.Channel || chat.Type == ChatType.Supergroup) return await UnbanChatMemberAsync(chatId, userId);
                else return true;
            }
            else return false;
        }

        /// <summary>
        /// Kicks a member from a chat. If the chat is a channel or a supergroup, the kicked user can't return without being
        /// unbanned by an admin first. Admins can re-add banned users.
        /// </summary>
        /// <param name="chatId">The chat id or the channel username of the chat to kick the user from</param>
        /// <param name="userId">The Id of the user to be kicked</param>
        /// <param name="untilDate">Date when the user will be automatically unbanned. 
        /// If user is banned for more than 366 days or less than 30 seconds from the current time they are considered to be banned forever</param>
        /// <returns>True on success</returns>
        public bool BanChatMember(ChatId chatId, int userId, DateTime untilDate = default(DateTime))
            => BanChatMemberAsync(chatId, userId, untilDate).Result;
        /// <summary>
        /// Kicks a member from a chat. If the chat is a channel or a supergroup, the kicked user can't return without being
        /// unbanned by an admin first. Admins can re-add banned users.
        /// </summary>
        /// <param name="chatId">The chat id or the channel username of the chat to kick the user from</param>
        /// <param name="userId">The Id of the user to be kicked</param>
        /// <param name="untilDate">Date when the user will be automatically unbanned. 
        /// If user is banned for more than 366 days or less than 30 seconds from the current time they are considered to be banned forever</param>
        /// <returns>True on success</returns>
        public async Task<bool> BanChatMemberAsync(ChatId chatId, int userId, DateTime untilDate = default(DateTime))
        {
            Dictionary<string, object> args = new Dictionary<string, object>() { { "chat_id", chatId }, { "user_id", userId } };
            if (untilDate != default(DateTime)) args.Add("until_date", ((DateTimeOffset)untilDate).ToUnixTimeSeconds());

            return await ApiMethodAsync<bool>("kickChatMember", args);
        }

        /// <summary>
        /// Unbans a user from a chat
        /// </summary>
        /// <param name="chatId">Unique identifier for the target group or username of the target supergroup or channel 
        /// (in the format @username)</param>
        /// <param name="userId">The id of the user to unban</param>
        /// <returns>True on success</returns>
        public bool UnbanChatMember(ChatId chatId, int userId) => UnbanChatMemberAsync(chatId, userId).Result;
        /// <summary>
        /// Unbans a user from a chat
        /// </summary>
        /// <param name="chatId">Unique identifier for the target group or username of the target supergroup or channel 
        /// (in the format @username)</param>
        /// <param name="userId">The id of the user to unban</param>
        /// <returns>True on success</returns>
        public async Task<bool> UnbanChatMemberAsync(ChatId chatId, int userId)
        {
            Dictionary<string, object> args = new Dictionary<string, object>() { { "chat_id", chatId }, { "user_id", userId } };

            return await ApiMethodAsync<bool>("unbanChatMember", args);
        }

        /// <summary>
        /// Use this method to restrict a user in a supergroup. 
        /// The bot must be an administrator in the supergroup for this to work and must have the appropriate admin rights. 
        /// Pass True for all boolean parameters to lift restrictions from a user. Returns True on success.
        /// </summary>
        /// <param name="chatId">Unique identifier for the target chat or username of the target supergroup 
        /// (in the format @supergroupusername)</param>
        /// <param name="userId">Unique identifier of the target user</param>
        /// <param name="untilDate">Date when restrictions will be lifted for the user. 
        /// If user is restricted for more than 366 days or less than 30 seconds from the current time, 
        /// they are considered to be restricted forever</param>
        /// <param name="canSendMessages">Pass True, if the user can send text messages, contacts, locations and venues</param>
        /// <param name="canSendMediaMessages">Pass True, if the user can send audios, documents, photos, videos, 
        /// video notes and voice notes, implies canSendMessages</param>
        /// <param name="canSendOtherMessages">Pass True, if the user can send animations, games, stickers and use inline bots, 
        /// implies canSendMediaMessages</param>
        /// <param name="canSendWebPagePreviews">Pass True, if the user may add web page previews to their messages, 
        /// implies canSendMediaMessages</param>
        /// <returns>True on success</returns>
        public bool RestrictChatMember(ChatId chatId, int userId, DateTime untilDate = default(DateTime),
            bool canSendMessages = false, bool canSendMediaMessages = false, bool canSendOtherMessages = false,
            bool canSendWebPagePreviews = false)
            => RestrictChatMemberAsync(chatId, userId, untilDate, canSendMessages, canSendMediaMessages, canSendOtherMessages,
                canSendWebPagePreviews).Result;
        /// <summary>
        /// Use this method to restrict a user in a supergroup. 
        /// The bot must be an administrator in the supergroup for this to work and must have the appropriate admin rights. 
        /// Pass True for all boolean parameters to lift restrictions from a user. Returns True on success.
        /// </summary>
        /// <param name="chatId">Unique identifier for the target chat or username of the target supergroup 
        /// (in the format @supergroupusername)</param>
        /// <param name="userId">Unique identifier of the target user</param>
        /// <param name="untilDate">Date when restrictions will be lifted for the user. 
        /// If user is restricted for more than 366 days or less than 30 seconds from the current time, 
        /// they are considered to be restricted forever</param>
        /// <param name="canSendMessages">Pass True, if the user can send text messages, contacts, locations and venues</param>
        /// <param name="canSendMediaMessages">Pass True, if the user can send audios, documents, photos, videos, 
        /// video notes and voice notes, implies canSendMessages</param>
        /// <param name="canSendOtherMessages">Pass True, if the user can send animations, games, stickers and use inline bots, 
        /// implies canSendMediaMessages</param>
        /// <param name="canSendWebPagePreviews">Pass True, if the user may add web page previews to their messages, 
        /// implies canSendMediaMessages</param>
        /// <returns>True on success</returns>
        public async Task<bool> RestrictChatMemberAsync(ChatId chatId, int userId, DateTime untilDate = default(DateTime),
            bool canSendMessages = false, bool canSendMediaMessages = false, bool canSendOtherMessages = false,
            bool canSendWebPagePreviews = false)
        {
            Dictionary<string, object> args = new Dictionary<string, object>() { { "chat_id", chatId }, { "user_id", userId } };
            if (untilDate != default(DateTime)) args.Add("until_date", ((DateTimeOffset)untilDate).ToUnixTimeSeconds());
            if (canSendMessages) args.Add("can_send_messages", true);
            if (canSendMediaMessages) args.Add("can_send_media_messages", true);
            if (canSendOtherMessages) args.Add("can_send_other_messages", true);
            if (canSendWebPagePreviews) args.Add("can_send_web_page_previews", true);

            return await ApiMethodAsync<bool>("restrictChatMember", args);
        }

        /// <summary>
        /// Use this method to promote or demote a user in a supergroup or a channel. 
        /// The bot must be an administrator in the chat for this to work and must have the appropriate admin rights. 
        /// Pass noting for all boolean parameters to demote a user. Returns True on success.
        /// </summary>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel 
        /// (in the format @channelusername)</param>
        /// <param name="userId">Unique identifier of target user</param>
        /// <param name="canChangeInfo">Pass True, if the administrator can change chat title, photo and other settings</param>
        /// <param name="canPostMessages">Pass True, if the administrator can create channel posts, channels only</param>
        /// <param name="canEditMessages">Pass True, if the administrator can edit messages of other users and can pin messages, 
        /// channels only</param>
        /// <param name="canDeleteMessages">Pass True, if the administrator can delete messages of other users</param>
        /// <param name="canInviteUsers">Pass True, if the administrator can invite new users to the chat</param>
        /// <param name="canRestrictMembers">Pass True, if the administrator can restrict, ban or unban chat members</param>
        /// <param name="canPinMessages">Pass True, if the administrator can pin messages, supergroups only</param>
        /// <param name="canPromoteMembers">Pass True, if the administrator can add new administrators with a subset of his own privileges 
        /// or demote administrators that he has promoted, directly or indirectly 
        /// (promoted by administrators that were appointed by him)</param>
        /// <returns>True on success</returns>
        public bool PromoteChatMember(ChatId chatId, int userId, bool canChangeInfo = false,
            bool canPostMessages = false, bool canEditMessages = false, bool canDeleteMessages = false, bool canInviteUsers = false,
            bool canRestrictMembers = false, bool canPinMessages = false, bool canPromoteMembers = false)
            => PromoteChatMemberAsync(chatId, userId, canChangeInfo, canPostMessages, canEditMessages, canDeleteMessages,
                canInviteUsers, canRestrictMembers, canPinMessages, canPromoteMembers).Result;
        /// <summary>
        /// Use this method to promote or demote a user in a supergroup or a channel. 
        /// The bot must be an administrator in the chat for this to work and must have the appropriate admin rights. 
        /// Pass noting for all boolean parameters to demote a user. Returns True on success.
        /// </summary>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel 
        /// (in the format @channelusername)</param>
        /// <param name="userId">Unique identifier of target user</param>
        /// <param name="canChangeInfo">Pass True, if the administrator can change chat title, photo and other settings</param>
        /// <param name="canPostMessages">Pass True, if the administrator can create channel posts, channels only</param>
        /// <param name="canEditMessages">Pass True, if the administrator can edit messages of other users and can pin messages, 
        /// channels only</param>
        /// <param name="canDeleteMessages">Pass True, if the administrator can delete messages of other users</param>
        /// <param name="canInviteUsers">Pass True, if the administrator can invite new users to the chat</param>
        /// <param name="canRestrictMembers">Pass True, if the administrator can restrict, ban or unban chat members</param>
        /// <param name="canPinMessages">Pass True, if the administrator can pin messages, supergroups only</param>
        /// <param name="canPromoteMembers">Pass True, if the administrator can add new administrators with a subset of his own privileges 
        /// or demote administrators that he has promoted, directly or indirectly 
        /// (promoted by administrators that were appointed by him)</param>
        /// <returns>True on success</returns>
        public async Task<bool> PromoteChatMemberAsync(ChatId chatId, int userId, bool canChangeInfo = false,
            bool canPostMessages = false, bool canEditMessages = false, bool canDeleteMessages = false, bool canInviteUsers = false,
            bool canRestrictMembers = false, bool canPinMessages = false, bool canPromoteMembers = false)
        {
            Dictionary<string, object> args = new Dictionary<string, object>() { { "chat_id", chatId }, { "user_id", userId } };
            if (canChangeInfo) args.Add("can_change_info", true);
            if (canPostMessages) args.Add("can_post_messages", true);
            if (canEditMessages) args.Add("can_edit_messages", true);
            if (canDeleteMessages) args.Add("can_delete_messages", true);
            if (canInviteUsers) args.Add("can_invite_users", true);
            if (canRestrictMembers) args.Add("can_restrict_members", true);
            if (canPinMessages) args.Add("can_pin_messages", true);
            if (canPromoteMembers) args.Add("can_promote_members", true);

            return await ApiMethodAsync<bool>("promoteChatMember", args);
        }

        /// <summary>
        /// Use this method to set a new profile photo for the chat. Photos can't be changed for private chats. 
        /// The bot must be an administrator in the chat for this to work and must have the appropriate admin rights. Returns True on success.
        /// </summary>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel 
        /// (in the format @channelusername)</param>
        /// <param name="photo">New chat photo, uploaded using multipart/form-data</param>
        /// <returns>True on success</returns>
        public bool SetChatPhoto(ChatId chatId, SendFileMultipart photo)
            => SetChatPhotoAsync(chatId, photo).Result;
        /// <summary>
        /// Use this method to set a new profile photo for the chat. Photos can't be changed for private chats. 
        /// The bot must be an administrator in the chat for this to work and must have the appropriate admin rights. Returns True on success.
        /// </summary>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel 
        /// (in the format @channelusername)</param>
        /// <param name="photo">New chat photo, uploaded using multipart/form-data</param>
        /// <returns>True on success</returns>
        public async Task<bool> SetChatPhotoAsync(ChatId chatId, SendFileMultipart photo)
        {
            Dictionary<string, object> args = new Dictionary<string, object>() { { "chat_id", chatId }, { "photo", photo } };
            return await ApiMethodAsync<bool>("setChatPhoto", args);
        }

        /// <summary>
        /// Use this method to delete a chat photo. Photos can't be changed for private chats. 
        /// The bot must be an administrator in the chat for this to work and must have the appropriate admin rights. 
        /// Returns True on success.
        /// </summary>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel 
        /// (in the format @channelusername)</param>
        /// <returns>True on success</returns>
        public bool DeleteChatPhoto(ChatId chatId) => DeleteChatPhotoAsync(chatId).Result;
        /// <summary>
        /// Use this method to delete a chat photo. Photos can't be changed for private chats. 
        /// The bot must be an administrator in the chat for this to work and must have the appropriate admin rights. 
        /// Returns True on success.
        /// </summary>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel 
        /// (in the format @channelusername)</param>
        /// <returns>True on success</returns>
        public async Task<bool> DeleteChatPhotoAsync(ChatId chatId)
        {
            Dictionary<string, object> args = new Dictionary<string, object>() { { "chat_id", chatId } };
            return await ApiMethodAsync<bool>("deleteChatPhoto", args);
        }

        /// <summary>
        /// Use this method to change the title of a chat. Titles can't be changed for private chats. 
        /// The bot must be an administrator in the chat for this to work and must have the appropriate admin rights. 
        /// Returns True on success.
        /// </summary>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel 
        /// (in the format @channelusername)</param>
        /// <param name="title">New chat title, 1-255 characters</param>
        /// <returns>True on success</returns>
        public bool SetChatTitle(ChatId chatId, string title) => SetChatTitleAsync(chatId, title).Result;
        /// <summary>
        /// Use this method to change the title of a chat. Titles can't be changed for private chats. 
        /// The bot must be an administrator in the chat for this to work and must have the appropriate admin rights. 
        /// Returns True on success.
        /// </summary>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel 
        /// (in the format @channelusername)</param>
        /// <param name="title">New chat title, 1-255 characters</param>
        /// <returns>True on success</returns>
        public async Task<bool> SetChatTitleAsync(ChatId chatId, string title)
        {
            Dictionary<string, object> args = new Dictionary<string, object>() { { "chat_id", chatId }, { "title", title } };
            return await ApiMethodAsync<bool>("setChatTitle", args);
        }

        /// <summary>
        /// Use this method to change the description of a supergroup or a channel. 
        /// The bot must be an administrator in the chat for this to work and must have the appropriate admin rights. 
        /// Returns True on success.
        /// </summary>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel 
        /// (in the format @channelusername)</param>
        /// <param name="description">New chat description, 0-255 characters</param>
        /// <returns>True on success</returns>
        public bool SetChatDescription(ChatId chatId, string description) => SetChatDescriptionAsync(chatId, description).Result;
        /// <summary>
        /// Use this method to change the description of a supergroup or a channel. 
        /// The bot must be an administrator in the chat for this to work and must have the appropriate admin rights. 
        /// Returns True on success.
        /// </summary>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel 
        /// (in the format @channelusername)</param>
        /// <param name="description">New chat description, 0-255 characters</param>
        /// <returns>True on success</returns>
        public async Task<bool> SetChatDescriptionAsync(ChatId chatId, string description)
        {
            Dictionary<string, object> args = new Dictionary<string, object>() { { "chat_id", chatId }, { "description", description } };
            return await ApiMethodAsync<bool>("setChatDescription", args);
        }

        /// <summary>
        /// Use this method to pin a message in a supergroup or a channel. 
        /// The bot must be an administrator in the chat for this to work and must have the ‘can_pin_messages’ 
        /// admin right in the supergroup or ‘can_edit_messages’ admin right in the channel. Returns True on success.
        /// </summary>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel 
        /// (in the format @channelusername)</param>
        /// <param name="messageId">Identifier of a message to pin</param>
        /// <param name="disableNotification">Pass True, if it is not necessary to send a notification to all chat members 
        /// about the new pinned message. Notifications are always disabled in channels.</param>
        /// <returns>True on success</returns>
        public bool PinChatMessage(ChatId chatId, int messageId, bool disableNotification = false)
            => PinChatMessageAsync(chatId, messageId, disableNotification).Result;
        /// <summary>
        /// Use this method to pin a message in a supergroup or a channel. 
        /// The bot must be an administrator in the chat for this to work and must have the ‘can_pin_messages’ 
        /// admin right in the supergroup or ‘can_edit_messages’ admin right in the channel. Returns True on success.
        /// </summary>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel 
        /// (in the format @channelusername)</param>
        /// <param name="messageId">Identifier of a message to pin</param>
        /// <param name="disableNotification">Pass True, if it is not necessary to send a notification to all chat members 
        /// about the new pinned message. Notifications are always disabled in channels.</param>
        /// <returns>True on success</returns>
        public async Task<bool> PinChatMessageAsync(ChatId chatId, int messageId, bool disableNotification = false)
        {
            Dictionary<string, object> args = new Dictionary<string, object>() { { "chat_id", chatId }, { "message_id", messageId } };
            if (disableNotification) args.Add("disable_notification", true);

            return await ApiMethodAsync<bool>("pinChatMessage", args);
        }

        /// <summary>
        /// Use this method to unpin a message in a supergroup or a channel. 
        /// The bot must be an administrator in the chat for this to work and must have the ‘can_pin_messages’ 
        /// admin right in the supergroup or ‘can_edit_messages’ admin right in the channel. Returns True on success.
        /// </summary>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel 
        /// (in the format @channelusername)</param>
        /// <returns>True on success</returns>
        public bool UnpinChatMessage(ChatId chatId) => UnpinChatMessageAsync(chatId).Result;
        /// <summary>
        /// Use this method to unpin a message in a supergroup or a channel. 
        /// The bot must be an administrator in the chat for this to work and must have the ‘can_pin_messages’ 
        /// admin right in the supergroup or ‘can_edit_messages’ admin right in the channel. Returns True on success.
        /// </summary>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel 
        /// (in the format @channelusername)</param>
        /// <returns>True on success</returns>
        public async Task<bool> UnpinChatMessageAsync(ChatId chatId)
        {
            Dictionary<string, object> args = new Dictionary<string, object>() { { "chat_id", chatId } };
            return await ApiMethodAsync<bool>("unpinChatMessage", args);
        }
        #endregion
        #region Chat information and interaction
        /// <summary>
        /// Use this method to generate a new invite link for a chat; any previously generated link is revoked. 
        /// The bot must be an administrator in the chat for this to work and must have the appropriate admin rights. 
        /// Returns the new invite link as String on success.
        /// </summary>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel 
        /// (in the format @channelusername)</param>
        /// <returns>The invite link</returns>
        public string ExportChatInviteLink(ChatId chatId) => ExportChatInviteLinkAsync(chatId).Result;
        /// <summary>
        /// Use this method to generate a new invite link for a chat; any previously generated link is revoked. 
        /// The bot must be an administrator in the chat for this to work and must have the appropriate admin rights. 
        /// Returns the new invite link as String on success.
        /// </summary>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel 
        /// (in the format @channelusername)</param>
        /// <returns>The invite link</returns>
        public async Task<string> ExportChatInviteLinkAsync(ChatId chatId)
        {
            Dictionary<string, object> args = new Dictionary<string, object>() { { "chat_id", chatId } };
            return await ApiMethodAsync<string>("exportChatInviteLink", args);
        }

        /// <summary>
        /// Use this method for your bot to leave a group, supergroup or channel. Returns True on success.
        /// </summary>
        /// <param name="chatId">Unique identifier for the target chat or username of the target supergroup or channel 
        /// (in the format @channelusername)</param>
        /// <returns>True on success</returns>
        public bool LeaveChat(ChatId chatId) => LeaveChatAsync(chatId).Result;
        /// <summary>
        /// Use this method for your bot to leave a group, supergroup or channel. Returns True on success.
        /// </summary>
        /// <param name="chatId">Unique identifier for the target chat or username of the target supergroup or channel 
        /// (in the format @channelusername)</param>
        /// <returns>True on success</returns>
        public async Task<bool> LeaveChatAsync(ChatId chatId)
        {
            Dictionary<string, object> args = new Dictionary<string, object>() { { "chat_id", chatId } };
            return await ApiMethodAsync<bool>("leaveChat", args);
        }

        /// <summary>
        /// Use this method to get up to date information about the chat 
        /// (current name of the user for one-on-one conversations, current username of a user, group or channel, etc.).
        /// </summary>
        /// <param name="chatId">Unique identifier for the target chat or username of the target supergroup or channel 
        /// (in the format @channelusername)</param>
        /// <returns>The specified chat as a <see cref="Chat"/> object</returns>
        public Chat GetChat(ChatId chatId) => GetChatAsync(chatId).Result;
        /// <summary>
        /// Use this method to get up to date information about the chat 
        /// (current name of the user for one-on-one conversations, current username of a user, group or channel, etc.).
        /// </summary>
        /// <param name="chatId">Unique identifier for the target chat or username of the target supergroup or channel 
        /// (in the format @channelusername)</param>
        /// <returns>The specified chat as a <see cref="Chat"/> object</returns>
        public async Task<Chat> GetChatAsync(ChatId chatId)
        {
            Dictionary<string, object> args = new Dictionary<string, object>() { { "chat_id", chatId } };
            return await ApiMethodAsync<Chat>("getChat", args);
        }

        /// <summary>
        /// Use this method to get a list of administrators in a chat. On success, returns an Array of ChatMember 
        /// objects that contains information about all chat administrators except other bots. 
        /// If the chat is a group or a supergroup and no administrators were appointed, only the creator will be returned.
        /// </summary>
        /// <param name="chatId">Unique identifier for the target chat or username of the target supergroup or channel 
        /// (in the format @channelusername)</param>
        /// <returns>The chat administrators</returns>
        public ChatMember[] GetChatAdministrators(ChatId chatId) => GetChatAdministratorsAsync(chatId).Result;
        /// <summary>
        /// Use this method to get a list of administrators in a chat. On success, returns an Array of ChatMember 
        /// objects that contains information about all chat administrators except other bots. 
        /// If the chat is a group or a supergroup and no administrators were appointed, only the creator will be returned.
        /// </summary>
        /// <param name="chatId">Unique identifier for the target chat or username of the target supergroup or channel 
        /// (in the format @channelusername)</param>
        /// <returns>The chat administrators</returns>
        public async Task<ChatMember[]> GetChatAdministratorsAsync(ChatId chatId)
        {
            Dictionary<string, object> args = new Dictionary<string, object>() { { "chat_id", chatId } };
            return await ApiMethodAsync<ChatMember[]>("getChatAdministrators", args);
        }

        /// <summary>
        /// Whether an user is an admin of the specified group.
        /// </summary>
        /// <param name="chatId">Unique identifier for the target chat or username of the target supergroup or channel 
        /// (in the format @channelusername)</param>
        /// <param name="userId">Telegram identifier of target user</param>
        /// <returns>True if the user is an admin</returns>
        public bool IsChatAdministrator(ChatId chatId, int userId) => IsChatAdministratorAsync(chatId, userId).Result;
        /// <summary>
        /// Whether an user is an admin of the specified group.
        /// </summary>
        /// <param name="chatId">Unique identifier for the target chat or username of the target supergroup or channel 
        /// (in the format @channelusername)</param>
        /// <param name="userId">Telegram identifier of target user</param>
        /// <returns>True if the user is an admin</returns>
        public async Task<bool> IsChatAdministratorAsync(ChatId chatId, int userId)
        {
            ChatMember[] admins = await GetChatAdministratorsAsync(chatId);
            return admins.Any(x => x.User.Id == userId);
        }

        /// <summary>
        /// Use this method to get the number of members in a chat.
        /// </summary>
        /// <param name="chatId">Unique identifier for the target chat or username of the target supergroup or channel 
        /// (in the format @channelusername)</param>
        /// <returns>The number of chat members</returns>
        public int GetChatMembersCount(ChatId chatId) => GetChatMembersCountAsync(chatId).Result;
        /// <summary>
        /// Use this method to get the number of members in a chat.
        /// </summary>
        /// <param name="chatId">Unique identifier for the target chat or username of the target supergroup or channel 
        /// (in the format @channelusername)</param>
        /// <returns>The number of chat members</returns>
        public async Task<int> GetChatMembersCountAsync(ChatId chatId)
        {
            Dictionary<string, object> args = new Dictionary<string, object>() { { "chat_id", chatId } };
            return await ApiMethodAsync<int>("getChatMembersCount", args);
        }

        /// <summary>
        /// Use this method to get information about a member of a chat.
        /// </summary>
        /// <param name="chatId">Unique identifier for the target chat or username of the target supergroup or channel 
        /// (in the format @channelusername)</param>
        /// <param name="userId">Telegram identifier of target user</param>
        /// <returns>The specified chat member</returns>
        public ChatMember GetChatMember(ChatId chatId, int userId) => GetChatMemberAsync(chatId, userId).Result;
        /// <summary>
        /// Use this method to get information about a member of a chat.
        /// </summary>
        /// <param name="chatId">Unique identifier for the target chat or username of the target supergroup or channel 
        /// (in the format @channelusername)</param>
        /// <param name="userId">Telegram identifier of target user</param>
        /// <returns>The specified chat member</returns>
        public async Task<ChatMember> GetChatMemberAsync(ChatId chatId, int userId)
        {
            Dictionary<string, object> args = new Dictionary<string, object>() { { "chat_id", chatId }, { "user_id", userId } };
            return await ApiMethodAsync<ChatMember>("getChatMember", args);
        }

        /// <summary>
        /// Use this method to set a new group sticker set for a supergroup. 
        /// The bot must be an administrator in the chat for this to work and must have the appropriate admin rights. 
        /// Use the field can_set_sticker_set optionally returned in getChat requests to check if the bot can use this method. 
        /// Returns True on success.
        /// </summary>
        /// <param name="chatId">Unique identifier for the target chat or username of the target supergroup or channel 
        /// (in the format @channelusername)</param>
        /// <param name="stickerSetName">Name of the sticker set to be set as the group sticker set</param>
        /// <returns>True on success</returns>
        public bool SetChatStickerSet(ChatId chatId, string stickerSetName) => SetChatStickerSetAsync(chatId, stickerSetName).Result;
        /// <summary>
        /// Use this method to set a new group sticker set for a supergroup. 
        /// The bot must be an administrator in the chat for this to work and must have the appropriate admin rights. 
        /// Use the field can_set_sticker_set optionally returned in getChat requests to check if the bot can use this method. 
        /// Returns True on success.
        /// </summary>
        /// <param name="chatId">Unique identifier for the target chat or username of the target supergroup or channel 
        /// (in the format @channelusername)</param>
        /// <param name="stickerSetName">Name of the sticker set to be set as the group sticker set</param>
        /// <returns>True on success</returns>
        public async Task<bool> SetChatStickerSetAsync(ChatId chatId, string stickerSetName)
        {
            Dictionary<string, object> args = new Dictionary<string, object>() { { "chat_id", chatId }, { "sticker_set_name", stickerSetName } };
            return await ApiMethodAsync<bool>("setChatStickerSet", args);
        }

        /// <summary>
        /// Use this method to delete a group sticker set from a supergroup. 
        /// The bot must be an administrator in the chat for this to work and must have the appropriate admin rights. 
        /// Use the field can_set_sticker_set optionally returned in getChat requests to check if the bot can use this method. 
        /// Returns True on success.
        /// </summary>
        /// <param name="chatId">Unique identifier for the target chat or username of the target supergroup or channel 
        /// (in the format @channelusername)</param>
        /// <returns>True on success</returns>
        public bool DeleteChatStickerSet(ChatId chatId) => DeleteChatStickerSetAsync(chatId).Result;
        /// <summary>
        /// Use this method to delete a group sticker set from a supergroup. 
        /// The bot must be an administrator in the chat for this to work and must have the appropriate admin rights. 
        /// Use the field can_set_sticker_set optionally returned in getChat requests to check if the bot can use this method. 
        /// Returns True on success.
        /// </summary>
        /// <param name="chatId">Unique identifier for the target chat or username of the target supergroup or channel 
        /// (in the format @channelusername)</param>
        /// <returns>True on success</returns>
        public async Task<bool> DeleteChatStickerSetAsync(ChatId chatId)
        {
            Dictionary<string, object> args = new Dictionary<string, object>() { { "chat_id", chatId } };
            return await ApiMethodAsync<bool>("deleteChatStickerSet", args);
        }
        #endregion
        #region Callback query
        /// <summary>
        /// Use this method to send answers to callback queries sent from inline keyboards. 
        /// The answer will be displayed to the user as a notification at the top of the chat screen or as an alert. 
        /// On success, True is returned.
        /// </summary>
        /// <param name="callbackQueryId">Unique identifier for the query to be answered</param>
        /// <param name="text">Text of the notification. If not specified, nothing will be shown to the user, 0-200 characters</param>
        /// <param name="showAlert">If true, an alert will be shown by the client instead of a notification at the top of the chat screen. 
        /// Defaults to false.</param>
        /// <param name="url">URL that will be opened by the user's client. 
        /// If you have created a Game and accepted the conditions via @Botfather, specify the URL that opens your game – 
        /// note that this will only work if the query comes from a callback_game button. Otherwise, you may use links like t.me/your_bot? 
        /// start = XXXX that open your bot with a parameter.</param>
        /// <param name="cacheTime">The maximum amount of time in seconds that the result of the callback query may be cached client-side. 
        /// Telegram apps will support caching starting in version 3.14. Defaults to 0.</param>
        /// <returns>True on success</returns>
        public bool AnswerCallbackQuery(string callbackQueryId, string text = null, bool showAlert = false,
            string url = null, int cacheTime = 0)
            => AnswerCallbackQueryAsync(callbackQueryId, text, showAlert, url, cacheTime).Result;
        /// <summary>
        /// Use this method to send answers to callback queries sent from inline keyboards. 
        /// The answer will be displayed to the user as a notification at the top of the chat screen or as an alert. 
        /// On success, True is returned.
        /// </summary>
        /// <param name="callbackQueryId">Unique identifier for the query to be answered</param>
        /// <param name="text">Text of the notification. If not specified, nothing will be shown to the user, 0-200 characters</param>
        /// <param name="showAlert">If true, an alert will be shown by the client instead of a notification at the top of the chat screen. 
        /// Defaults to false.</param>
        /// <param name="url">URL that will be opened by the user's client. 
        /// If you have created a Game and accepted the conditions via @Botfather, specify the URL that opens your game – 
        /// note that this will only work if the query comes from a callback_game button. Otherwise, you may use links like t.me/your_bot? 
        /// start = XXXX that open your bot with a parameter.</param>
        /// <param name="cacheTime">The maximum amount of time in seconds that the result of the callback query may be cached client-side. 
        /// Telegram apps will support caching starting in version 3.14. Defaults to 0.</param>
        /// <returns>True on success</returns>
        public async Task<bool> AnswerCallbackQueryAsync(string callbackQueryId, string text = null, bool showAlert = false,
            string url = null, int cacheTime = 0)
        {
            Dictionary<string, object> args = new Dictionary<string, object>() { { "callback_query_id", callbackQueryId } };
            if (!string.IsNullOrWhiteSpace(text)) args.Add("text", text);
            if (showAlert) args.Add("show_alert", true);
            if (!string.IsNullOrWhiteSpace(url)) args.Add("url", url);
            if (cacheTime != 0) args.Add("cache_time", cacheTime);

            return await ApiMethodAsync<bool>("answerCallbackQuery", args);
        }
        #endregion
        #endregion
        #region Updating Messages
        /// <summary>
        /// Use this method to edit text and game messages sent by the bot. 
        /// On success, the edited Message is returned.
        /// </summary>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel 
        /// (in the format @channelusername)</param>
        /// <param name="messageId">Identifier of the message sent by the bot</param>
        /// <param name="text">New text of the message</param>
        /// <param name="parseMode">Send Markdown or HTML, if you want Telegram apps to show bold, 
        /// italic, fixed-width text or inline URLs in your bot's message.</param>
        /// <param name="disableWebPagePreview">Disables link previews for links in this message</param>
        /// <param name="replyMarkup">An inline keyboard.</param>
        /// <returns>The edited message</returns>
        public Message EditMessageText(ChatId chatId, int messageId, string text, ParseMode parseMode = ParseMode.None,
            bool disableWebPagePreview = false, InlineKeyboardMarkup replyMarkup = null)
            => EditMessageTextAsync(chatId, messageId, text, parseMode, disableWebPagePreview, replyMarkup).Result;
        /// <summary>
        /// Use this method to edit text and game messages sent by the bot. 
        /// On success, the edited Message is returned.
        /// </summary>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel 
        /// (in the format @channelusername)</param>
        /// <param name="messageId">Identifier of the message sent by the bot</param>
        /// <param name="text">New text of the message</param>
        /// <param name="parseMode">Send Markdown or HTML, if you want Telegram apps to show bold, 
        /// italic, fixed-width text or inline URLs in your bot's message.</param>
        /// <param name="disableWebPagePreview">Disables link previews for links in this message</param>
        /// <param name="replyMarkup">An inline keyboard.</param>
        /// <returns>The edited message</returns>
        public async Task<Message> EditMessageTextAsync(ChatId chatId, int messageId, string text, ParseMode parseMode = ParseMode.None,
            bool disableWebPagePreview = false, InlineKeyboardMarkup replyMarkup = null)
        {
            Dictionary<string, object> args = new Dictionary<string, object>() { { "chat_id", chatId },
                { "message_id", messageId }, { "text", text } };
            if (parseMode != ParseMode.None) args.Add("parse_mode", Enum.GetString(parseMode));
            if (disableWebPagePreview) args.Add("disable_web_page_preview", true);
            if (replyMarkup != null) args.Add("reply_markup", replyMarkup);

            return await ApiMethodAsync<Message>("editMessageText", args);
        }

        /// <summary>
        /// Use this method to edit text and game messages sent via the bot (for inline bots).
        /// On success, True is returned.
        /// </summary>
        /// <param name="inlineMessageId">Identifier of the inline message</param>
        /// <param name="text">New text of the message</param>
        /// <param name="parseMode">Send Markdown or HTML, if you want Telegram apps to show bold, 
        /// italic, fixed-width text or inline URLs in your bot's message.</param>
        /// <param name="disableWebPagePreview">Disables link previews for links in this message</param>
        /// <param name="replyMarkup">An inline keyboard, if you want any</param>
        /// <returns>True on success</returns>
        public bool EditMessageText(string inlineMessageId, string text, ParseMode parseMode = ParseMode.None,
            bool disableWebPagePreview = false, InlineKeyboardMarkup replyMarkup = null)
            => EditMessageTextAsync(inlineMessageId, text, parseMode, disableWebPagePreview, replyMarkup).Result;
        /// <summary>
        /// Use this method to edit text and game messages sent via the bot (for inline bots).
        /// On success, True is returned.
        /// </summary>
        /// <param name="inlineMessageId">Identifier of the inline message</param>
        /// <param name="text">New text of the message</param>
        /// <param name="parseMode">Send Markdown or HTML, if you want Telegram apps to show bold, 
        /// italic, fixed-width text or inline URLs in your bot's message.</param>
        /// <param name="disableWebPagePreview">Disables link previews for links in this message</param>
        /// <param name="replyMarkup">An inline keyboard, if you want any</param>
        /// <returns>True on success</returns>
        public async Task<bool> EditMessageTextAsync(string inlineMessageId, string text, ParseMode parseMode = ParseMode.None,
            bool disableWebPagePreview = false, InlineKeyboardMarkup replyMarkup = null)
        {
            Dictionary<string, object> args = new Dictionary<string, object>() { { "inline_message_id", inlineMessageId },
                { "text", text } };
            if (parseMode != ParseMode.None) args.Add("parse_mode", Enum.GetString(parseMode));
            if (disableWebPagePreview) args.Add("disable_web_page_preview", true);
            if (replyMarkup != null) args.Add("reply_markup", replyMarkup);

            return await ApiMethodAsync<bool>("editMessageText", args);
        }

        /// <summary>
        /// Use this method to edit captions of messages sent by the bot. 
        /// On success, the edited Message is returned.
        /// </summary>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel 
        /// (in the format @channelusername)</param>
        /// <param name="messageId">Identifier of the message sent by the bot</param>
        /// <param name="caption">New caption of the message</param>
        /// <param name="parseMode">Send Markdown or HTML, if you want Telegram apps to show bold, 
        /// italic, fixed-width text or inline URLs in your bot's message.</param>
        /// <param name="replyMarkup">An inline keyboard.</param>
        /// <returns>The edited message</returns>
        public Message EditMessageCaption(ChatId chatId, int messageId, string caption, ParseMode parseMode = ParseMode.None,
            InlineKeyboardMarkup replyMarkup = null)
            => EditMessageCaptionAsync(chatId, messageId, caption, parseMode, replyMarkup).Result;
        /// <summary>
        /// Use this method to edit captions of messages sent by the bot. 
        /// On success, the edited Message is returned.
        /// </summary>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel 
        /// (in the format @channelusername)</param>
        /// <param name="messageId">Identifier of the message sent by the bot</param>
        /// <param name="caption">New caption of the message</param>
        /// <param name="parseMode">Send Markdown or HTML, if you want Telegram apps to show bold, 
        /// italic, fixed-width text or inline URLs in your bot's message.</param>
        /// <param name="replyMarkup">An inline keyboard.</param>
        /// <returns>The edited message</returns>
        public async Task<Message> EditMessageCaptionAsync(ChatId chatId, int messageId, string caption, ParseMode parseMode = ParseMode.None,
            InlineKeyboardMarkup replyMarkup = null)
        {
            Dictionary<string, object> args = new Dictionary<string, object>() { { "chat_id", chatId },
                { "message_id", messageId }, { "caption", caption } };
            if (parseMode != ParseMode.None) args.Add("parse_mode", Enum.GetString(parseMode));
            if (replyMarkup != null) args.Add("reply_markup", replyMarkup);

            return await ApiMethodAsync<Message>("editMessageCaption", args);
        }

        /// <summary>
        /// Use this method to edit captions of messages sent via the bot (for inline bots). 
        /// On success, True is returned.
        /// </summary>
        /// <param name="inlineMessageId">Identifier of the inline message</param>
        /// <param name="caption">New caption of the message</param>
        /// <param name="parseMode">Send Markdown or HTML, if you want Telegram apps to show bold, 
        /// italic, fixed-width text or inline URLs in your bot's message.</param>
        /// <param name="replyMarkup">An inline keyboard, if you want any</param>
        /// <returns>True on success</returns>
        public bool EditMessageCaption(string inlineMessageId, string caption, ParseMode parseMode = ParseMode.None,
            InlineKeyboardMarkup replyMarkup = null)
            => EditMessageCaptionAsync(inlineMessageId, caption, parseMode, replyMarkup).Result;
        /// <summary>
        /// Use this method to edit captions of messages sent via the bot (for inline bots). 
        /// On success, True is returned.
        /// </summary>
        /// <param name="inlineMessageId">Identifier of the inline message</param>
        /// <param name="caption">New caption of the message</param>
        /// <param name="parseMode">Send Markdown or HTML, if you want Telegram apps to show bold, 
        /// italic, fixed-width text or inline URLs in your bot's message.</param>
        /// <param name="replyMarkup">An inline keyboard, if you want any</param>
        /// <returns>True on success</returns>
        public async Task<bool> EditMessageCaptionAsync(string inlineMessageId, string caption, ParseMode parseMode = ParseMode.None,
            InlineKeyboardMarkup replyMarkup = null)
        {
            Dictionary<string, object> args = new Dictionary<string, object>() { { "inline_message_id", inlineMessageId },
                { "caption", caption } };
            if (parseMode != ParseMode.None) args.Add("parse_mode", Enum.GetString(parseMode));
            if (replyMarkup != null) args.Add("reply_markup", replyMarkup);

            return await ApiMethodAsync<bool>("editMessageCaption", args);
        }

        /// <summary>
        /// Use this method to replace a media the bot sent with another one.
        /// On success, the edited Message is returned.
        /// </summary>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel 
        /// (in the format @channelusername)</param>
        /// <param name="messageId">Identifier of the message sent by the bot</param>
        /// <param name="media">The new media of the message</param>
        /// <param name="replyMarkup">An inline keyboard, if you want any</param>
        /// <returns>The edited message</returns>
        public Message EditMessageMedia(ChatId chatId, int messageId, SendFile media, InlineKeyboardMarkup replyMarkup = null)
            => EditMessageMediaAsync(chatId, messageId, media, replyMarkup).Result;
        /// <summary>
        /// Use this method to replace a media the bot sent with another one.
        /// On success, the edited Message is returned.
        /// </summary>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel 
        /// (in the format @channelusername)</param>
        /// <param name="messageId">Identifier of the message sent by the bot</param>
        /// <param name="media">The new media of the message</param>
        /// <param name="replyMarkup">An inline keyboard, if you want any</param>
        /// <returns>The edited message</returns>
        public async Task<Message> EditMessageMediaAsync(ChatId chatId, int messageId, SendFile media, InlineKeyboardMarkup replyMarkup = null)
        {
            Dictionary<string, object> args = new Dictionary<string, object>() { { "chat_id", chatId },
                { "message_id", messageId }, { "media", media } };
            if (replyMarkup != null) args.Add("reply_markup", replyMarkup);

            return await ApiMethodAsync<Message>("editMessageMedia", args);
        }

        /// <summary>
        /// Use this method to replace a media the bot sent with another one (for inline bots).
        /// On success, the edited Message is returned.
        /// </summary>
        /// <param name="inlineMessageId">Identifier of the inline message</param>
        /// <param name="media">The new media of the message</param>
        /// <param name="replyMarkup">An inline keyboard, if you want any</param>
        /// <returns>The edited message</returns>
        public Message EditMessageMedia(string inlineMessageId, SendFile media, InlineKeyboardMarkup replyMarkup = null)
            => EditMessageMediaAsync(inlineMessageId, media, replyMarkup).Result;
        /// <summary>
        /// Use this method to replace a media the bot sent with another one (for inline bots).
        /// On success, the edited Message is returned.
        /// </summary>
        /// <param name="inlineMessageId">Identifier of the inline message</param>
        /// <param name="media">The new media of the message</param>
        /// <param name="replyMarkup">An inline keyboard, if you want any</param>
        /// <returns>The edited message</returns>
        public async Task<Message> EditMessageMediaAsync(string inlineMessageId, SendFile media, InlineKeyboardMarkup replyMarkup = null)
        {
            Dictionary<string, object> args = new Dictionary<string, object>() { { "inline_message_id", inlineMessageId },
                { "media", media } };
            if (replyMarkup != null) args.Add("reply_markup", replyMarkup);

            return await ApiMethodAsync<Message>("editMessageMedia", args);
        }

        /// <summary>
        /// Use this method to edit only the reply markups of messages sent by the bot. 
        /// On success, the edited Message is returned.
        /// </summary>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel 
        /// (in the format @channelusername)</param>
        /// <param name="messageId">Identifier of the message sent by the bot</param>
        /// <param name="replyMarkup">An inline keyboard.</param>
        /// <returns>The edited message</returns>
        public Message EditMessageReplyMarkup(ChatId chatId, int messageId, InlineKeyboardMarkup replyMarkup = null)
            => EditMessageReplyMarkupAsync(chatId, messageId, replyMarkup).Result;
        /// <summary>
        /// Use this method to edit only the reply markups of messages sent by the bot. 
        /// On success, the edited Message is returned.
        /// </summary>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel 
        /// (in the format @channelusername)</param>
        /// <param name="messageId">Identifier of the message sent by the bot</param>
        /// <param name="replyMarkup">An inline keyboard.</param>
        /// <returns>The edited message</returns>
        public async Task<Message> EditMessageReplyMarkupAsync(ChatId chatId, int messageId, InlineKeyboardMarkup replyMarkup = null)
        {
            Dictionary<string, object> args = new Dictionary<string, object>() { { "chat_id", chatId },
                { "message_id", messageId } };
            if (replyMarkup != null) args.Add("reply_markup", replyMarkup);

            return await ApiMethodAsync<Message>("editMessageReplyMarkup", args);
        }

        /// <summary>
        /// Use this method to edit only the reply markups of messages sent via the bot (for inline bots). 
        /// On success, True is returned.
        /// </summary>
        /// <param name="inlineMessageId">Identifier of the inline message</param>
        /// <param name="replyMarkup">An inline keyboard, if you want any</param>
        /// <returns>True on success</returns>
        public bool EditMessageReplyMarkup(string inlineMessageId, InlineKeyboardMarkup replyMarkup = null)
            => EditMessageReplyMarkupAsync(inlineMessageId, replyMarkup).Result;
        /// <summary>
        /// Use this method to edit only the reply markups of messages sent via the bot (for inline bots). 
        /// On success, True is returned.
        /// </summary>
        /// <param name="inlineMessageId">Identifier of the inline message</param>
        /// <param name="replyMarkup">An inline keyboard, if you want any</param>
        /// <returns>True on success</returns>
        public async Task<bool> EditMessageReplyMarkupAsync(string inlineMessageId, InlineKeyboardMarkup replyMarkup = null)
        {
            Dictionary<string, object> args = new Dictionary<string, object>() { { "inline_message_id", inlineMessageId } };
            if (replyMarkup != null) args.Add("reply_markup", replyMarkup);

            return await ApiMethodAsync<bool>("editMessageReplyMarkup", args);
        }

        /// <summary>
        /// Use this method to delete a message, including service messages, with the following limitations:
        /// <para>- A message can only be deleted if it was sent less than 48 hours ago.</para>
        /// <para>- Bots can delete outgoing messages in groups and supergroups.</para>
        /// <para>- Bots granted can_post_messages permissions can delete outgoing messages in channels.</para>
        /// <para>- If the bot is an administrator of a group, it can delete any message there.</para>
        /// <para>- If the bot has can_delete_messages permission in a supergroup or a channel, it can delete any message there.</para>
        /// <para>Returns True on success.</para>
        /// </summary>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel 
        /// (in the format @channelusername)</param>
        /// <param name="messageId">Identifier of the message to delete</param>
        /// <returns>True on success</returns>
        public bool DeleteMessage(ChatId chatId, int messageId) => DeleteMessageAsync(chatId, messageId).Result;
        /// <summary>
        /// Use this method to delete a message, including service messages, with the following limitations:
        /// <para>- A message can only be deleted if it was sent less than 48 hours ago.</para>
        /// <para>- Bots can delete outgoing messages in groups and supergroups.</para>
        /// <para>- Bots granted can_post_messages permissions can delete outgoing messages in channels.</para>
        /// <para>- If the bot is an administrator of a group, it can delete any message there.</para>
        /// <para>- If the bot has can_delete_messages permission in a supergroup or a channel, it can delete any message there.</para>
        /// <para>Returns True on success.</para>
        /// </summary>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel 
        /// (in the format @channelusername)</param>
        /// <param name="messageId">Identifier of the message to delete</param>
        /// <returns>True on success</returns>
        public async Task<bool> DeleteMessageAsync(ChatId chatId, int messageId)
        {
            Dictionary<string, object> args = new Dictionary<string, object>() { { "chat_id", chatId }, { "message_id", messageId } };
            return await ApiMethodAsync<bool>("deleteMessage", args);
        }
        #endregion
        #region Stickers
        /// <summary>
        /// Use this method to send .webp stickers. On success, the sent Message is returned.
        /// </summary>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel 
        /// (in the format @channelusername)</param>
        /// <param name="sticker">Sticker to send. A .webp file an one of either <see cref="SendFileId"/>, <see cref="SendFileUrl"/>
        /// or <see cref="SendFileMultipart"/></param>
        /// <param name="disableNotification">Sends the message silently. Users will receive a notification with no sound.</param>
        /// <param name="replyToMessageId">If the message is a reply, ID of the original message</param>
        /// <param name="replyMarkup">Additional interface options.</param>
        /// <returns>The sent message</returns>
        public Message SendSticker(ChatId chatId, SendFile sticker, bool disableNotification = false,
            int replyToMessageId = -1, ReplyMarkupBase replyMarkup = null)
            => SendStickerAsync(chatId, sticker, disableNotification, replyToMessageId, replyMarkup).Result;
        /// <summary>
        /// Use this method to send .webp stickers. On success, the sent Message is returned.
        /// </summary>
        /// <param name="chatId">Unique identifier for the target chat or username of the target channel 
        /// (in the format @channelusername)</param>
        /// <param name="sticker">Sticker to send. A .webp file an one of either <see cref="SendFileId"/>, <see cref="SendFileUrl"/>
        /// or <see cref="SendFileMultipart"/></param>
        /// <param name="disableNotification">Sends the message silently. Users will receive a notification with no sound.</param>
        /// <param name="replyToMessageId">If the message is a reply, ID of the original message</param>
        /// <param name="replyMarkup">Additional interface options.</param>
        /// <returns>The sent message</returns>
        public async Task<Message> SendStickerAsync(ChatId chatId, SendFile sticker, bool disableNotification = false,
            int replyToMessageId = -1, ReplyMarkupBase replyMarkup = null)
        {
            Dictionary<string, object> args = new Dictionary<string, object>() { { "chat_id", chatId }, { "sticker", sticker } };
            if (disableNotification) args.Add("disable_notification", true);
            if (replyToMessageId != -1) args.Add("reply_to_message_id", replyToMessageId);
            if (replyMarkup != null) args.Add("reply_markup", replyMarkup);

            return await ApiMethodAsync<Message>("sendSticker", args);
        }

        /// <summary>
        /// Use this method to get a sticker set. On success, a StickerSet object is returned.
        /// </summary>
        /// <param name="name">Name of the sticker set</param>
        /// <returns>The sticker set, of course</returns>
        public StickerSet GetStickerSet(string name) => GetStickerSetAsync(name).Result;
        /// <summary>
        /// Use this method to get a sticker set. On success, a StickerSet object is returned.
        /// </summary>
        /// <param name="name">Name of the sticker set</param>
        /// <returns>The sticker set, of course</returns>
        public async Task<StickerSet> GetStickerSetAsync(string name)
        {
            Dictionary<string, object> args = new Dictionary<string, object>() { { "name", name } };
            return await ApiMethodAsync<StickerSet>("getStickerSet", args);
        }

        /// <summary>
        /// Use this method to upload a .png file with a sticker for later use in createNewStickerSet 
        /// and addStickerToSet methods (can be used multiple times). Returns the uploaded File on success.
        /// </summary>
        /// <param name="userId">User identifier of sticker file owner</param>
        /// <param name="pngSticker">Png image with the sticker, must be up to 512 kilobytes in size, 
        /// dimensions must not exceed 512px, and either width or height must be exactly 512px.</param>
        /// <returns>The uploaded file (containing only the fileId)</returns>
        public File UploadStickerFile(int userId, SendFile pngSticker) => UploadStickerFileAsync(userId, pngSticker).Result;
        /// <summary>
        /// Use this method to upload a .png file with a sticker for later use in createNewStickerSet 
        /// and addStickerToSet methods (can be used multiple times). Returns the uploaded File on success.
        /// </summary>
        /// <param name="userId">User identifier of sticker file owner</param>
        /// <param name="pngSticker">Png image with the sticker, must be up to 512 kilobytes in size, 
        /// dimensions must not exceed 512px, and either width or height must be exactly 512px.</param>
        /// <returns>The uploaded file (containing only the fileId)</returns>
        public async Task<File> UploadStickerFileAsync(int userId, SendFile pngSticker)
        {
            Dictionary<string, object> args = new Dictionary<string, object>() { { "user_id", userId }, { "png_sticker", pngSticker } };
            return await ApiMethodAsync<File>("uploadStickerFile", args);
        }

        /// <summary>
        /// Use this method to create new sticker set owned by a user. 
        /// The bot will be able to edit the created sticker set. Returns True on success.
        /// </summary>
        /// <param name="userId">User identifier of created sticker set owner</param>
        /// <param name="name">Short name of sticker set, to be used in t.me/addstickers/ URLs (e.g., animals). 
        /// Can contain only english letters, digits and underscores. 
        /// Must begin with a letter, can't contain consecutive underscores and must end in “_by_&lt;bot username&gt;”. 
        /// &lt;bot_username&gt; is case insensitive. 1-64 characters.</param>
        /// <param name="title">Sticker set title, 1-64 characters</param>
        /// <param name="pngSticker">Png image with the sticker, must be up to 512 kilobytes in size, dimensions must not exceed 512px, 
        /// and either width or height must be exactly 512px. One of either <see cref="SendFileId"/>, <see cref="SendFileUrl"/>
        ///  or <see cref="SendFileMultipart"/></param>
        /// <param name="emojis">One or more emoji corresponding to the sticker</param>
        /// <param name="containsMasks">Pass True, if a set of mask stickers should be created</param>
        /// <param name="maskPosition">An object for position where the mask should be placed on faces</param>
        /// <returns>True on success</returns>
        public bool CreateNewStickerSet(int userId, string name, string title, SendFile pngSticker,
            string emojis, bool containsMasks = false, MaskPosition maskPosition = null)
            => CreateNewStickerSetAsync(userId, name, title, pngSticker, emojis, containsMasks, maskPosition).Result;
        /// <summary>
        /// Use this method to create new sticker set owned by a user. 
        /// The bot will be able to edit the created sticker set. Returns True on success.
        /// </summary>
        /// <param name="userId">User identifier of created sticker set owner</param>
        /// <param name="name">Short name of sticker set, to be used in t.me/addstickers/ URLs (e.g., animals). 
        /// Can contain only english letters, digits and underscores. 
        /// Must begin with a letter, can't contain consecutive underscores and must end in “_by_&lt;bot username&gt;”. 
        /// &lt;bot_username&gt; is case insensitive. 1-64 characters.</param>
        /// <param name="title">Sticker set title, 1-64 characters</param>
        /// <param name="pngSticker">Png image with the sticker, must be up to 512 kilobytes in size, dimensions must not exceed 512px, 
        /// and either width or height must be exactly 512px. One of either <see cref="SendFileId"/>, <see cref="SendFileUrl"/>
        ///  or <see cref="SendFileMultipart"/></param>
        /// <param name="emojis">One or more emoji corresponding to the sticker</param>
        /// <param name="containsMasks">Pass True, if a set of mask stickers should be created</param>
        /// <param name="maskPosition">An object for position where the mask should be placed on faces</param>
        /// <returns>True on success</returns>
        public async Task<bool> CreateNewStickerSetAsync(int userId, string name, string title, SendFile pngSticker,
            string emojis, bool containsMasks = false, MaskPosition maskPosition = null)
        {
            Dictionary<string, object> args = new Dictionary<string, object>()
            {
                { "user_id", userId }, { "name", name }, { "title", title },
                { "png_sticker", pngSticker }, { "emojis", emojis }
            };
            if (containsMasks) args.Add("contains_masks", true);
            if (maskPosition != null) args.Add("mask_position", maskPosition);

            return await ApiMethodAsync<bool>("createNewStickerSet", args);
        }

        /// <summary>
        /// Use this method to add a new sticker to a set created by the bot. Returns True on success.
        /// </summary>
        /// <param name="userId">User identifier of sticker set owner</param>
        /// <param name="name">Sticker set name</param>
        /// <param name="pngSticker">Png image with the sticker, must be up to 512 kilobytes in size, dimensions must not exceed 512px, 
        /// and either width or height must be exactly 512px. One of either <see cref="SendFileId"/>, <see cref="SendFileUrl"/> 
        /// or <see cref="SendFileMultipart"/></param>
        /// <param name="emojis">One or more emoji corresponding to the sticker</param>
        /// <param name="maskPosition">An object for where the mask should be placed on faces, if the sticker is one</param>
        /// <returns>True on success</returns>
        public bool AddStickerToSet(int userId, string name, SendFile pngSticker, string emojis,
            MaskPosition maskPosition = null)
            => AddStickerToSetAsync(userId, name, pngSticker, emojis, maskPosition).Result;
        /// <summary>
        /// Use this method to add a new sticker to a set created by the bot. Returns True on success.
        /// </summary>
        /// <param name="userId">User identifier of sticker set owner</param>
        /// <param name="name">Sticker set name</param>
        /// <param name="pngSticker">Png image with the sticker, must be up to 512 kilobytes in size, dimensions must not exceed 512px, 
        /// and either width or height must be exactly 512px. One of either <see cref="SendFileId"/>, <see cref="SendFileUrl"/> 
        /// or <see cref="SendFileMultipart"/></param>
        /// <param name="emojis">One or more emoji corresponding to the sticker</param>
        /// <param name="maskPosition">An object for where the mask should be placed on faces, if the sticker is one</param>
        /// <returns>True on success</returns>
        public async Task<bool> AddStickerToSetAsync(int userId, string name, SendFile pngSticker, string emojis,
            MaskPosition maskPosition = null)
        {
            Dictionary<string, object> args = new Dictionary<string, object>()
            {
                { "user_id", userId }, { "name", name },
                { "png_sticker", pngSticker }, { "emojis", emojis }
            };
            if (maskPosition != null) args.Add("mask_position", maskPosition);

            return await ApiMethodAsync<bool>("addStickerToSet", args);
        }

        /// <summary>
        /// Use this method to move a sticker in a set created by the bot to a specific position . Returns True on success.
        /// </summary>
        /// <param name="sticker">File identifier of the sticker</param>
        /// <param name="position">New position in the set, zero-based</param>
        /// <returns>True on success</returns>
        public bool SetStickerPositionInSet(string sticker, int position)
            => SetStickerPositionInSetAsync(sticker, position).Result;
        /// <summary>
        /// Use this method to move a sticker in a set created by the bot to a specific position . Returns True on success.
        /// </summary>
        /// <param name="sticker">File identifier of the sticker</param>
        /// <param name="position">New position in the set, zero-based</param>
        /// <returns>True on success</returns>
        public async Task<bool> SetStickerPositionInSetAsync(string sticker, int position)
        {
            Dictionary<string, object> args = new Dictionary<string, object>() { { "sticker", sticker }, { "position", position } };
            return await ApiMethodAsync<bool>("setStickerPositionInSet", args);
        }

        /// <summary>
        /// Use this method to delete a sticker from a set created by the bot. Returns True on success.
        /// </summary>
        /// <param name="sticker">File identifier of the sticker</param>
        /// <returns>True on success</returns>
        public bool DeleteStickerFromSet(string sticker) => DeleteStickerFromSetAsync(sticker).Result;
        /// <summary>
        /// Use this method to delete a sticker from a set created by the bot. Returns True on success.
        /// </summary>
        /// <param name="sticker">File identifier of the sticker</param>
        /// <returns>True on success</returns>
        public async Task<bool> DeleteStickerFromSetAsync(string sticker)
        {
            Dictionary<string, object> args = new Dictionary<string, object>() { { "sticker", sticker } };
            return await ApiMethodAsync<bool>("deleteStickerFromSet", args);
        }
        #endregion
        #region Inline mode
        /// <summary>
        /// Use this method to send answers to an inline query. On success, True is returned.
        /// No more than 50 results per query are allowed.
        /// </summary>
        /// <param name="inlineQueryId">Unique identifier for the answered query</param>
        /// <param name="results">The results for the inline query</param>
        /// <param name="cacheTime">The maximum amount of time in seconds that the result of the inline query may be 
        /// cached on the server. Defaults to 300.</param>
        /// <param name="isPersonal">Pass True, if results may be cached on the server side only for the user that sent the query. 
        /// By default, results may be returned to any user who sends the same query</param>
        /// <param name="nextOffset">Pass the offset that a client should send in the next query with the same text 
        /// to receive more results. Pass an empty string if there are no more results or if you don‘t support pagination. 
        /// Offset length can’t exceed 64 bytes.</param>
        /// <param name="switchPmText">If passed, clients will display a button with specified text that switches 
        /// the user to a private chat with the bot and sends the bot a start message with the parameter switch_pm_parameter</param>
        /// <param name="switchPmParameter">Deep-linking parameter for the /start message sent to the bot when user presses the switch button. 
        /// 1-64 characters, only A-Z, a-z, 0-9, _ and - are allowed.</param>
        /// <returns>True on success</returns>
        public bool AnswerInlineQuery(string inlineQueryId, InlineQueryResult[] results, int cacheTime = 300,
            bool isPersonal = false, string nextOffset = null, string switchPmText = null, string switchPmParameter = null)
            => AnswerInlineQueryAsync(inlineQueryId, results, cacheTime, isPersonal, nextOffset, switchPmText, switchPmParameter).Result;
        /// <summary>
        /// Use this method to send answers to an inline query. On success, True is returned.
        /// No more than 50 results per query are allowed.
        /// </summary>
        /// <param name="inlineQueryId">Unique identifier for the answered query</param>
        /// <param name="results">The results for the inline query</param>
        /// <param name="cacheTime">The maximum amount of time in seconds that the result of the inline query may be 
        /// cached on the server. Defaults to 300.</param>
        /// <param name="isPersonal">Pass True, if results may be cached on the server side only for the user that sent the query. 
        /// By default, results may be returned to any user who sends the same query</param>
        /// <param name="nextOffset">Pass the offset that a client should send in the next query with the same text 
        /// to receive more results. Pass an empty string if there are no more results or if you don‘t support pagination. 
        /// Offset length can’t exceed 64 bytes.</param>
        /// <param name="switchPmText">If passed, clients will display a button with specified text that switches 
        /// the user to a private chat with the bot and sends the bot a start message with the parameter switch_pm_parameter</param>
        /// <param name="switchPmParameter">Deep-linking parameter for the /start message sent to the bot when user presses the switch button. 
        /// 1-64 characters, only A-Z, a-z, 0-9, _ and - are allowed.</param>
        /// <returns>True on success</returns>
        public async Task<bool> AnswerInlineQueryAsync(string inlineQueryId, InlineQueryResult[] results, int cacheTime = 300,
            bool isPersonal = false, string nextOffset = null, string switchPmText = null, string switchPmParameter = null)
        {
            Dictionary<string, object> args = new Dictionary<string, object>() { { "inline_query_id", inlineQueryId },
                { "results", results } };
            if (cacheTime != 300) args.Add("cache_time", cacheTime);
            if (isPersonal) args.Add("is_personal", true);
            if (!string.IsNullOrWhiteSpace(nextOffset)) args.Add("next_offset", nextOffset);
            if (!string.IsNullOrWhiteSpace(switchPmText)) args.Add("switch_pm_text", switchPmText);
            if (!string.IsNullOrWhiteSpace(switchPmParameter)) args.Add("switch_pm_parameter", switchPmParameter);

            return await ApiMethodAsync<bool>("answerInlineQuery", args);
        }
        #endregion
        #region Payment
        /// <summary>
        /// Use this method to send invoices. On success, the sent Message is returned.
        /// </summary>
        /// <param name="chatId">Unique identifier for the target private chat</param>
        /// <param name="title">Product name, 1-32 characters</param>
        /// <param name="description">Product description, 1-255 characters</param>
        /// <param name="payload">Bot-defined invoice payload, 1-128 bytes. 
        /// This will not be displayed to the user, use for your internal processes.</param>
        /// <param name="providerToken">Payments provider token, obtained via Botfather</param>
        /// <param name="startParameter">Unique deep-linking parameter that can be used to generate 
        /// this invoice when used as a start parameter</param>
        /// <param name="currency">Three-letter ISO 4217 currency code</param>
        /// <param name="prices">Price breakdown, a list of components 
        /// (e.g. product price, tax, discount, delivery cost, delivery tax, bonus, etc.)</param>
        /// <param name="providerData">JSON-encoded data about the invoice, which will be shared with the payment provider. 
        /// A detailed description of required fields should be provided by the payment provider.</param>
        /// <param name="photoUrl">URL of the product photo for the invoice. Can be a photo of the goods or a marketing image for a service. 
        /// People like it better when they see what they are paying for.</param>
        /// <param name="photoSize">Photo size</param>
        /// <param name="photoWidth">Photo width</param>
        /// <param name="photoHeight">Photo height</param>
        /// <param name="needName">Pass True, if you require the user's full name to complete the order</param>
        /// <param name="needPhoneNumber">Pass True, if you require the user's phone number to complete the order</param>
        /// <param name="needEmail">Pass True, if you require the user's email address to complete the order</param>
        /// <param name="needShippingAddress">Pass True, if you require the user's shipping address to complete the order</param>
        /// <param name="sendPhoneNumberToProvider">Pass True, if user's phone number should be sent to provider</param>
        /// <param name="sendEmailToProvider">Pass True, if user's email address should be sent to provider</param>
        /// <param name="isFlexible">Pass True, if the final price depends on the shipping method</param>
        /// <param name="disableNotification">Sends the message silently. Users will receive a notification with no sound.</param>
        /// <param name="replyToMessageId">If the message is a reply, ID of the original message</param>
        /// <param name="replyMarkup">An object for an inline keyboard. If empty, one 'Pay total price' button will be shown. 
        /// If not empty, the first button must be a Pay button.</param>
        /// <returns>The sent message</returns>
        public Message SendInvoice(int chatId, string title, string description, string payload, string providerToken,
            string startParameter, string currency, LabeledPrice[] prices, string providerData = null, string photoUrl = null,
            int photoSize = 0, int photoWidth = 0, int photoHeight = 0, bool needName = false, bool needPhoneNumber = false,
            bool needEmail = false, bool needShippingAddress = false, bool sendPhoneNumberToProvider = false, bool sendEmailToProvider = false,
            bool isFlexible = false, bool disableNotification = false, int replyToMessageId = -1, InlineKeyboardMarkup replyMarkup = null)
            => SendInvoiceAsync(chatId, title, description, payload, providerToken, startParameter, currency, prices, providerData,
                photoUrl, photoSize, photoWidth, photoHeight, needName, needPhoneNumber, needEmail, needShippingAddress,
                sendPhoneNumberToProvider, sendEmailToProvider, isFlexible, disableNotification, replyToMessageId, replyMarkup).Result;
        /// <summary>
        /// Use this method to send invoices. On success, the sent Message is returned.
        /// </summary>
        /// <param name="chatId">Unique identifier for the target private chat</param>
        /// <param name="title">Product name, 1-32 characters</param>
        /// <param name="description">Product description, 1-255 characters</param>
        /// <param name="payload">Bot-defined invoice payload, 1-128 bytes. 
        /// This will not be displayed to the user, use for your internal processes.</param>
        /// <param name="providerToken">Payments provider token, obtained via Botfather</param>
        /// <param name="startParameter">Unique deep-linking parameter that can be used to generate 
        /// this invoice when used as a start parameter</param>
        /// <param name="currency">Three-letter ISO 4217 currency code</param>
        /// <param name="prices">Price breakdown, a list of components 
        /// (e.g. product price, tax, discount, delivery cost, delivery tax, bonus, etc.)</param>
        /// <param name="providerData">JSON-encoded data about the invoice, which will be shared with the payment provider. 
        /// A detailed description of required fields should be provided by the payment provider.</param>
        /// <param name="photoUrl">URL of the product photo for the invoice. Can be a photo of the goods or a marketing image for a service. 
        /// People like it better when they see what they are paying for.</param>
        /// <param name="photoSize">Photo size</param>
        /// <param name="photoWidth">Photo width</param>
        /// <param name="photoHeight">Photo height</param>
        /// <param name="needName">Pass True, if you require the user's full name to complete the order</param>
        /// <param name="needPhoneNumber">Pass True, if you require the user's phone number to complete the order</param>
        /// <param name="needEmail">Pass True, if you require the user's email address to complete the order</param>
        /// <param name="needShippingAddress">Pass True, if you require the user's shipping address to complete the order</param>
        /// <param name="sendPhoneNumberToProvider">Pass True, if user's phone number should be sent to provider</param>
        /// <param name="sendEmailToProvider">Pass True, if user's email address should be sent to provider</param>
        /// <param name="isFlexible">Pass True, if the final price depends on the shipping method</param>
        /// <param name="disableNotification">Sends the message silently. Users will receive a notification with no sound.</param>
        /// <param name="replyToMessageId">If the message is a reply, ID of the original message</param>
        /// <param name="replyMarkup">An object for an inline keyboard. If empty, one 'Pay total price' button will be shown. 
        /// If not empty, the first button must be a Pay button.</param>
        /// <returns>The sent message</returns>
        public async Task<Message> SendInvoiceAsync(int chatId, string title, string description, string payload, string providerToken,
            string startParameter, string currency, LabeledPrice[] prices, string providerData = null, string photoUrl = null,
            int photoSize = 0, int photoWidth = 0, int photoHeight = 0, bool needName = false, bool needPhoneNumber = false,
            bool needEmail = false, bool needShippingAddress = false, bool sendPhoneNumberToProvider = false, bool sendEmailToProvider = false,
            bool isFlexible = false, bool disableNotification = false, int replyToMessageId = -1, InlineKeyboardMarkup replyMarkup = null)
        {
            Dictionary<string, object> args = new Dictionary<string, object>()
            {
                { "chat_id", chatId }, { "title", title }, { "description", description }, { "payload", payload },
                { "provider_token", providerToken }, { "startParameter", startParameter }, { "currency", currency },
                { "prices", prices }
            };
            if (!string.IsNullOrWhiteSpace(providerData)) args.Add("provider_data", providerData);
            if (!string.IsNullOrWhiteSpace(photoUrl)) args.Add("photo_url", photoUrl);
            if (photoSize != 0) args.Add("photo_size", photoSize);
            if (photoWidth != 0) args.Add("photo_width", photoWidth);
            if (photoHeight != 0) args.Add("photo_height", photoHeight);
            if (needName) args.Add("need_name", true);
            if (needPhoneNumber) args.Add("need_phone_number", true);
            if (needEmail) args.Add("need_email", true);
            if (needShippingAddress) args.Add("need_shipping_address", true);
            if (sendPhoneNumberToProvider) args.Add("send_phone_number_to_provider", true);
            if (sendEmailToProvider) args.Add("send_email_to_provider", true);
            if (isFlexible) args.Add("is_flexible", true);
            if (disableNotification) args.Add("disable_notification", true);
            if (replyToMessageId != -1) args.Add("reply_to_message_id", replyToMessageId);
            if (replyMarkup != null) args.Add("reply_markup", replyMarkup);

            return await ApiMethodAsync<Message>("sendInvoice", args);
        }

        /// <summary>
        /// If you sent an invoice requesting a shipping address and the parameter is_flexible was specified, 
        /// the Bot API will send an Update with a shipping_query field to the bot. 
        /// Use this method to reply to shipping queries. On success, True is returned.
        /// </summary>
        /// <param name="shippingQueryId">Unique identifier for the query to be answered</param>
        /// <param name="ok">Specify True if delivery to the specified address is possible and False if there are any problems 
        /// (for example, if delivery to the specified address is not possible)</param>
        /// <param name="shippingOptions">Required if ok is True. An array of available shipping options.</param>
        /// <param name="errorMessage">Required if ok is False. Error message in human readable form that explains why 
        /// it is impossible to complete the order (e.g. "Sorry, delivery to your desired address is unavailable'). 
        /// Telegram will display this message to the user.</param>
        /// <returns>True on success</returns>
        public bool AnswerShippingQuery(string shippingQueryId, bool ok, ShippingOption[] shippingOptions = null,
            string errorMessage = null)
            => AnswerShippingQueryAsync(shippingQueryId, ok, shippingOptions).Result;
        /// <summary>
        /// If you sent an invoice requesting a shipping address and the parameter is_flexible was specified, 
        /// the Bot API will send an Update with a shipping_query field to the bot. 
        /// Use this method to reply to shipping queries. On success, True is returned.
        /// </summary>
        /// <param name="shippingQueryId">Unique identifier for the query to be answered</param>
        /// <param name="ok">Specify True if delivery to the specified address is possible and False if there are any problems 
        /// (for example, if delivery to the specified address is not possible)</param>
        /// <param name="shippingOptions">Required if ok is True. An array of available shipping options.</param>
        /// <param name="errorMessage">Required if ok is False. Error message in human readable form that explains why 
        /// it is impossible to complete the order (e.g. "Sorry, delivery to your desired address is unavailable'). 
        /// Telegram will display this message to the user.</param>
        /// <returns>True on success</returns>
        public async Task<bool> AnswerShippingQueryAsync(string shippingQueryId, bool ok, ShippingOption[] shippingOptions = null,
            string errorMessage = null)
        {
            Dictionary<string, object> args = new Dictionary<string, object>() { { "shipping_query_id", shippingQueryId }, { "ok", ok } };
            if (ok) args.Add("shipping_options", shippingOptions);
            else args.Add("error_message", errorMessage);

            return await ApiMethodAsync<bool>("answerShippingQuery", args);
        }

        /// <summary>
        /// Once the user has confirmed their payment and shipping details, the Bot API sends the final confirmation 
        /// in the form of an Update with the field pre_checkout_query. Use this method to respond to such pre-checkout queries. 
        /// On success, True is returned. Note: The Bot API must receive an answer within 10 seconds after the pre-checkout query was sent.
        /// </summary>
        /// <param name="preCheckoutQueryId">Unique identifier for the query to be answered</param>
        /// <param name="ok">Specify True if everything is alright (goods are available, etc.) and the bot is ready to proceed with the order. 
        /// Use False if there are any problems.</param>
        /// <param name="errorMessage">Required if ok is False. 
        /// Error message in human readable form that explains the reason for failure to proceed with the checkout 
        /// (e.g. "Sorry, somebody just bought the last of our amazing black T-shirts while you were busy filling out your payment details. 
        /// Please choose a different color or garment!"). Telegram will display this message to the user.</param>
        /// <returns>True on success</returns>
        public bool AnswerPreCheckoutQuery(string preCheckoutQueryId, bool ok, string errorMessage = null)
            => AnswerPreCheckoutQueryAsync(preCheckoutQueryId, ok, errorMessage).Result;
        /// <summary>
        /// Once the user has confirmed their payment and shipping details, the Bot API sends the final confirmation 
        /// in the form of an Update with the field pre_checkout_query. Use this method to respond to such pre-checkout queries. 
        /// On success, True is returned. Note: The Bot API must receive an answer within 10 seconds after the pre-checkout query was sent.
        /// </summary>
        /// <param name="preCheckoutQueryId">Unique identifier for the query to be answered</param>
        /// <param name="ok">Specify True if everything is alright (goods are available, etc.) and the bot is ready to proceed with the order. 
        /// Use False if there are any problems.</param>
        /// <param name="errorMessage">Required if ok is False. 
        /// Error message in human readable form that explains the reason for failure to proceed with the checkout 
        /// (e.g. "Sorry, somebody just bought the last of our amazing black T-shirts while you were busy filling out your payment details. 
        /// Please choose a different color or garment!"). Telegram will display this message to the user.</param>
        /// <returns>True on success</returns>
        public async Task<bool> AnswerPreCheckoutQueryAsync(string preCheckoutQueryId, bool ok, string errorMessage = null)
        {
            Dictionary<string, object> args = new Dictionary<string, object>() { { "pre_checkout_query_id", preCheckoutQueryId }, { "ok", ok } };
            if (!ok) args.Add("error_message", errorMessage);

            return await ApiMethodAsync<bool>("answerPreCheckoutQuery", args);
        }
        #endregion
        #region Games
        /// <summary>
        /// Use this method to send a game. On success, the sent Message is returned.
        /// </summary>
        /// <param name="chatId">Unique identifier for the target chat</param>
        /// <param name="gameShortName">Short name of the game, serves as the unique identifier for the game. 
        /// Set up your games via Botfather.</param>
        /// <param name="disableNotification">Sends the message silently. Users will receive a notification with no sound.</param>
        /// <param name="replyToMessageId">If the message is a reply, ID of the original message</param>
        /// <param name="replyMarkup">An object for an inline keyboard. If empty, one ‘Play game_title’ button will be shown. 
        /// If not empty, the first button must launch the game.</param>
        /// <returns>The sent Message</returns>
        public Message SendGame(long chatId, string gameShortName, bool disableNotification = false,
            int replyToMessageId = -1, InlineKeyboardMarkup replyMarkup = null)
            => SendGameAsync(chatId, gameShortName, disableNotification, replyToMessageId, replyMarkup).Result;
        /// <summary>
        /// Use this method to send a game. On success, the sent Message is returned.
        /// </summary>
        /// <param name="chatId">Unique identifier for the target chat</param>
        /// <param name="gameShortName">Short name of the game, serves as the unique identifier for the game. 
        /// Set up your games via Botfather.</param>
        /// <param name="disableNotification">Sends the message silently. Users will receive a notification with no sound.</param>
        /// <param name="replyToMessageId">If the message is a reply, ID of the original message</param>
        /// <param name="replyMarkup">An object for an inline keyboard. If empty, one ‘Play game_title’ button will be shown. 
        /// If not empty, the first button must launch the game.</param>
        /// <returns>The sent Message</returns>
        public async Task<Message> SendGameAsync(long chatId, string gameShortName, bool disableNotification = false,
            int replyToMessageId = -1, InlineKeyboardMarkup replyMarkup = null)
        {
            Dictionary<string, object> args = new Dictionary<string, object>() { { "chat_id", chatId }, { "gameShortName", gameShortName } };
            if (disableNotification) args.Add("disable_notification", true);
            if (replyToMessageId != -1) args.Add("reply_to_message_id", replyToMessageId);
            if (replyMarkup != null) args.Add("reply_markup", replyMarkup);

            return await ApiMethodAsync<Message>("sendGame", args);
        }

        /// <summary>
        /// Use this method to set the score of the specified user in a game. 
        /// On success, returns the edited Message. 
        /// Returns an error, if the new score is not greater than the user's current score in the chat and force is False.
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <param name="score">New score, must be non-negative</param>
        /// <param name="chatId">Unique identifier for the target chat</param>
        /// <param name="messageId">Identifier of the sent message</param>
        /// <param name="force">Pass True, if the high score is allowed to decrease. This can be useful when fixing mistakes or banning cheaters</param>
        /// <param name="disableEditMessage">Pass True, if the game message should not be automatically edited to include the current scoreboard</param>
        /// <returns>The edited message</returns>
        public Message SetGameScore(int userId, int score, long chatId, int messageId, bool force = false,
            bool disableEditMessage = false)
            => SetGameScoreAsync(userId, score, chatId, messageId, force, disableEditMessage).Result;
        /// <summary>
        /// Use this method to set the score of the specified user in a game. 
        /// On success, returns the edited Message. 
        /// Returns an error, if the new score is not greater than the user's current score in the chat and force is False.
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <param name="score">New score, must be non-negative</param>
        /// <param name="chatId">Unique identifier for the target chat</param>
        /// <param name="messageId">Identifier of the sent message</param>
        /// <param name="force">Pass True, if the high score is allowed to decrease. This can be useful when fixing mistakes or banning cheaters</param>
        /// <param name="disableEditMessage">Pass True, if the game message should not be automatically edited to include the current scoreboard</param>
        /// <returns>The edited message</returns>
        public async Task<Message> SetGameScoreAsync(int userId, int score, long chatId, int messageId, bool force = false,
            bool disableEditMessage = false)
        {
            Dictionary<string, object> args = new Dictionary<string, object>() { { "user_id", userId }, { "score", score },
                { "chat_id", chatId }, { "message_id", messageId } };
            if (force) args.Add("force", true);
            if (disableEditMessage) args.Add("disable_edit_message", true);

            return await ApiMethodAsync<Message>("setGameScore", args);
        }

        /// <summary>
        /// Use this method to set the score of the specified user in a game. 
        /// On success, returns True. 
        /// Returns an error, if the new score is not greater than the user's current score in the chat and force is False.
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <param name="score">New score, must be non-negative</param>
        /// <param name="inlineMessageId">Identifier of the inline message</param>
        /// <param name="force">Pass True, if the high score is allowed to decrease. This can be useful when fixing mistakes or banning cheaters</param>
        /// <param name="disableEditMessage">Pass True, if the game message should not be automatically edited to include the current scoreboard</param>
        /// <returns>True on success</returns>
        public bool SetGameScore(int userId, int score, string inlineMessageId, bool force = false,
            bool disableEditMessage = false)
            => SetGameScoreAsync(userId, score, inlineMessageId, force, disableEditMessage).Result;
        /// <summary>
        /// Use this method to set the score of the specified user in a game. 
        /// On success, returns True. 
        /// Returns an error, if the new score is not greater than the user's current score in the chat and force is False.
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <param name="score">New score, must be non-negative</param>
        /// <param name="inlineMessageId">Identifier of the inline message</param>
        /// <param name="force">Pass True, if the high score is allowed to decrease. This can be useful when fixing mistakes or banning cheaters</param>
        /// <param name="disableEditMessage">Pass True, if the game message should not be automatically edited to include the current scoreboard</param>
        /// <returns>True on success</returns>
        public async Task<bool> SetGameScoreAsync(int userId, int score, string inlineMessageId, bool force = false,
            bool disableEditMessage = false)
        {
            Dictionary<string, object> args = new Dictionary<string, object>() { { "user_id", userId }, { "score", score },
                { "inline_message_id", inlineMessageId } };
            if (force) args.Add("force", true);
            if (disableEditMessage) args.Add("disable_edit_message", true);

            return await ApiMethodAsync<bool>("setGameScore", args);
        }

        /// <summary>
        /// Use this method to get data for high score tables. 
        /// Will return the score of the specified user and several of his neighbors in a game. 
        /// On success, returns an Array of GameHighScore objects.
        /// </summary>
        /// <param name="userId">Target user id</param>
        /// <param name="chatId">Unique identifier for the target chat</param>
        /// <param name="messageId">Identifier of the sent message</param>
        /// <returns>An array of GameHighScore</returns>
        public GameHighScore[] GetGameHighScores(int userId, long chatId, int messageId)
            => GetGameHighScoresAsync(userId, chatId, messageId).Result;
        /// <summary>
        /// Use this method to get data for high score tables. 
        /// Will return the score of the specified user and several of his neighbors in a game. 
        /// On success, returns an Array of GameHighScore objects.
        /// </summary>
        /// <param name="userId">Target user id</param>
        /// <param name="chatId">Unique identifier for the target chat</param>
        /// <param name="messageId">Identifier of the sent message</param>
        /// <returns>An array of GameHighScore</returns>
        public async Task<GameHighScore[]> GetGameHighScoresAsync(int userId, long chatId, int messageId)
        {
            Dictionary<string, object> args = new Dictionary<string, object>()
            {
                { "user_id", userId }, { "chat_id", chatId }, { "message_id", messageId }
            };
            return await ApiMethodAsync<GameHighScore[]>("getGameHighScores", args);
        }

        /// <summary>
        /// Use this method to get data for high score tables. 
        /// Will return the score of the specified user and several of his neighbors in a game. 
        /// On success, returns an Array of GameHighScore objects.
        /// </summary>
        /// <param name="userId">Target user id</param>
        /// <param name="inlineMessageId">Identifier of the inline message</param>
        /// <returns>An array of GameHighScore</returns>
        public GameHighScore[] GetGameHighScores(int userId, string inlineMessageId)
            => GetGameHighScoresAsync(userId, inlineMessageId).Result;
        /// <summary>
        /// Use this method to get data for high score tables. 
        /// Will return the score of the specified user and several of his neighbors in a game. 
        /// On success, returns an Array of GameHighScore objects.
        /// </summary>
        /// <param name="userId">Target user id</param>
        /// <param name="inlineMessageId">Identifier of the inline message</param>
        /// <returns>An array of GameHighScore</returns>
        public async Task<GameHighScore[]> GetGameHighScoresAsync(int userId, string inlineMessageId)
        {
            Dictionary<string, object> args = new Dictionary<string, object>()
            {
                { "user_id", userId }, { "inline_message_id", inlineMessageId }
            };
            return await ApiMethodAsync<GameHighScore[]>("getGameHighScores", args);
        }
        #endregion
        #region Telegram Passport
        /// <summary>
        /// <para>Informs a user that some of the Telegram Passport elements they provided contains errors. 
        /// The user will not be able to re-submit their Passport to you until the errors are fixed 
        /// (the contents of the field for which you returned the error must change). Returns True on success.</para>
        /// 
        /// <para>Use this if the data submitted by the user doesn't satisfy the standards your service requires for any reason. 
        /// For example, if a birthday date seems invalid, a submitted document is blurry, a scan shows evidence of tampering, etc. 
        /// Supply some details in the error message to make sure the user knows how to correct the issues.</para>
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <param name="errors">A JSON-serialized array describing the errors</param>
        /// <returns>True on success</returns>
        public bool SetPassportDataErrors(int userId, PassportElementError[] errors)
            => SetPassportDataErrorsAsync(userId, errors).Result;
        /// <summary>
        /// <para>Informs a user that some of the Telegram Passport elements they provided contains errors. 
        /// The user will not be able to re-submit their Passport to you until the errors are fixed 
        /// (the contents of the field for which you returned the error must change). Returns True on success.</para>
        /// 
        /// <para>Use this if the data submitted by the user doesn't satisfy the standards your service requires for any reason. 
        /// For example, if a birthday date seems invalid, a submitted document is blurry, a scan shows evidence of tampering, etc. 
        /// Supply some details in the error message to make sure the user knows how to correct the issues.</para>
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <param name="errors">A JSON-serialized array describing the errors</param>
        /// <returns>True on success</returns>
        public async Task<bool> SetPassportDataErrorsAsync(int userId, PassportElementError[] errors)
        {
            Dictionary<string, object> args = new Dictionary<string, object>()
            {
                { "user_id", userId }, { "errors", errors }
            };
            return await ApiMethodAsync<bool>("setPassportDataErrors", args);
        }
        #endregion
    }
}
