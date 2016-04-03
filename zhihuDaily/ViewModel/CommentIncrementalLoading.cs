using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Data;
using zhihuDaily.DataService;
using zhihuDaily.Model;

namespace zhihuDaily.ViewModel
{
    public delegate void DataLoadingEventHandler();
    public delegate void DataLoadedEventHandler();

    class CommentIncrementalLoading : ObservableCollection<Comment>, ISupportIncrementalLoading
    {
        private APIService _api = new APIService();

        private bool _busy = false;
        private bool _has_more_items = false;
        private string _last_comment_id;
        private string _story_id;
        private bool _short_comment;

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

        public CommentIncrementalLoading(string last_comment_id, string story_id, bool short_comment)
        {
            _last_comment_id = last_comment_id;
            _story_id = story_id;
            _short_comment = short_comment;
            HasMoreItems = true;
        }


        private async Task<LoadMoreItemsResult> InnerLoadMoreItemsAsync(uint expectedCount)
        {
            _busy = true;
            var actualCount = 0;
            List<Comment> list = null;
            try
            {
                if (DataLoading != null)
                {
                    DataLoading();
                }
                if (_short_comment)
                {
                    list = await _api.GetBeforeShortComments(_story_id, _last_comment_id);
                }
                else
                {
                    list = await _api.GetBeforeLongComments(_story_id, _last_comment_id);
                }
            }
            catch (Exception)
            {
                HasMoreItems = false;
            }

            if (list != null && list.Any())
            {
                actualCount = list.Count;
                list.ForEach((s) => Add(s));
                _last_comment_id = list.Last().Id.ToString();  //更新最后一篇评论的id
                HasMoreItems = true;
            }
            else
            {
                HasMoreItems = false;
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

    class APIService
    {
        /// <summary>
        /// 获取文章短评论
        /// </summary>
        /// <param name="story_id"></param>
        /// <returns></returns>
        public async Task<List<Comment>> GetShortComments(string story_id)
        {
            try
            {
                ICommonService<Comments> _commentService = new CommonService<Comments>();
                var resObj = await _commentService.GetNotAvailableCacheObjAsync("story",story_id,"short-comments");

                return resObj.CommentList.ToList();
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                return new List<Comment>();
            }
        }
        /// <summary>
        /// 获取文章长评论
        /// </summary>
        /// <param name="story_id"></param>
        /// <returns></returns>
        public async Task<List<Comment>> GetLongComments(string story_id)
        {
            try
            {
                ICommonService<Comments> _commentService = new CommonService<Comments>();
                var resObj = await _commentService.GetNotAvailableCacheObjAsync("story", story_id, "long-comments");

                return resObj.CommentList.ToList();
            }
            catch
            {
                return new List<Comment>();
            }
        }
        /// <summary>
        /// 分页获取文章短评论
        /// </summary>
        /// <param name="story_id"></param>
        /// <param name="last_comment_id"></param>
        /// <returns></returns>
        public async Task<List<Comment>> GetBeforeShortComments(string story_id, string last_comment_id)
        {
            try
            {
                ICommonService<Comments> _commentService = new CommonService<Comments>();
                var resObj = await _commentService.GetNotAvailableCacheObjAsync("story", story_id, "short-comments","before",last_comment_id);

                return resObj.CommentList.ToList();
            }
            catch
            {
                return new List<Comment>();
            }
        }
        /// <summary>
        /// 分页获取文章长评论
        /// </summary>
        /// <param name="story_id"></param>
        /// <param name="last_comment_id"></param>
        /// <returns></returns>
        public async Task<List<Comment>> GetBeforeLongComments(string story_id, string last_comment_id)
        {
            try
            {
                ICommonService<Comments> _commentService = new CommonService<Comments>();
                var resObj = await _commentService.GetNotAvailableCacheObjAsync("story", story_id, "long-comments", "before", last_comment_id);

                return resObj.CommentList.ToList();
            }
            catch
            {
                return new List<Comment>();
            }
        }

        public async Task<CollectionNews> GetCollectionStories()
        {
            try
            {
                ICommonService<CollectionNews> _commentService = new CommonService<CollectionNews>();
                var resObj = await _commentService.GetNotAvailableCacheObjAsync("favorites/");

                return resObj;
            }
            catch
            {
                return new CollectionNews {Stories =new List<Story>()};
            }
        }
        public async Task<CollectionNews> GetBeforeCollectionStories(string lastTime)
        {
            try
            {
                ICommonService<CollectionNews> _commentService = new CommonService<CollectionNews>();
                var resObj = await _commentService.GetNotAvailableCacheObjAsync("favorites","before",lastTime);

                return resObj;
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                return new CollectionNews { Stories = new List<Story>() };
            }
        }
    }
}
