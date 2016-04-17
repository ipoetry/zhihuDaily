using System;

using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace zhihuDaily.DataService
{
    public class ServiceBase
    {
        private HttpClient httpClient;

        /// <summary>
        /// 通用获取接口泛型方法
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="url">接口url</param>
        /// <returns></returns>
        protected static async Task<T> GetDataAsync<T>(string url,string para="")
        {
            var httpClientHandler = new RetryHandler(new MyHttpClientHandler());
            var httpClient = new HttpClient(httpClientHandler);

            var ContentUri = string.IsNullOrEmpty(para) ? new Uri(url) : new Uri(string.Format(url,para));

            var response = new HttpResponseMessage();
            try
            {
                response = await httpClient.GetAsync(ContentUri);
                response.EnsureSuccessStatusCode();
                string responseText = await response.Content.ReadAsStringAsync();
                return JsonConvertHelper.JsonDeserialize<T>(responseText);

            }
            catch (Exception)
            {
                return default(T);
            }
        }

        protected async Task<string> GetJsonDataAsync(string url)
        {
            //httpClient = new HttpClient(new MyHttpClientHandler());
            //System.Diagnostics.Debug.WriteLine("Access Url：" + url);
            //var response = new HttpResponseMessage();
            //try
            //{
            //    response = await httpClient.GetAsync(url);
            //    string responseText = await response.Content.ReadAsStringAsync();
            //    response.EnsureSuccessStatusCode();

            //    return responseText;
            //}
            //catch (Exception)
            //{
            //    return string.Empty;
            //}
             return await WebProvider.GetInstance().GetRequestDataAsync(url);
        }

        public class MyHttpClientHandler : HttpClientHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows Phone 10.0;  Android 4.2.1; Nokia; Lumia 520) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/42.0.2311.135 Mobile Safari/537.36 Edge/12.10130");
                return base.SendAsync(request, cancellationToken);
            }
        }
        /// <summary>
        /// 扩展请求失败重试
        /// </summary>
        public class RetryHandler : DelegatingHandler
        {
            private const int MaxRetries = 3;

            public RetryHandler(HttpMessageHandler innerHandler) : base(innerHandler) { }
            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                HttpResponseMessage response;
                {
                    for (int i = 0; i < MaxRetries-1; i++)
                    {
                        response = await base.SendAsync(request, cancellationToken);
                        if (response.IsSuccessStatusCode)
                            return response;
                    }
                    response = await base.SendAsync(request, cancellationToken);
                    return response;
                }
            }
        }

    }
}
