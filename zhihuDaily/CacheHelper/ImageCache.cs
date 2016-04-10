using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Windows.Storage;
using zhihuDaily.DataService;

namespace zhihuDaily
{
    public class ImageCache
    {
        private const string ImageCacheFolder = "ImageCache";
        private readonly string msImageCacheFolder = $"ms-appdata:///local/{ImageCacheFolder}/";
        private StorageFolder _cacheFolder;
        private IList<string> _cachedFileNames;

        private ImageCache()
        {

        }
        public async Task<Uri> GetImageSourceFromUrlAsync(string url)
        {
            string fileName = EncryptUtils.GetMd5String(url);
            if (this._cachedFileNames.Contains(fileName))
            {
                return new Uri(msImageCacheFolder + fileName);
            }
            if (await DownloadAndSaveAsync(url, fileName))
            {
                _cachedFileNames.Add(fileName);
                return new Uri(msImageCacheFolder + fileName);
            }
            System.Diagnostics.Debug.WriteLine("Download image failed. " + url);
            return new Uri(url);
        }

        private async Task<bool> DownloadAndSaveAsync(string url, string fileName)
        {
            try
            {
                var request = WebRequest.CreateHttp(url);
                request.Method = "GET";
                using (var response = await request.GetResponseAsync())
                {
                    using (var responseStream = response.GetResponseStream())
                    {
                        var file = await this._cacheFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
                        using (var fs = await file.OpenStreamForWriteAsync())
                        {
                            await responseStream.CopyToAsync(fs);
                            System.Diagnostics.Debug.WriteLine("new Downloaded: " + url);
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error: " + ex.Message);
                return false;
            }
        }

        public async Task LoadCache()
        {
            this._cacheFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(ImageCacheFolder, CreationCollisionOption.OpenIfExists);
            await ClearOutDateCache();
            this._cachedFileNames = (await this._cacheFolder.GetFilesAsync()).Select(c => c.Name).ToList();
        }
        /// <summary>
        /// 清除7天前的图片缓存
        /// </summary>
        /// <returns></returns>
        public async Task ClearOutDateCache()
        {
           var files = await this._cacheFolder.GetFilesAsync();
            foreach (var file in files)
            {
                if((file.DateCreated-DateTime.Now).TotalDays>7)
                    await file.DeleteAsync();
            }
        }

        public IList<string> CachedFileNames
        {
            get { return _cachedFileNames; }
            set { _cachedFileNames = value; }
        }
        private volatile static ImageCache _instance = null;

        private static readonly object lockHelper = new object();
        /// <summary>
        /// 非线程安全的
        /// </summary>
        /// <returns></returns>
        public static async  Task<ImageCache> CreateInstance()
        {
            if (_instance == null)
            {
                lock (lockHelper)
                {
                    if (_instance == null)
                    { 
                        _instance = new ImageCache();
                    }
                }
                await _instance.LoadCache();
            }           
            return _instance;
        }
    }
}
