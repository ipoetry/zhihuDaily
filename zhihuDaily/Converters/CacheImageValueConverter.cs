using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace zhihuDaily.Converters
{
    public class CacheImageValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            //var image = value as Image;
            //if (image == null || image.BaseUri == null)
            //{
            //    var bitmap = new BitmapImage(new Uri("ms-appx:///Assets/www/news_detail_header_def.jpg"));
            //    var notifier = new TaskCompletionNotifier<BitmapImage>();
            //    notifier.SetTask(Task.FromResult(bitmap));
            //    return notifier;
            //}
            //else
            //{
            //    var task = Task.Run(async () =>
            //    {
            //        var cache = ServiceLocator.Current.GetInstance<ImageCache>();
            //        var uri = await cache.GetImageSourceFromUrlAsync(image.BaseUri.ToString());
            //        return uri;
            //    });
            //    var notifier = new TaskCompletionNotifier<BitmapImage>();
            //    notifier.SetTask(task, c => new BitmapImage(c));
            //    return notifier;
            //}
            var image = value;
            if(image != null)
            {              
                var task = Task.Run(async () =>
                {
                    var cache = await ImageCache.CreateInstance();
                    var uri = await cache.GetImageSourceFromUrlAsync(image.ToString());
                    return uri;
                });
                var notifier = new TaskCompletionNotifier<BitmapImage>();
                notifier.SetTask(task, c => new BitmapImage(c));
                return notifier;
            }
            return string.Empty;
        }


        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
