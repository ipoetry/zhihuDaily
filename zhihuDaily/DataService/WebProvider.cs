using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using zhihuDaily.DataService;

namespace zhihuDaily
{
    public class WebProvider
    {
        #region Private Field
        private static readonly WebProvider instance = new WebProvider();
        private static HttpClient httpClient;
        private string IMEI;
        #endregion

        #region Constructor
        private WebProvider()
        {
            Initialize();
        }
        #endregion

        #region Public Function

        #region Singleton Pattern
        public static WebProvider GetInstance()
        {
            if (NetWorkHelper.IsConnectedToInternet)
            {
                var user = ViewModel.ViewModelLocator.AppShell.UserInfo;
                if (user != null && !string.IsNullOrEmpty(user.AccessToken))
                {

                    if (httpClient.DefaultRequestHeaders.ContainsKey("Authorization"))
                    {
                        httpClient.DefaultRequestHeaders.Remove("Authorization");
                    }
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {user.AccessToken}");
                }
                return instance;
            }
            else
            {
                throw new System.Net.Http.HttpRequestException("Seems unable to connect to the network.");
            }
        }

        #endregion

        /// <summary>
        /// Initialize
        /// Initial the IMEI and HttpClient
        /// </summary>
        public void Initialize()
        {
            if (string.IsNullOrEmpty(IMEI))
            {
                Random random = new Random(DateTime.Now.Millisecond);
                for (int i = 0; i < 15; i++)
                {
                    IMEI += random.Next(10);
                }

                httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("User-Agent", "ZhihuApi/1.0.0-beta (Linux; Android 4.2.2; GT-P5210 Build/samsung/GT-P5210/GT-P5210/JDQ39E/zh_CN) Google-HTTP-Java-Client/1.20.0 (gzip) Google-HTTP-Java-Client/1.20.0 (gzip)");
                httpClient.DefaultRequestHeaders.Add("x-api-version", "4");
                httpClient.DefaultRequestHeaders.Add("x-app-version", "2.5.4");
                httpClient.DefaultRequestHeaders.Add("x-os", "Android 4.2.2");
                httpClient.DefaultRequestHeaders.Add("x-device", "GT-P5210");
                var easId = new Windows.Security.ExchangeActiveSyncProvisioning.EasClientDeviceInformation().Id;
                httpClient.DefaultRequestHeaders.Add("x-uuid", easId.ToString());
                
            }
        }

        public async Task<string> GetRequestDataAsync(string url)
        {
            var response = new HttpResponseMessage();
            try
            {
                response = await httpClient.GetAsync(new Uri(url));
                string responseText = await response.Content.ReadAsStringAsync();
                response.EnsureSuccessStatusCode();
                System.Diagnostics.Debug.WriteLine("请求url[get]:" + url + " 结果：" + responseText);
                return responseText;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        public class ContentType{
            /// <summary>
            /// application/x-www-form-urlencoded
            /// </summary>
            public static string ContentType1 { get { return "application/x-www-form-urlencoded"; } }
            /// <summary>
            /// application/x-www-form-urlencoded; charset=UTF-8
            /// </summary>
            public static string ContentType2 { get { return "application/x-www-form-urlencoded; charset=UTF-8"; } }
            /// <summary>
            /// application/json; charset=UTF-8
            /// </summary>
            public static string ContentType3 { get { return "application/json; charset=UTF-8"; } }
        }
        /// <summary>
        /// 向服务器发送post请求 返回服务器回复数据(string)
        /// </summary>
        /// <param name="url">请求url</param>
        /// <param name="body">文本数据</param>
        /// <param name="contentType">请求内容类型</param>
        /// <returns></returns>
        public async Task<string> SendPostRequestAsync(string url, string body,string contentType)
        {
            try
            {
                HttpRequestMessage mSent = new HttpRequestMessage(HttpMethod.Post, new Uri(url));
                mSent.Content = new HttpStringContent(body, UnicodeEncoding.Utf8, contentType);
                HttpResponseMessage response = await httpClient.SendRequestAsync(mSent);
                response.EnsureSuccessStatusCode();
                
                string result = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine("请求url[post]:" + url+" 结果："+result);
                return result;
            }
            catch
            {
                return string.Empty;
            }
        }

        public async Task<string> SendDeleteRequestAsync(string url)
        {
            try
            {
                HttpRequestMessage mSent = new HttpRequestMessage(HttpMethod.Delete, new Uri(url));
                //mSent.Content = new HttpStringContent(body, UnicodeEncoding.Utf8, contentType);
                HttpResponseMessage response = await httpClient.SendRequestAsync(mSent);
                response.EnsureSuccessStatusCode();

                string result = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine("请求url[post]:" + url + " 结果：" + result);
                return result;
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 向服务器发送post请求 返回服务器回复数据(string)
        /// </summary>
        /// <param name="url">请求url</param>
        /// <param name="stream">文本数据</param>
        /// <returns></returns>
        public async Task<string> SendPostRequestAsync(string url, IInputStream stream)
        {
            try
            {
                HttpRequestMessage mSent = new HttpRequestMessage(HttpMethod.Post, new Uri(url));
                mSent.Headers.Add("Content-Type", "application/octet-stream");
                mSent.Headers.Add("Transfer-Encoding", "chunked");

                mSent.Content = new HttpStreamContent(stream);
                HttpResponseMessage response = await httpClient.SendRequestAsync(mSent);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch
            {
                return string.Empty;
            }
        }

        #endregion

    }
}
