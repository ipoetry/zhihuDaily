using GalaSoft.MvvmLight.Messaging;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using zhihuDaily.Model;
using zhihuDaily.ViewModel;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace zhihuDaily.View
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class EditSinaShare : Page
    {
        public EditSinaShare()
        {
            this.InitializeComponent();
        }
        EditSinaSharePageViewModel _viewModel;
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            object[] parameters = e.Parameter as object[];
            if (parameters != null && parameters[0] != null)
            {
                ShareObject objShare = parameters[0] as ShareObject;
                this.DataContext = _viewModel = new EditSinaSharePageViewModel(objShare);
            }
            
            Messenger.Default.Register<NotificationMessage>(this, (msg) =>
            {
                switch (msg.Notification)
                {
                    case "gobackSharePage":
                        this.Frame.GoBack();
                        break;
                    default:
                        break;
                }
            });

            InputPane.GetForCurrentView().Showing += EditSinaShare_Showing;
            InputPane.GetForCurrentView().Hiding += EditSinaShare_Hiding;
        }

        private void EditSinaShare_Hiding(InputPane sender, InputPaneVisibilityEventArgs args)
        {
            MainContainer.Margin = new Thickness(0, 0, 0, 0);
        }

        private void EditSinaShare_Showing(InputPane sender, InputPaneVisibilityEventArgs args)
        {
            MainContainer.Margin =new Thickness(0,0,0, args.OccludedRect.Height);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            InputPane.GetForCurrentView().Showing -= EditSinaShare_Showing;
            InputPane.GetForCurrentView().Hiding -= EditSinaShare_Hiding;
        }


    }
}
