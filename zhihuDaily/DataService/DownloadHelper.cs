using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;

namespace zhihuDaily.DataService
{
    class DownloadHelper
    {
        private async static Task<StorageFile> SaveAsync(Uri fileUri,StorageFolder folder,string fileName)
        {
            var file = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            var downloader = new BackgroundDownloader();
            var download = downloader.CreateDownload(
                fileUri,
                file);

            var res = await download.StartAsync();
            return file;
        }

        public static async void SaveImage(string url)
        {
            try
            {
                if (string.IsNullOrEmpty(url))
                    return;
                StorageFolder applicationFolder = ApplicationData.Current.LocalFolder;
                await SaveAsync(new Uri(url), applicationFolder, "SplashImage.jpg");
            }
            catch (Exception)
            {   }
        }
    }
}
