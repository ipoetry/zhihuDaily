using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
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
                return instance;
            }
            else
            {
                throw new HttpRequestException("Seems unable to connect to the network.");
            }
        }

        public static void CancelPendingRequests()
        {
            httpClient.CancelPendingRequests();
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

                httpClient = new HttpClient(new ServiceBase.MyHttpClientHandler());
            }
        }

        public async Task<string> GetRequestDataAsync(string url)
        {
            System.Diagnostics.Debug.WriteLine("Access Url：" + url);
            var response = new HttpResponseMessage();
            try
            {
                response = await httpClient.GetAsync(url);
                string responseText = await response.Content.ReadAsStringAsync();
                response.EnsureSuccessStatusCode();

                return responseText;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        #endregion

    }
}
