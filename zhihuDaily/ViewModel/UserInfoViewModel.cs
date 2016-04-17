using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using zhihuDaily.Model;

namespace zhihuDaily.ViewModel
{
    class UserInfoViewModel: ViewModelBase
    {
        private ElementTheme _appTheme;
        public ElementTheme AppTheme
        {
            get
            {
                return _appTheme;
            }

            set
            {
                _appTheme = value;
                RaisePropertyChanged(() => AppTheme);
            }
        }

        public UserInfoViewModel()
        {
            AppTheme = AppSettings.Instance.CurrentTheme;
        }
    }
}
