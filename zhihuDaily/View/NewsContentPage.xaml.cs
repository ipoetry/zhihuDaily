using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
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
                string msg = e.Value;
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
                    case "up":
                        Debug.WriteLine("up");
                        break;
                    case "down":
                        Debug.WriteLine("down");
                        break;
                    default: break;
                }

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error:" + ex);
            }
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
            if (AppSettings.Instance.UserInfoJson == string.Empty)
            {
                await new Functions().SinaLogin();
                if (AppSettings.Instance.UserInfoJson == string.Empty)
                    return;
            }
            if (newsContentViewModel.StoryExtra.Favorite)
                await WebProvider.GetInstance().SendDeleteRequestAsync($"http://news-at.zhihu.com/api/4/favorite/{newsId}");
            else
                await WebProvider.GetInstance().SendPostRequestAsync($"http://news-at.zhihu.com/api/4/favorite/{newsId}", string.Empty, WebProvider.ContentType.ContentType1);
            newsContentViewModel.StoryExtra.Favorite = !newsContentViewModel.StoryExtra.Favorite;
        }

        private void webView_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            if (!AppSettings.Instance.IsNonePicMode)
            {
                LoadImageAsync();
            }
        }

        private void btnComment_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(NewsCommentPage), new object[] { newsContentViewModel.IdList[newsContentViewModel.currentIndex], newsContentViewModel.StoryExtra });
        }

        private async void btnPopul_Click(object sender, RoutedEventArgs e)
        {
            newsContentViewModel.StoryExtra.VoteStatus =Math.Abs(newsContentViewModel.StoryExtra.VoteStatus-1);
            await WebProvider.GetInstance().SendPostRequestAsync($"http://news-at.zhihu.com/api/4/vote/story/{newsId}", $"data={newsContentViewModel.StoryExtra.VoteStatus}", WebProvider.ContentType.ContentType2);
        }

    }
}
