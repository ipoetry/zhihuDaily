using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using zhihuDaily.DataService;
using zhihuDaily.Model;
using zhihuDaily.ViewModel;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace zhihuDaily
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class NewsContentPage : Page
    {
        AppSettings settings = AppSettings.Instance;
        public NewsContentPage()
        {
            this.InitializeComponent();
        }

        string shareUrl = string.Empty;
        string title = string.Empty;
        string newsId = string.Empty;
        string defaultLoading = string.Empty;

        NewsContentViewModel newsContentViewModel = null;
        Story story = null;
       // TopStory topStory = null;
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Functions.SetTheme(this.wvContainer);
            // if (e.NavigationMode == NavigationMode.New)
            {
                dynamic obj = e.Parameter;
                story = obj.ClickItem as Story;
                //if (story == null)
                //{
                //    topStory = obj.ClickItem as TopStory;
                //    if (topStory == null)
                //        return;
                //    else
                //        story = new Story { Id=topStory.Id, Title =topStory.Title, GaPrefix=topStory.GaPrefix, Type = topStory.Type };

                //}
                if (story != null)
                {
                    downloader.ImgLoadedProcess += Downloader_ImgLoadedProcess; //注册页面图片下载
                    newsId = story.Id.ToString();
                    newsContentViewModel = new NewsContentViewModel(newsId, obj.CurrentList);
                    newsContentViewModel.MessageNoticeHanlder += NewsContentViewModel_MessageNoticeHanlder;
                    this.DataContext = newsContentViewModel;
                }              
            }

            DataTransferManager.GetForCurrentView().DataRequested += OnShareDataRequested;

            #region MvvMLight Message 不可用，原因未知
            //Messenger.Default.Register<NotificationMessage>(this, async (msg) =>
            //{
            //    switch (msg.Notification)
            //    {
            //        case "OnLoadCompleted":
            //                if (msg.Sender != null)
            //                {
            //                    #region Initialize WebView
            //                    NewsContent newsContent = msg.Sender as NewsContent;
            //                    string html = await CombinHtml(newsContent);
            //                    webView.NavigateToString(html);
            //                    if (!AppSettings.Instance.IsNonePicMode)
            //                    {
            //                        LoadImageAsync();
            //                    }
            //                #endregion

            //                title = newsContent.Title;
            //                    shareUrl = newsContent.ShareUrl;
            //                }
            //                break;
            //        default:
            //                break;
            //        }
            //});
            #endregion

        }

        private async void NewsContentViewModel_MessageNoticeHanlder(NewsContent newsContent)
        {
            if (newsContent != null)
            {
                string html = await CombinHtml(newsContent);
                webView.NavigateToString(html);

                title = newsContent.Title;
                shareUrl = newsContent.ShareUrl;
            }
        }

        NewsImageDowloader downloader=new NewsImageDowloader();

        public void LoadImageAsync()
        {
            downloader.BeginDownloadAsync(OriUrls);
        }

        private async void Downloader_ImgLoadedProcess(string value,IStorageFile file)
        {
            String javascript = "img_replace_by_url";
            string base64=string.Empty;
            try
            {
                using (var stream = await file.OpenAsync(FileAccessMode.Read))
                {
                    var reader = new Windows.Storage.Streams.DataReader(stream.GetInputStreamAt(0));
                    var bytes = new byte[stream.Size];
                    await reader.LoadAsync((uint)stream.Size);
                    reader.ReadBytes(bytes);
                    base64 = Convert.ToBase64String(bytes);
                }
                await webView.InvokeScriptAsync(javascript,new string[] { $"{value}", $"data:image/png; base64,{base64}" } );
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
            
        }

        public async Task<string> CombinHtml(NewsContent newsContent)
        {
            StorageFile templateFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/www/template.html"));
            string htmlTemplate = await FileIO.ReadTextAsync(templateFile);

            htmlTemplate = htmlTemplate.Replace("{content}", newsContent.Body); //替换内容

            //设置主题模式
            htmlTemplate = htmlTemplate.Replace("{nightTheme}", Model.AppSettings.Instance.IsNightMode.ToString().ToLower());

            //无图模式或者非wifi状态下 将 newsContent.Image 替换成默认的 headerDef
            
            String headerDef = "ms-appx-web:///Assets/www/news_detail_header_def.jpg";
            if (!AppSettings.Instance.IsNonePicMode)
            {
                headerDef = newsContent.Image;
                defaultLoading = AppSettings.Instance.IsNightMode ? "ms-appx-web:///Assets/Images/default_pic_content_image_loading_dark.png" : "ms-appx-web:///Assets/Images/default_pic_content_image_loading_light.png";
            }
            else {
                defaultLoading = string.Empty;
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("<div class=\"img-wrap\">")
                    .Append("<h1 class=\"headline-title\">")
                    .Append(newsContent.Title).Append("</h1>")
                    .Append("<span class=\"img-source\">")
                    .Append(newsContent.ImageSource).Append("</span>")
                    .Append("<img src=\"").Append(headerDef)
                    .Append("\" alt=\"\">")
                    .Append("<div class=\"img-mask\"></div>");
            htmlTemplate = htmlTemplate.Replace("<div class=\"img-place-holder\">", sb.ToString());
            htmlTemplate = ReplaceImgSrc(htmlTemplate);
            return htmlTemplate;
        }

        
        public string ReplaceImgSrc(string strHtml)
        {
            OriUrls.Clear();
            string pattern = @"(?i)(?<=<img[^>]*?\ssrc=(['""]?))[^'""\s>]+(?=\1[^>]*>)";
            return Regex.Replace(strHtml, pattern, new MatchEvaluator(MatchDel));
        }

        List<string> OriUrls = new List<string>();

        public string MatchDel(Match match)
        {            
            string localFile = "ms-appdata:///local/Cache" + EncryptUtils.GetMd5String(match.Value);
            OriUrls.Add(match.Value);
            if (match.Value == OriUrls[0]&&AppSettings.Instance.IsNonePicMode) //页面第一张大图 是否使用默认图片
            {
                return $"ms-appx-web:///Assets/www/news_detail_header_def.jpg\" src_link=\"{localFile}\" ori_link=\"{match.Value}";
            }

            return $"{defaultLoading}\" src_link=\"{localFile}\" ori_link=\"{match.Value}";
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            downloader.ImgLoadedProcess -= Downloader_ImgLoadedProcess; //反注册页面图片下载
            newsContentViewModel.MessageNoticeHanlder -= NewsContentViewModel_MessageNoticeHanlder;
           // Messenger.Default.Unregister<NotificationMessage>(this);
            DataTransferManager.GetForCurrentView().DataRequested -= OnShareDataRequested;
        }

        private  void webView_ScriptNotify(object sender, NotifyEventArgs e)
        {
            try
            {
                //var model = JsonConvertHelper.JsonDeserialize<JsInvokeModel>(e.Value);
                //switch (model.Type)
                //{
                //    case "swiperight":
                //        if (newsContentViewModel.CurrentIndex > 0)
                //            newsContentViewModel.LoadNewsContent(--newsContentViewModel.CurrentIndex);
                //        break;
                //    case "swipeleft":
                //        if (newsContentViewModel.CurrentIndex < newsContentViewModel.IdList.Count - 1)
                //            newsContentViewModel.LoadNewsContent(++newsContentViewModel.CurrentIndex);
                //        break;
                //    default: break;
                //}
                string msg = e.Value;
                System.Diagnostics.Debug.WriteLine(msg);
                switch (msg)
                {
                    case "goback":
                        if (newsContentViewModel.CurrentIndex > 0)
                        {
                            pr.IsActive = true;
                            newsContentViewModel.LoadNewsContent(--newsContentViewModel.CurrentIndex);
                        }
                            break;

                    case "goforward":
                        if (newsContentViewModel.CurrentIndex < newsContentViewModel.IdList.Count - 1)
                        {
                            pr.IsActive = true;
                            newsContentViewModel.LoadNewsContent(++newsContentViewModel.CurrentIndex);
                        }
                            break;
                    default: break;
                }

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error:" + ex);
            }
        }

        private  void webView_DOMContentLoaded(WebView sender, WebViewDOMContentLoadedEventArgs args)
        {
            //try
            //{

            //    //5、为body添加手势监听
            //    var js = @"var target = document.body;
            //         prepareTarget(target, eventListener);";
            //    await sender.InvokeScriptAsync("eval", new[] { js });

            //}
            //catch (Exception ex)
            //{
            //    System.Diagnostics.Debug.WriteLine("注册手势监听失败：" + ex.ToString());
            //}

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DataTransferManager.ShowShareUI();
        }

        private void OnShareDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            args.Request.Data.Properties.Title = title;
            args.Request.Data.Properties.Description = "[分享自知乎日报]";
            args.Request.Data.SetWebLink(new Uri(shareUrl));
        }

        private async void btn_newsCollection_Click(object sender, RoutedEventArgs e)
        {
            if (!await CollectionDS.Instance.IsFavExisted(story))
            {
               await CollectionDS.Instance.AddFavStory(story);
               this.imgCollection.Source = new BitmapImage(new Uri("ms-appx:///Assets/FunIcon/collected.png"));
            }
            else
            {
                await CollectionDS.Instance.RemoveFav(story);
                this.imgCollection.Source = new BitmapImage(new Uri("ms-appx:///Assets/FunIcon/collect.png"));
            }     
        }

        private async void webView_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            if (!AppSettings.Instance.IsNonePicMode)
            {
                LoadImageAsync();
            }
            this.imgCollection.Source = new BitmapImage(
                await CollectionDS.Instance.IsFavExisted(story) ?
                new Uri("ms-appx:///Assets/FunIcon/collected.png") :
                new Uri("ms-appx:///Assets/FunIcon/collect.png"));
        }

        private void btnComment_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(NewsCommentPage), new object[] { newsContentViewModel.IdList[newsContentViewModel.currentIndex], newsContentViewModel.StoryExtra });
        }
    }
    [DataContract]
    public class JsInvokeModel
    {
        [DataMember(Name = "type")]
        public string Type { get; set; }
    }
}
