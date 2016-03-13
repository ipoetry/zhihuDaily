using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using zhihuDaily.DataService;
using zhihuDaily.Model;

namespace zhihuDaily.ViewModel
{
    class ThemeBeforeDataSource : DataSourceBase<Story>
    {

        private readonly ICommonService<IList<Story>> _latestNesService;
        private  int _currentThemeId;
        public  int _lastThemeStoryId;
        public ThemeBeforeDataSource(ICommonService<IList<Story>> latestNesService,int themeId,int lastThemeStoryId)
        {
            _latestNesService = latestNesService;
            _currentThemeId = themeId;
            _lastThemeStoryId = lastThemeStoryId;
        }
        protected async override Task<IList<Story>> LoadItemsAsync()
        {
            var result = await _latestNesService.GetObjectAsync($"theme/{_currentThemeId}", "before", _lastThemeStoryId.ToString());
            if (result.LastOrDefault()!=null)
            {
                _lastThemeStoryId = result.LastOrDefault().Id;
            }
            return result;
        }
    }
}
