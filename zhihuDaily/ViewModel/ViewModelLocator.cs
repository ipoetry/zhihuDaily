using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using zhihuDaily.DataService;
using zhihuDaily.Model;

namespace zhihuDaily.ViewModel
{
    public class ViewModelLocator
    {

        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<ICommonService<StartImage>, CommonService<StartImage>>();
            SimpleIoc.Default.Register<ICommonService<Themes>, CommonService<Themes>>();
            SimpleIoc.Default.Register<ICommonService<LatestNews>, CommonService<LatestNews>>();

           // SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<HomePageViewModel>();
            SimpleIoc.Default.Register<AppShellViewModel>();
           // SimpleIoc.Default.Register<ThemePageViewModel>();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        // public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();
        public AppShellViewModel AppShell => ServiceLocator.Current.GetInstance<AppShellViewModel>();
        public HomePageViewModel HomePage => ServiceLocator.Current.GetInstance<HomePageViewModel>();
        //  public ThemePageViewModel ThemePage => ServiceLocator.Current.GetInstance<ThemePageViewModel>();

    }
}
