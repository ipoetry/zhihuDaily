using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using zhihuDaily.Model;

namespace zhihuDaily.DataService
{
    class OfflineNewsDownloader
    {

        const int offlinedays = 2;
        public delegate void OffilelinePrcoess(double process);
        public event OffilelinePrcoess OfflineProcessHandler;
        private bool isDownloading = false;
        HttpClient httpClient ;
        StorageFolder cacheFolder;
        List<Story> Storys = new List<Story>();

        public OfflineNewsDownloader()
        {
            httpClient = new HttpClient(new ServiceBase.MyHttpClientHandler());
        }


        public async void BeginDownloadAsync()
        {
            if (NetWorkHelper.IsConnectedToInternet) //network enabled
            {                           
                if (!isDownloading) //如果不在下载状态，则执行
                {
                    ICommonService<LatestNews> _latestNewsService = new CommonService<LatestNews>();
                    var latest = await _latestNewsService.GetObjectAsync2("stories", "latest");//today
                    if (latest.Stories != null)
                    {
                        Storys.AddRange(latest.Stories);
                    }
                    for (int i = -1; i > -offlinedays; i--)
                    {
                        string date = DateTime.Now.AddDays(-i).ToString("yyyyMMdd");
                        var result = await _latestNewsService.GetObjectAsync("stories", "before", date);
                        if (result.Stories != null)
                        {
                            Storys.AddRange(result.Stories);
                        }
                    }
                }

                int count = Storys.Count;
                for (int i = 1; i <= count; i++)
                {
                    if (!isDownloading)
                    {
                        var newsContent = await new CommonService<NewsContent>().GetObjectAsync("story", Storys[i-1].Id.ToString());
                        if (!string.IsNullOrEmpty(newsContent.Body))
                        {
                            IList<string> urlList = Functions.GetHtmlImageUrlList(newsContent.Body);
                            if (!string.IsNullOrEmpty(newsContent.Image))
                                urlList.Add(newsContent.Image);
                            await DownloadAsync(urlList);
                        }
                        if (OfflineProcessHandler != null)
                        {
                            OfflineProcessHandler(i / (double)count);
                        }
                    }
                }
            }
            else
            {
                ToastPrompt.ShowToast("没有网络");
                this.EndDownload();
            }
        }

        public void EndDownload()
        {
            isDownloading = true;
            if (OfflineProcessHandler != null)
            {
                double process = -1d;
                OfflineProcessHandler(process);
            }
        }

        public async Task SaveImage(string url, IStorageFile outputFile)
        {
            try
            {
                var response = new HttpResponseMessage();
                response = await httpClient.GetAsync(url);
                var responseStream = await response.Content.ReadAsStreamAsync();

                using (var fs = await outputFile.OpenStreamForWriteAsync())
                {
                    await responseStream.CopyToAsync(fs);
                }
                response.Dispose();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        public async Task<bool> DownloadAsync(IList<string> urls)
        {
            cacheFolder = await ApplicationData.Current.LocalFolder.GetFolderAsync("Cache");
            foreach (var url in urls)
            {
                string fileName = EncryptUtils.GetMd5String(url);
                //如果文件不存在或者为空文件，则开始下载
                if (!await StorageHelper.Instance.StorageItemExistsAsync("Cache/"+fileName))
                {                    
                    System.Diagnostics.Debug.WriteLine("Begin Cache Pic");
                    var storageFile = await cacheFolder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);
                    await SaveImage(url, storageFile);
                }
            }
            return true;
        }

    }
}
