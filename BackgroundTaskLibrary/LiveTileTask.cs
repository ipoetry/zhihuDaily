using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.UI.Notifications;

namespace BackgroundTaskLibrary
{
    public sealed class LiveTileTask : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            var deferral = taskInstance.GetDeferral();

            await GetLatestNews();

            deferral.Complete();
        }

        public static IAsyncOperation<string> GetLatestNews()
        {
            try
            {
                return AsyncInfo.Run(token => GetNews());
            }
            catch (Exception)
            {
                // ignored
            }
            return null;
        }

        private static async Task<string> GetNews()
        {
            try
            {
                //先判断网络状态
                string rootUrl = "http://news-at.zhihu.com/api/4/news/latest";
                var httpClient = new HttpClient();
                var response = new HttpResponseMessage();
                response = await httpClient.GetAsync(rootUrl);
                string responseText = await response.Content.ReadAsStringAsync();
                response.EnsureSuccessStatusCode();
                var result = string.IsNullOrEmpty(responseText) ? null : JsonConvertHelper.JsonDeserialize(responseText)?.Stories;

                if (result != null)
                {
                    var news = result.Take(3).ToList();
                    UpdatePrimaryTile(news);
                }

            }
            catch (Exception)
            {
                // ignored
            }
            return null;
        }
        
        static string TileTemplateXml = "<tile><visual><binding template=\"TileMedium\"><text hint-wrap=\"true\" >{0}</text><image src=\"{1}\" placement=\"background\" /></binding><binding template=\"TileWide\" ><text hint-wrap=\"true\">{0}</text><image src=\"{1}\" placement=\"background\" /></binding></visual></tile>";
        private static void UpdatePrimaryTile(IEnumerable<Story> news)
        {
            try
            {
                var updater = TileUpdateManager.CreateTileUpdaterForApplication();
                updater.EnableNotificationQueueForWide310x150(true);
                updater.EnableNotificationQueue(true);
                updater.Clear();

                foreach (var n in news)
                {
                    var doc = new XmlDocument();
                    var xml = string.Format(TileTemplateXml, n.Title, n.Images[0]);
                    doc.LoadXml(WebUtility.HtmlDecode(xml), new XmlLoadSettings
                    {
                        ProhibitDtd = false,
                        ValidateOnParse = false,
                        ElementContentWhiteSpace = false,
                        ResolveExternals = false
                    });

                    updater.Update(new TileNotification(doc));
                }
            }
            catch (Exception )
            {
                // ignored
            }
        }

    }

}
