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
using TelegramBotApi.Types.Markup;
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
                Update[] updates = await GetUpdates(allowedUpdates, LongPollingTimeout, _lastUpdateReceived + 1);
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
            Update[] updates = GetUpdates(timeout: 0, offset: -1).Result;
            if (updates.Length > 0) GetUpdates(timeout: 0, offset: updates[0].Id + 1).Wait();
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
        /// <returns></returns>
        public async Task<Update[]> GetUpdates(UpdateType[] allowedUpdates = null, int timeout = 0, int offset = 0, int limit = 100)
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
        public async Task<bool> DeleteWebhookAsync()
        {
            return await ApiMethodAsync<bool>("deleteWebhook");
        }

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
        public async Task<Message> SendTextMessage(ChatId chatId, string text, ParseMode parseMode = ParseMode.None,
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
        /// <param name="disableNotification">Sends the message silently. Users will receive a notification with no sound.</param>
        /// <param name="replyToMessageId">The messageId of the message to reply to, if any</param>
        /// <param name="replyMarkup">The reply markup. Additional interface options.</param>
        /// <returns>The sent message on success</returns>
        public async Task<Message> SendAudioAsync(ChatId chatId, SendFile audio, string caption = null,
            ParseMode parseMode = ParseMode.None, int duration = 0, string performer = null, string title = null,
            bool disableNotification = false, int replyToMessageId = -1, ReplyMarkupBase replyMarkup = null)
        {
            Dictionary<string, object> args = new Dictionary<string, object>() { { "chat_id", chatId }, { "audio", audio } };
            if (!string.IsNullOrWhiteSpace(caption)) args.Add("caption", caption);
            if (parseMode != ParseMode.None) args.Add("parse_mode", Enum.GetString(parseMode));
            if (duration != 0) args.Add("duration", duration);
            if (!string.IsNullOrWhiteSpace(performer)) args.Add("performer", performer);
            if (!string.IsNullOrWhiteSpace(title)) args.Add("title", title);
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
        /// <param name="caption">The caption of the document, if any</param>
        /// <param name="parseMode">The parse mode of the message, if any</param>
        /// <param name="disableNotification">If this is true, the user will receive a notification without sound</param>
        /// <param name="replyToMessageId">The message id of the message to reply to in the target chat, if any</param>
        /// <param name="replyMarkup">The reply markup. Additional interface options.</param>
        /// <returns>The sent message on success</returns>
        public async Task<Message> SendDocumentAsync(ChatId chatId, SendFile document, string caption = null, 
            ParseMode parseMode = ParseMode.None, bool disableNotification = false, int replyToMessageId = -1,
            ReplyMarkupBase replyMarkup = null)
        {
            Dictionary<string, object> args = new Dictionary<string, object>() { { "chat_id", chatId }, { "document", document } };
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
        /// <param name="caption">Optional. Caption for the video message</param>
        /// <param name="parseMode">Optional. Parse mode for the message caption</param>
        /// <param name="supportsStreaming">Pass true if the uploaded video is suitable for streaming</param>
        /// <param name="disableNotification">If this is true, users will receive a silent notification</param>
        /// <param name="replyToMessageId">The message id of the message to reply to, if any</param>
        /// <param name="replyMarkup">The reply markup. Additional interface options.</param>
        /// <returns>The sent message on success</returns>
        public async Task<Message> SendVideoAsync(ChatId chatId, SendFile video, int duration = 0, int width = 0, int height = 0,
            string caption = null, ParseMode parseMode = ParseMode.None, bool supportsStreaming = false,
            bool disableNotification = false, int replyToMessageId = -1, ReplyMarkupBase replyMarkup = null)
        {
            Dictionary<string, object> args = new Dictionary<string, object>() { { "chat_id", chatId }, { "video", video } };
            if (duration != 0) args.Add("duration", duration);
            if (width != 0) args.Add("width", width);
            if (height != 0) args.Add("height", height);
            if (!string.IsNullOrWhiteSpace(caption)) args.Add("caption", caption);
            if (parseMode != ParseMode.None) args.Add("parse_mode", Enum.GetString(parseMode));
            if (supportsStreaming) args.Add("supports_streaming", true);
            if (disableNotification) args.Add("disable_notification", true);
            if (replyToMessageId != -1) args.Add("reply_to_message_id", replyToMessageId);
            if (replyMarkup != null) args.Add("reply_markup", replyMarkup);

            return await ApiMethodAsync<Message>("sendVideo", args);
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
        /// <param name="disableNotification">If this is true, users will receive a silent notification</param>
        /// <param name="replyToMessageId">The message id of the message to reply to, if any</param>
        /// <param name="replyMarkup">The reply markup. Additional interface options.</param>
        /// <returns>The sent message on success</returns>
        public async Task<Message> SendVideoNoteAsync(ChatId chatId, SendFile videoNote, int duration = 0, int length = 0,
            bool disableNotification = false, int replyToMessageId = -1, ReplyMarkupBase replyMarkup = null)
        {
            Dictionary<string, object> args = new Dictionary<string, object>() { { "chat_id", chatId }, { "video_note", videoNote } };
            if (duration != 0) args.Add("duration", duration);
            if (length != 0) args.Add("length", length);
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
        /// <returns>The sent message on success</returns>
        public async Task<Message> SendMediaGroupAsync(ChatId chatId, InputMedia[] media, bool disableNotification = false,
            int replyToMessageId = -1)
        {
            Dictionary<string, object> args = new Dictionary<string, object>() { { "chat_id", chatId }, { "media", media } };
            if (disableNotification) args.Add("disable_notification", true);
            if (replyToMessageId != -1) args.Add("reply_to_message_id", replyToMessageId);

            return await ApiMethodAsync<Message>("sendMediaGroup", args);
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
        /// <param name="disableNotification">If this is true, users will receive a silent notification</param>
        /// <param name="replyToMessageId">The message id of the message to reply to, if any</param>
        /// <param name="replyMarkup">The reply markup. Additional interface options.</param>
        /// <returns>The sent message on success</returns>
        public async Task<Message> SendVenueAsync(ChatId chatId, double latitude, double longitude, string title,
            string address, string foursquareId = null, bool disableNotification = false, int replyToMessageId = -1,
            ReplyMarkupBase replyMarkup = null)
        {
            Dictionary<string, object> args = new Dictionary<string, object>()
            {
                { "chat_id", chatId }, { "latitude", latitude },
                { "longitude", longitude }, { "title", title }, { "address", address }
            };
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
        /// <param name="disableNotification">If this is true, users will receive a silent notification</param>
        /// <param name="replyToMessageId">The message id of the message to reply to, if any</param>
        /// <param name="replyMarkup">The reply markup. Additional interface options.</param>
        /// <returns>The sent message on success</returns>
        public async Task<Message> SendContactAsync(ChatId chatId, string phoneNumber, string firstName, string lastName = null,
            bool disableNotification = false, int replyToMessageId = -1, ReplyMarkupBase replyMarkup = null)
        {
            Dictionary<string, object> args = new Dictionary<string, object>() { { "chat_id", chatId }, { "phoneNumber", phoneNumber },
                { "first_name", firstName }};
            if (!string.IsNullOrWhiteSpace(lastName)) args.Add("last_name", lastName);
            if (disableNotification) args.Add("disable_notification", true);
            if (replyToMessageId != -1) args.Add("reply_to_message_id", replyToMessageId);
            if (replyMarkup != null) args.Add("reply_markup", replyMarkup);

            return await ApiMethodAsync<Message>("sendContact", args);
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
        /// <returns></returns>
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
        /// <returns></returns>
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
        /// <returns></returns>
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
        public async Task<int> GetChatMembersCount(ChatId chatId)
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
        public async Task<bool> DeleteChatStickerSetAsync(ChatId chatId)
        {
            Dictionary<string, object> args = new Dictionary<string, object>() { { "chat_id", chatId } };
            return await ApiMethodAsync<bool>("deleteChatStickerSet", args);
        }
        #endregion
        #endregion
    }
}
