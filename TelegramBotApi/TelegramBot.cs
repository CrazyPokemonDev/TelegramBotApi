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
                    if (kvp.Value is SendFileAttach sfa)
                    {
                        return await ApiMethodMultipartAsync<T>(method, args, SendFileAttach.AttachName, sfa, timeout);
                    }
                    url += $"{kvp.Key}={Serialize(kvp.Value)}&";
                }
            HttpWebRequest request = WebRequest.CreateHttp(url);
            if (timeout != 0) request.Timeout = timeout;
            WebResponse response = await request.GetResponseAsync();
            return DeserializeResponse<T>(response);
        }

        private async Task<T> ApiMethodMultipartAsync<T>(string method, Dictionary<string, object> args,
            string multipartObjectKey, ISendFileMultipart multipartFile, int timeout = 100)
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

        private string Serialize(object obj)
        {
            switch (obj)
            {
                case SendFileId sfi:
                    return WebUtility.HtmlEncode(sfi.FileId);
                case SendFileUrl sfu:
                    return WebUtility.HtmlEncode(sfu.Url);
                case SendFileAttach sfa:
                    return WebUtility.HtmlEncode("attach://" + SendFileAttach.AttachName);
                case string s:
                    return WebUtility.HtmlEncode(s);
            }
            return WebUtility.HtmlEncode(JsonConvert.SerializeObject(obj));
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

            return await ApiMethodAsync<Update[]>("getUpdates", args, timeout);
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
        /// <summary>
        /// A simple method for testing your bot's auth token. Requires no parameters. 
        /// Returns basic information about the bot in form of a User object.
        /// </summary>
        /// <returns>Returns basic information about the bot in form of a User object.</returns>
        public async Task<User> GetMeAsync()
        {
            return await ApiMethodAsync<User>("getMe");
        }

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
        #endregion
    }
}
