using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TelegramBotApi.Types;
using TelegramBotApi.Types.Exceptions;
using TelegramBotApi.Types.Upload;

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
        /// <returns>The response object</returns>
        public async Task<T> ApiMethodAsync<T>(string method, Dictionary<string, object> args)
        {
            string url = ApiUrl + method + "?";
            foreach (var kvp in args)
            {
                if (kvp.Value is SendFileMultipart sfm)
                {
                    args.Remove(kvp.Key);
                    return await ApiMethodMultipartAsync<T>(method, args, kvp.Key, sfm);
                }
                if (kvp.Value is SendFileAttach sfa)
                {
                    return await ApiMethodMultipartAsync<T>(method, args, SendFileAttach.AttachName, sfa);
                }
                url += $"{kvp.Key}={Serialize(kvp.Value)}&";
            }
            HttpWebRequest request = WebRequest.CreateHttp(url);
            WebResponse response = await request.GetResponseAsync();
            return DeserializeResponse<T>(response);
        }

        private async Task<T> ApiMethodMultipartAsync<T>(string method, Dictionary<string, object> args, 
            string multipartObjectKey, ISendFileMultipart multipartFile)
        {
            HttpClient httpClient = new HttpClient();
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
                    return sfi.FileId;
                case SendFileUrl sfu:
                    return sfu.Url;
                case SendFileAttach sfa:
                    return "attach://" + SendFileAttach.AttachName;
                case string s:
                    return s;
            }
            return JsonConvert.SerializeObject(obj);
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
    }
}
