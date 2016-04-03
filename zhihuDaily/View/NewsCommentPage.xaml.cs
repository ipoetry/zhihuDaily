using GalaSoft.MvvmLight.Messaging;
using Windows.ApplicationModel.DataTransfer;
using Windows.Data.Json;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using zhihuDaily.Controls;
using zhihuDaily.DataService;
using zhihuDaily.Model;
using zhihuDaily.View;
using zhihuDaily.ViewModel;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace zhihuDaily
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class NewsCommentPage : Page
    {
        private NewsCommentViewMode _viewModel;

        public NewsCommentPage()
        {
            this.InitializeComponent();
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.NavigationMode == NavigationMode.Back && e.SourcePageType.Name == "NewsCommentPage")
            {
                return;
            }
            Functions.SetTheme(this.mainContaier);
            object[] parameters = e.Parameter as object[];
            if (parameters != null && parameters[0] != null && parameters[1] != null)
            {
                this.DataContext = _viewModel = new NewsCommentViewMode(parameters[0].ToString(), parameters[1] as StoryExtra);
            }

            Messenger.Default.Register<Comment>(this, (msg) =>
            {
                if (_viewModel.ShortComments != null)
                {
                    _viewModel.ShortComments.Insert(0,msg);
                }
            });
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (e.SourcePageType.Name != "PostCommentPage")
            {
                Messenger.Default.Unregister<Comment>(this);
            }
        }

        private async void btnPostComment_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (AppSettings.Instance.UserInfoJson == string.Empty)
            {
                await new Functions().SinaLogin();
            }
            Frame.Navigate(typeof(PostCommentPage), new[] { _viewModel.StoryId, string.Empty });
        }

        private async void btnAgree_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Comment selectedComment = ((MenuFlyoutItem)sender).DataContext as Comment;

            if (ViewModelLocator.AppShell.UserInfo.Name == string.Empty)
            {
                selectedComment.Likes++;
                selectedComment.Voted = !selectedComment.Voted;
                return;
            }

            if (!selectedComment.Voted)
            {
                string resJosn = await WebProvider.GetInstance().SendPostRequestAsync($"http://news-at.zhihu.com/api/4/vote/comment/{selectedComment.Id}", string.Empty, WebProvider.ContentType.ContentType1);
                if (!string.IsNullOrEmpty(resJosn))
                {
                    selectedComment.Likes = (int)JsonObject.Parse(resJosn)["count"].GetNumber();
                }

            }
            else
            {
                string resJosn = await WebProvider.GetInstance().SendDeleteRequestAsync($"http://news-at.zhihu.com/api/4/vote/comment/{selectedComment.Id}");
                if (!string.IsNullOrEmpty(resJosn))
                {
                    selectedComment.Likes = (int)JsonObject.Parse(resJosn)["count"].GetNumber();
                }
            }
            selectedComment.Voted = !selectedComment.Voted;
        }

        private void btnReport_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Comment selectedComment = ((MenuFlyoutItem)sender).DataContext as Comment;
            var msgPopup = new MessagePopupWindow("");
            msgPopup.RightClick += async(s, ea) => {
                string resJosn = await WebProvider.GetInstance().SendDeleteRequestAsync($"http://news-at.zhihu.com/api/4/report/comment/{selectedComment.Id}");
            };
            msgPopup.ShowWIndow();
        }

        private void btnCopy_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Comment selectedComment = ((MenuFlyoutItem)sender).DataContext as Comment;
            DataPackage dp = new DataPackage();
            dp.SetText(selectedComment.Content);
            Clipboard.SetContent(dp);
        }

        private async void btnReply_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await new Functions().SinaLogin();
            Comment selectedComment = ((MenuFlyoutItem)sender).DataContext as Comment;
            Frame.Navigate(typeof(PostCommentPage), new[] { _viewModel.StoryId, JsonConvertHelper.JsonSerializer(selectedComment) });
        }

        private async void btnDelete_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Comment selectedComment = ((MenuFlyoutItem)sender).DataContext as Comment;
            await WebProvider.GetInstance().SendDeleteRequestAsync($"http://news-at.zhihu.com/api/4/comment/{selectedComment.Id}");

            if (_viewModel.LongComments!=null&&_viewModel.LongComments.Contains(selectedComment))
            {
                _viewModel.LongComments.Remove(selectedComment);
            }
            if (_viewModel.ShortComments!=null&&_viewModel.ShortComments.Contains(selectedComment))
            {
                _viewModel.ShortComments.Remove(selectedComment);
            }
        }
    }
}
