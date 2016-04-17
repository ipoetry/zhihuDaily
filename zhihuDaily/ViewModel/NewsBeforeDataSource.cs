using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using zhihuDaily.DataService;
using zhihuDaily.Model;

namespace zhihuDaily.ViewModel
{
    public class NewsBeforeDataSource : DataSourceBase<Story>
    {
        private readonly ICommonService<LatestNews> _latestNewsService;
        private DateTime _latestDate;
        public NewsBeforeDataSource(ICommonService<LatestNews> latestNewsService,DateTime latestDate)
        {
            _latestNewsService = latestNewsService;
            _latestDate = latestDate;
        }

        protected async override Task<IList<Story>> LoadItemsAsync()
        {
            string date = _latestDate.AddDays(-_currentPage).ToString("yyyyMMdd");
            var result = await _latestNewsService.GetObjectAsync("stories", "before", date);
            if (result.Stories != null)
            {
                List<Story> tempStorys = new List<Story>();
               // tempStorys = .ToList();
                tempStorys.Add(new Story { Date= _latestDate.AddDays(-_currentPage-1), IsDateTitleDisplay = Windows.UI.Xaml.Visibility.Visible,IsStoryItemDisplay= Windows.UI.Xaml.Visibility.Collapsed });
               
                tempStorys.AddRange(result.Stories);
                for (int i = 1; i < tempStorys.Count; i++)
                {
                    tempStorys[i].Date = _latestDate.AddDays(-_currentPage-1);
                    tempStorys[i].IsDateTitleDisplay = Windows.UI.Xaml.Visibility.Collapsed;
                    tempStorys[i].IsStoryItemDisplay = Windows.UI.Xaml.Visibility.Visible;
                }
                return tempStorys;
            }
            return null;
           // return result.Stories == null ? null : result.Stories.ToList();
        }

        protected override void AddItems(IList<Story> items)
        {
            if (items != null)
            {
                this.Add(items[0]);
                foreach (var news in items)
                {
                    if (!this.Any(n => n.Id == news.Id))
                    {
                        this.Add(news);
                    }
                }
            }
        }
    }
}
