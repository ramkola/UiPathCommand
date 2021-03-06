﻿using KUiPath.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace KUiPath.Manager
{
    public static class HttpClientManager
    {
        private static HttpClient Client;

        /// <summary>
        /// 認証ありのWeb APIを利用する時に利用するパラメータです。
        /// </summary>
        public static string BearerValue { get; set; } = string.Empty;

        #region HttpClientManager

        /// <summary>
        /// Create HttpClientManager Instance.
        /// Set parameter for Authentication proxy
        /// </summary>
        static HttpClientManager()
        {

            try
            {
                var reader = new AppSettingsReader();
                if ((bool)reader.GetValue("UseProxy", typeof(bool)))
                {
                    HttpClientHandler handler = new HttpClientHandler();
                    handler.UseProxy = true;
                    handler.Proxy = new WebProxy(reader.GetValue("ProxyUrl", typeof(string)).ToString());
                    handler.Credentials = new NetworkCredential(
                        reader.GetValue("ProxyAccount", typeof(string)).ToString(),
                        reader.GetValue("ProxyPassword", typeof(string)).ToString());
                    HttpClientManager.Client = new HttpClient(handler);
                }
                else
                {
                    HttpClientManager.Client = new HttpClient();
                }
            }
            catch (Exception)
            { }
        }

        #endregion


        public static async Task<T> ExecutePostAsync<T>(string url, T contents) where T : BaseCommandModel
        {
            //各種設定を行います。
            HttpClientManager.InitializeClient();

            //指定されたContentsを指定されたURLにPOSTします。
            var response = await HttpClientManager.Client.PostAsJsonAsync<T>(url, contents);

            //レスポンスのContentsをJson形式から指定されたT型のObjectのインスタンスに変換します。
            string responseContents = await response.Content.ReadAsStringAsync();

            try
            {
                contents = JsonConvert.DeserializeObject<T>(responseContents);
            }
            catch (JsonException)
            {
                contents.ResponceContent = responseContents;
            }
            return contents;
        }

        public static async Task<T> ExecuteGetAsync<T>(string url)
        {
            //各種設定を行います。
            HttpClientManager.InitializeClient();

            //指定されたContentsを指定されたURLにPOSTします。
            var response = await HttpClientManager.Client.GetStringAsync(url);

            //レスポンスのContentsをJson形式から指定されたT型のObjectのインスタンスに変換します。
            return JsonConvert.DeserializeObject<T>(response);
        }

        #region private

        private static void InitializeClient()
        {
            HttpClientManager.Client.DefaultRequestHeaders.Accept.Clear();
            HttpClientManager.Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpClientManager.Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpClientManager.BearerValue);
        }
        #endregion

    }
}
