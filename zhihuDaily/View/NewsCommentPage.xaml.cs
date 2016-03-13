using Windows.UI.Xaml.Controls;
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
    public sealed partial class NewsCommentPage : Page
    {
        private NewsCommentViewMode _viewModel;
        AppSettings settings = AppSettings.Instance;
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
            Functions.SetTheme(this.mainContaier);
            object[] parameters = e.Parameter as object[];
            if (parameters != null && parameters[0] != null && parameters[1] != null)
            {
                this.DataContext = _viewModel = new NewsCommentViewMode(parameters[0].ToString(), parameters[1] as StoryExtra);
            }
        }

    }

}
