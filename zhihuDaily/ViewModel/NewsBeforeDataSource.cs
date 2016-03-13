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
        private readonly ICommonService<LatestNews> _latestNesService;

        public NewsBeforeDataSource(ICommonService<LatestNews> latestNesService)
        {
            _latestNesService = latestNesService;
        }

        protected async override Task<IList<Story>> LoadItemsAsync()
        {
            string date = DateTime.Now.AddDays(-_currentPage).ToString("yyyyMMdd");
            var result = await _latestNesService.GetObjectAsync("news", "before", date);
            if (result.Stories != null)
            {
                List<Story> tempStorys = new List<Story>();
               // tempStorys = .ToList();
                tempStorys.Add(new Story { Date= DateTime.Now.AddDays(-_currentPage), IsDateTitleDisplay = Windows.UI.Xaml.Visibility.Visible,IsStoryItemDisplay= Windows.UI.Xaml.Visibility.Collapsed });
               
                tempStorys.AddRange(result.Stories);
                for (int i = 1; i < tempStorys.Count; i++)
                {
                    tempStorys[i].Date = DateTime.Now.AddDays(-_currentPage);
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
