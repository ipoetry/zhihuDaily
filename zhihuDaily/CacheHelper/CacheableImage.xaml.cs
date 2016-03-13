using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using zhihuDaily.DataService;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace zhihuDaily
{
    public sealed partial class CacheableImage : UserControl
    {
        public CacheableImage()
        {
            this.InitializeComponent();
        }


        private static StorageFolder _cacheFolder;

        public static async Task LoadCache()
        {
            _cacheFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Cache", CreationCollisionOption.OpenIfExists);
        }

        public static readonly DependencyProperty CachedDirectoryProperty = DependencyProperty.Register(
            "CachedDirectory", typeof(string), typeof(CacheableImage), new PropertyMetadata("/ImageCached/"));

        public static readonly DependencyProperty FaildImageUrlProperty = DependencyProperty.Register(
            "FaildImageUrl", typeof(Uri), typeof(CacheableImage), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty LoadingImageUrlProperty = DependencyProperty.Register(
            "LoadingImageUrl", typeof(Uri), typeof(CacheableImage), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty StretchProperty = DependencyProperty.Register(
            "Stretch", typeof(Stretch), typeof(CacheableImage), new PropertyMetadata(default(Stretch), StretchPropertyChangedCallback));

        private static void StretchPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var cachedImage = (CacheableImage)dependencyObject;
            var stretch = (Stretch)dependencyPropertyChangedEventArgs.NewValue;
            if (cachedImage.Image != null)
            {
                cachedImage.Image.Stretch = stretch;
            }
        }

        public Stretch Stretch
        {
            get { return (Stretch)GetValue(StretchProperty); }
            set { SetValue(StretchProperty, value); }
        }


        /// <summary>
        /// 加载失败的图片
        /// </summary>
        public Uri FaildImageUrl
        {
            get { return (Uri)GetValue(FaildImageUrlProperty); }
            set { SetValue(FaildImageUrlProperty, value); }
        }

        /// <summary>
        /// 加载中显示的图片（需要进行网络请求时）
        /// </summary>
        public Uri LoadingImageUrl
        {
            get { return (Uri)GetValue(LoadingImageUrlProperty); }
            set { SetValue(LoadingImageUrlProperty, value); }
        }


        /// <summary>
        /// 缓存到本地的目录
        /// </summary>
        public string CachedDirectory
        {
            get { return (string)GetValue(CachedDirectoryProperty); }
            set { SetValue(CachedDirectoryProperty, value); }
        }


        public static readonly DependencyProperty ImageUrlProperty = DependencyProperty.Register(
            "ImageUrl", typeof(string), typeof(CacheableImage), new PropertyMetadata(default(string), ImageUrlPropertyChangedCallback));

        public string ImageUrl
        {
            get { return (string)GetValue(ImageUrlProperty); }
            set { SetValue(ImageUrlProperty, value); }
        }

        private static async void ImageUrlPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var cachedImage = (CacheableImage)dependencyObject;
            var imageUrl = (string)dependencyPropertyChangedEventArgs.NewValue;

            if (string.IsNullOrEmpty(imageUrl))
            {
                return;
            }

            if (imageUrl.StartsWith("http://") || imageUrl.Equals("https://"))
            {
                 await LoadCache();
                 Uri uri=  await GetImageSourceFromUrlAsync(imageUrl);
                 cachedImage.Image.Source = new BitmapImage(uri);
            }
            else
            {
                //本地图片
                var bitmapImage = new BitmapImage(new Uri(imageUrl, UriKind.Relative));
                cachedImage.Image.Source = bitmapImage;
            }
        }

        public static async Task<Uri> GetImageSourceFromUrlAsync(string url)
        {
            string fileName = EncryptUtils.GetMd5String(url);

            if (await StorageHelper.Instance.StorageItemExistsAsync(fileName))
            {
                if (await DownloadAndSaveAsync(url, fileName))
                {
                    return new Uri("ms-appdata:///local/Cache/" + fileName);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Download image failed. " + url);
                }
            }           
            return new Uri(url);
        }

        private static async Task<bool> DownloadAndSaveAsync(string url, string fileName)
        {
            try
            {
                var request = WebRequest.CreateHttp(url);

                request.Method = "GET";
                using (var response = await request.GetResponseAsync())
                {
                    using (var responseStream = response.GetResponseStream())
                    {
                        var file =
                            await _cacheFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
                        using (var fs = await file.OpenStreamForWriteAsync())
                        {
                            await responseStream.CopyToAsync(fs);
                            System.Diagnostics.Debug.WriteLine("Downloaded: " + url);
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



        public static readonly DependencyProperty ImageStreamProperty = DependencyProperty.Register(
            "ImageStream", typeof(Stream), typeof(CacheableImage), new PropertyMetadata(default(Stream), ImageStreamPropertyChangedCallback));

        private static async void ImageStreamPropertyChangedCallback(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var cachedImage = (CacheableImage)dependencyObject;
            var imageStream = (IRandomAccessStream)dependencyPropertyChangedEventArgs.NewValue;

            var bitmapImage = new BitmapImage();
            await bitmapImage.SetSourceAsync(imageStream);
            cachedImage.Image.Source = bitmapImage;
        }

        /// <summary>
        /// 支持直接传递流进来
        /// </summary>
        public Stream ImageStream
        {
            get { return (Stream)GetValue(ImageStreamProperty); }
            set { SetValue(ImageStreamProperty, value); }
        }
    }
}
