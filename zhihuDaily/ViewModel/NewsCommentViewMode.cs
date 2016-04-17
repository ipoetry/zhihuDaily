using GalaSoft.MvvmLight;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using zhihuDaily.DataService;
using zhihuDaily.Model;

namespace zhihuDaily.ViewModel
{
    class NewsCommentViewMode : ViewModelBase
    {
        private APIService _api = new APIService();
        private string _story_id;
        public string StoryId { get { return _story_id; } }
        public NewsCommentViewMode(string story_id, StoryExtra extra)
        {
            _story_id = story_id;
            StoryExtra = extra;
            AppTheme = AppSettings.Instance.CurrentTheme;
            LoadMainSource();
        }

        private StoryExtra _storyExtra;

        public StoryExtra StoryExtra
        {
            get { return _storyExtra; }
            set {
                _storyExtra = value;
                RaisePropertyChanged(() => StoryExtra);
            }
        }

        private bool hasLongComments = false;
        /// <summary>
        /// 是否有长评论
        /// </summary>
        public bool HasLongComment
        {
            get { return hasLongComments; }
            set
            {
                hasLongComments = value;
                RaisePropertyChanged(() => HasLongComment);
            }
        }

        private bool hasShortComments = false;
        /// <summary>
        /// 是否有短评论
        /// </summary>
        public bool HasShortComment
        {
            get { return hasShortComments; }
            set
            {
                hasShortComments = value;
                RaisePropertyChanged(() => HasShortComment);
            }
        }

        private CommentIncrementalLoading _short_comments;
        public CommentIncrementalLoading ShortComments
        {
            get
            {
                return _short_comments;
            }
            set
            {
                _short_comments = value;
                RaisePropertyChanged(() => ShortComments);
            }
        }

        private CommentIncrementalLoading _long_comments;
        public CommentIncrementalLoading LongComments
        {
            get
            {
                return _long_comments;
            }
            set
            {
                _long_comments = value;
                RaisePropertyChanged(() => LongComments);
            }
        }

        private bool isActive;
        public bool IsActive
        {
            get
            {
                return isActive;
            }
            set
            {
                isActive = value;
                RaisePropertyChanged(() => IsActive);
            }
        }

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


        public async void LoadMainSource()
        {
            IsActive = true;

            var t1 = _api.GetShortComments(_story_id);
            var t2 = _api.GetLongComments(_story_id);

            List<Comment> short_comments = await t1;
            List<Comment> long_comments = await t2;

            if (short_comments != null && short_comments.Any())
            {
                CommentIncrementalLoading c = new CommentIncrementalLoading(short_comments.Last().Id.ToString(), _story_id, true);
                short_comments.ForEach((comment) => { c.Add(comment); });
                HasShortComment = short_comments.Count > 0;
                ShortComments = c;

                c.DataLoaded += C_DataLoaded;
                c.DataLoading += C_DataLoading;
            }
            if (long_comments != null && long_comments.Any())
            {
                CommentIncrementalLoading c = new CommentIncrementalLoading(long_comments.Last().Id.ToString(), _story_id, false);
                long_comments.ForEach((comment) => { c.Add(comment); });
                HasLongComment = long_comments.Count > 0;
                LongComments = c;

                c.DataLoaded += C_DataLoaded;
                c.DataLoading += C_DataLoading;
            }
            IsActive = false;
        }

        /// <summary>
        /// 
        /// </summary>
        private void C_DataLoading()
        {
            IsActive = true;
        }
        /// <summary>
        /// 
        /// </summary>
        private void C_DataLoaded()
        {
            IsActive = false;
        }

    }

}
