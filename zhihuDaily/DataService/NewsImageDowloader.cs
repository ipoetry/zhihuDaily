using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Storage;

namespace zhihuDaily.DataService
{
    public delegate void ImgLoadedProcessHandler(string value, IStorageFile file);
    class NewsImageDowloader
    {
        public NewsImageDowloader()
        {
            httpClient = new HttpClient(new ServiceBase.MyHttpClientHandler());
        }

        HttpClient httpClient;
        StorageFolder cacheFolder;

        public event ImgLoadedProcessHandler ImgLoadedProcess;

        public async Task SaveImage(string url,IStorageFile outputFile)
        {
            try
            {
                var response = new HttpResponseMessage();
                response = await httpClient.GetAsync(url);
                using (var responseStream = await response.Content.ReadAsStreamAsync())
                {
                    using (var fs = await outputFile.OpenStreamForWriteAsync())
                    {
                        await responseStream.CopyToAsync(fs);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        List<string> urlList = new List<string>();
        public async void BeginDownloadAsync(IList<string> urls)
        {
            cacheFolder = await  ApplicationData.Current.LocalFolder.GetFolderAsync("Cache");
            urlList.AddRange(urls);
            for (int i = 0; i < urlList.Count; i++)
            {
                string fileName = EncryptUtils.GetMd5String(urlList[i]);

                var storageFile = await cacheFolder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);

                //如果文件不存在或者为空文件，则开始下载
                if (!await StorageHelper.Instance.StorageItemExistsAsync(fileName))
                { 
                    await SaveImage(urlList[i], storageFile);
                }
                if (ImgLoadedProcess != null)
                {
                    ImgLoadedProcess(urlList[i], storageFile);
                }
            }
        }

    }
}
