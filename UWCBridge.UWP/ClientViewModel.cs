using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using UWPBridge.Shared;

namespace UWCBridge.UWP
{
    public class ClientViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string LoginLabel => "ログイン名";
        public string PasswordLabel => "パスワード";
        public string TokenLabel => "JWTトークン";
        public string ResponseHeaderLabel => "HTTP HEADER";
        public string ResponseStatusLabel => "HTTP STATUS CODE";
        public string ResponseBodyLabel => "Body";
        public string ButtonLoginLabel => "ログイン";
        public string ButtonUsersLabel => "ユーザ一覧";

        /// <summary>
        /// ログイン名
        /// </summary>
        public string Login { get; set; }
        /// <summary>
        /// パスワード
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// JWTトークン
        /// </summary>
        private string token;
        /// <summary>
        /// JWTトークン
        /// </summary>
        public string Token
        {
            get { return token; }
            set { token = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Token))); }
        }

        /// <summary>
        /// レスポンスヘッダ
        /// </summary>
        private string responseHeader;
        /// <summary>
        /// レスポンスヘッダ
        /// </summary>
        public string ResponseHeader
        {
            get { return responseHeader; }
            set { responseHeader = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ResponseHeader))); }
        }

        /// <summary>
        /// レスポンスステータス
        /// </summary>
        private string responseStatus;
        /// <summary>
        /// レスポンスステータス
        /// </summary>
        public string ResponseStatus
        {
            get { return responseStatus; }
            set { responseStatus = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ResponseStatus))); }
        }

        /// <summary>
        /// レスポンスボディ
        /// </summary>
        private string responseBody;
        /// <summary>
        /// レスポンスボディ
        /// </summary>
        public string ResponseBody
        {
            get { return responseBody; }
            set { responseBody = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ResponseBody))); }
        }
        /// <summary>
        /// ログイン
        /// </summary>
        public async void CommandLoginAsync()
        {
            ClearResult();

            try
            {
                var loginModel = new LoginModel { Username = Login, Password = Password };
                var json = JsonConvert.SerializeObject(loginModel);
                Debug.WriteLine(json);

                using (var client = new HttpClient())
                using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
                {
                    var httpResponseMessage = await client.PostAsync(API_LOGIN, content);
                    ResponseHeader = GetHeaderString(httpResponseMessage.Headers);
                    ResponseStatus = httpResponseMessage.StatusCode.ToString();
                    ResponseBody = await httpResponseMessage.Content.ReadAsStringAsync();

                    if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
                    {
                        var tokenModel = JsonConvert.DeserializeObject<TokenModel>(ResponseBody);
                        Token = tokenModel.token;
                    }
                }
            }
            catch (Exception ex)
            {
                ResponseBody = ex.ToString();
            }
        }
        /// <summary>
        /// ユーザ一覧
        /// </summary>
        public async void CommandUsersAsync()
        {
            ClearResult();

            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrWhiteSpace(Token))
                    {
                        client.DefaultRequestHeaders.Add(KEY_AUTH, "Bearer " + Token);
                    }
                    var httpResponseMessage = await client.GetAsync(API_USERS);
                    ResponseHeader = GetHeaderString(httpResponseMessage.Headers);
                    ResponseStatus = httpResponseMessage.StatusCode.ToString();
                    ResponseBody = await httpResponseMessage.Content.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {
                ResponseBody = ex.ToString();
            }
        }
        /// <summary>
        /// ヘッダを文字列にする
        /// </summary>
        /// <param name="headers">HTTP HEADER</param>
        /// <returns>文字列</returns>
        private string GetHeaderString(HttpResponseHeaders headers)
        {
            string result = "";
            foreach (var item in headers)
            {
                result += item.Key;
                foreach (var value in item.Value)
                {
                    result += " " + value;
                }
                result += "\n";
            }
            return result;
        }

        private const string URL_BASE = @"http://localhost:5001";
        private const string API_LOGIN = URL_BASE + @"/api/token";
        private const string API_USERS = URL_BASE + @"/api/users";
        private const string HEADER_TOKEN = "token";
        private const string KEY_AUTH = "Authorization";

        /// <summary>
        /// レスポンス表示を消す
        /// </summary>
        private void ClearResult()
        {
            ResponseStatus = null;
            ResponseBody = null;
        }
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ClientViewModel()
        {
            Login = "admin";
            Password = "admin";
        }
    }
}
