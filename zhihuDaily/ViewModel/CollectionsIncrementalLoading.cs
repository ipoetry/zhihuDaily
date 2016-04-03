using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Data;
using zhihuDaily.Model;

namespace zhihuDaily.ViewModel
{
    class CollectionsIncrementalLoading : ObservableCollection<Story>, ISupportIncrementalLoading
    {

        private APIService _api = new APIService();

        private bool _busy = false;
        private bool _has_more_items = false;
        private string _lastTime;

        public event DataLoadingEventHandler DataLoading;
        public event DataLoadedEventHandler DataLoaded;

        public bool HasMoreItems
        {
            get
            {
                if (_busy)
                    return false;
                else
                    return _has_more_items;
            }
            private set
            {
                _has_more_items = value;
            }
        }

        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            return InnerLoadMoreItemsAsync(count).AsAsyncOperation();
        }

        public CollectionsIncrementalLoading(long lastTime)
        {
            HasMoreItems = true;
            _lastTime = lastTime.ToString();
        }


        private async Task<LoadMoreItemsResult> InnerLoadMoreItemsAsync(uint expectedCount)
        {
            _busy = true;
            var actualCount = 0;
            List<Story> list = null;
            CollectionNews collectionNews = null;
            try
            {
                if (DataLoading != null)
                {
                    DataLoading();
                }
                collectionNews = await _api.GetBeforeCollectionStories(_lastTime);
                list = collectionNews.Stories;
            }
            catch (Exception)
            {
                HasMoreItems = false;
            }

            if (collectionNews.LastTime > 0)
            {
                _lastTime = collectionNews.LastTime.ToString();
                HasMoreItems = true;
            }
            else
            {
                HasMoreItems = false;
            }

            if (list != null && list.Any())
            {
                actualCount = list.Count;
                list.ForEach((s) => Add(s));
            }

            if (DataLoaded != null)
            {
                DataLoaded();
            }
            _busy = false;
            return new LoadMoreItemsResult
            {
                Count = (uint)actualCount
            };
        }
    }
}
