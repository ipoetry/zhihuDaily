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
        /// 第四版API
        /// </summary>
        //public string rootUrl= "http://news-at.zhihu.com/api/4";
        /// <summary>
        /// 获取启动图像，需要传入分辨率参数【320*432，480*728，720*1184，1080*1776】
        /// </summary>
        public static readonly string startImgUrl = "http://news-at.zhihu.com/api/4/start-image/{0}";
        /// <summary>
        /// 获取最新消息
        /// </summary>
        public static readonly string laestNewsUrl = "http://news-at.zhihu.com/api/4/news/latest";
        /// <summary>
        /// 获取消息内容和离线下载，传入消息ID
        /// </summary>
        public static readonly string newsContentUrl = "http://news-at.zhihu.com/api/4/news/{0}";
        /// <summary>
        /// 获取历史消息，需要传入时间参数【20150816】
        /// </summary>
        public static readonly string newsBeforeUrl = "http://news.at.zhihu.com/api/4/news/before/{0}";
        /// <summary>
        /// 获取新闻附加信息(赞数量，评论数量等)，传入消息ID
        /// </summary>
        public static readonly string storyExtraUrl = "http://news-at.zhihu.com/api/4/story-extra/{0}";
        /// <summary>
        /// 获取长评论，传入消息ID
        /// </summary>
        public static readonly string longCommentsUrl = "http://news-at.zhihu.com/api/4/story/{0}/long-comments";
        /// <summary>
        /// 获取长评论，传入消息ID
        /// </summary>
        public static readonly string shortCommentsUrl = "http://news-at.zhihu.com/api/4/story/{0}/short-comments";
        /// <summary>
        /// 获取所有主题
        /// </summary>
        public static readonly string themesUrl = "http://news-at.zhihu.com/api/4/themes";
        /// <summary>
        /// 获取主题内容，传入主题ID
        /// </summary>
        public static readonly string themeContentUrl = "http://news-at.zhihu.com/api/4/theme/{0}";

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
