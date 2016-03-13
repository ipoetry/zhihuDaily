﻿using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using zhihuDaily.Model;

namespace zhihuDaily.ViewModel
{
    class NewsCommentViewMode : ViewModelBase
    {
        private APIService _api = new APIService();
        private string _story_id;
        private StoryExtra _se;

        public NewsCommentViewMode(string story_id, StoryExtra extra)
        {
            _story_id = story_id;
            _se = extra;

            Update();
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

        private string _total_comments;
        public string TotalComments
        {
            get
            {
                return _total_comments;
            }
            set
            {
                _total_comments = value;
                RaisePropertyChanged(() => TotalComments);
            }
        }
        private string _total_short_comments;
        public string TotalShortComments
        {
            get
            {
                return _total_short_comments;
            }
            set
            {
                _total_short_comments = value;
                RaisePropertyChanged(() => TotalShortComments);
            }
        }
        private string _total_long_comments;
        public string TotalLongComments
        {
            get
            {
                return _total_long_comments;
            }
            set
            {
                _total_long_comments = value;
                RaisePropertyChanged(() => TotalLongComments);
            }
        }

        private bool _is_loading;
        public bool IsLoading
        {
            get
            {
                return _is_loading;
            }
            set
            {
                _is_loading = value;
                RaisePropertyChanged(() => IsLoading);
            }
        }

        public async void Update()
        {
            IsLoading = true;

            TotalComments = _se.Comments.ToString();
            TotalLongComments = _se.LongComments.ToString();
            TotalShortComments = _se.ShortComments.ToString();

            var t1 = _api.GetShortComments(_story_id);
            var t2 = _api.GetLongComments(_story_id);

            List<Comment> short_comments = await t1;
            List<Comment> long_comments = await t2;

            if (short_comments != null && short_comments.Any())
            {
                CommentIncrementalLoading c = new CommentIncrementalLoading(short_comments.Last().Id.ToString(), _story_id, true);
                short_comments.ForEach((comment) => { c.Add(comment); });

                ShortComments = c;

                c.DataLoaded += C_DataLoaded;
                c.DataLoading += C_DataLoading;
            }
            if (long_comments != null && long_comments.Any())
            {
                CommentIncrementalLoading c = new CommentIncrementalLoading(long_comments.Last().Id.ToString(), _story_id, false);
                long_comments.ForEach((comment) => { c.Add(comment); });

                LongComments = c;

                c.DataLoaded += C_DataLoaded;
                c.DataLoading += C_DataLoading;
            }

            IsLoading = false;
        }

        /// <summary>
        /// 
        /// </summary>
        private void C_DataLoading()
        {
            IsLoading = true;
        }
        /// <summary>
        /// 
        /// </summary>
        private void C_DataLoaded()
        {
            IsLoading = false;
        }
    }

}
