using GalaSoft.MvvmLight;
using Windows.UI.Popups;
using zhihuDaily.DataService;
using zhihuDaily.Model;
using System;
using Windows.UI.Xaml;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.Generic;

namespace zhihuDaily.ViewModel
{
    class ThemePageViewModel: ViewModelBase
    {
       // private readonly ICommonService<IList<Story>> _latestNewsService;
        public ThemePageViewModel(string themeId)
        {
            this.LoadTheme(themeId);
            
            this.ItemClickCommand = new RelayCommand<object>((e) =>
            {
                Messenger.Default.Send(new NotificationMessage(e, "OnItemClick"));
            });

            this.RefreshCommand = new RelayCommand(() =>
            {
                //Refresh the data
                System.Diagnostics.Debug.WriteLine("ddfgd");
                this.LoadTheme(themeId);
            });
        }

        // if progress is complete property 
        private bool isCompleted = false;

        public bool IsCompleted
        {
            get { return isCompleted; }
            set
            {
                isCompleted = value;
                RaisePropertyChanged(() => IsCompleted);
            }
        }

        private ThemeBeforeDataSource newsDS;

        public ThemeBeforeDataSource NewsDS
        {
            get { return newsDS; }
            set
            {
                newsDS = value;
                RaisePropertyChanged(() => NewsDS);
            }
        }

        private Theme theme;
        /// <summary>
        /// 主题列表
        /// </summary>
        public Theme Theme
        {
            get { return theme; }
            set
            {
                theme = value;
                RaisePropertyChanged(() => Theme);
            }
        }

        public RelayCommand<object> ItemClickCommand { set; get; }
        public RelayCommand RefreshCommand { get; set; }

        public async void LoadTheme(string _id)
        {
            ICommonService<Theme> themeService = new CommonService<Theme>();
            Theme result = await themeService.GetObjectAsync("theme", _id);
            int themeLastStoryId = 0;
           // this.NewsDS = new ThemeBeforeDataSource(_latestNewsService, int.Parse(_id), themeLastStoryId);
            if (result != null)
            {
                this.Theme = new Theme();
                this.Theme.Stories = result.Stories;
                this.Theme.Name = result.Name;
                this.Theme.Editors = result.Editors;
                this.Theme.Background = result.Background;
                isCompleted = false;
                //if (this.Theme.Stories != null)
                //{
                //    foreach (var story in this.Theme.Stories)
                //    {
                //        themeLastStoryId = story.Id;
                //        this.NewsDS.Add(story);
                //    }
                //    this.NewsDS._lastThemeStoryId = themeLastStoryId;
                //}
                
            }
        }
    }
}
