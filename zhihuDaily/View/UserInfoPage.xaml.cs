using System;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using zhihuDaily.DataService;
using zhihuDaily.Model;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace zhihuDaily.View
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class UserInfoPage : Page
    {
        public UserInfoPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            this.DataContext = ViewModel.ViewModelLocator.AppShell;
        }

        private async void btnChangeAvater_Tapped(object sender, TappedRoutedEventArgs e)
        {
            FileOpenPicker picker = new FileOpenPicker();
            picker.ViewMode = PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            // 选取单个文件
            StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                var stream = await file.OpenSequentialReadAsync();
                await WebProvider.GetInstance().SendPostRequestAsync("http://news-at.zhihu.com/api/4/avatar", stream);
                string resJosn = await WebProvider.GetInstance().GetRequestDataAsync("http://news-at.zhihu.com/api/4/account");
                var UserInfo = JsonConvertHelper.JsonDeserialize<UserInfo>(resJosn);
                ViewModel.ViewModelLocator.AppShell.UserInfo.Avatar = UserInfo.Avatar;
                ViewModel.ViewModelLocator.AppShell.UserInfo.Name = UserInfo.Name;
                ViewModel.ViewModelLocator.AppShell.UserInfo.BoundServices = UserInfo.BoundServices;
                AppSettings.Instance.UserInfoJson = JsonConvertHelper.JsonSerializer(ViewModel.ViewModelLocator.AppShell.UserInfo);
            }
        }

        private async void tbEdit_Unchecked(object sender, RoutedEventArgs e)
        {
            string userName = tbUserName.Text.Trim();
            if (userName!=string.Empty && userName != ViewModel.ViewModelLocator.AppShell.UserInfo.Name)
            {
                ViewModel.ViewModelLocator.AppShell.UserInfo.Name = userName;

                string postJosn = "{" + $"\"name\":\"{userName}\"" + "}";
                await WebProvider.GetInstance().SendPostRequestAsync("http://news-at.zhihu.com/api/4/name",postJosn, WebProvider.ContentType.ContentType3);
                AppSettings.Instance.UserInfoJson = JsonConvertHelper.JsonSerializer(ViewModel.ViewModelLocator.AppShell.UserInfo);
            }
        }

        private void btn_Logout_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ViewModelLocator.AppShell.UserInfo = new UserInfo();
            AppSettings.Instance.UserInfoJson = string.Empty;
            if(this.Frame.CanGoBack)
                this.Frame.GoBack();
        }
    }
}
