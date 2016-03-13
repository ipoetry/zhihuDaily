﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace zhihuDaily.ViewModel
{
    public delegate void OnDataRequestError(int code);

    /// <summary>
    /// datasource base for app that enabled incremental loading (page based)
    /// </summary>
    public abstract class DataSourceBase<T> : IncrementalLoadingBase<T>
    {
        /// <summary>
        /// the refresh will clear current items, and re-fetch from beginning, so that we will keep a correct page number
        /// </summary>
        public async virtual Task Refresh()
        {
            //reset
            this._currentPage = 1;
            this._hasMoreItems = true;

            this.Clear();
            await LoadMoreItemsAsync(20);
        }

        protected DateTime _lastTime = DateTime.MinValue;

        protected virtual bool IsInTime()
        {
            var delta = DateTime.Now - _lastTime;
            _lastTime = DateTime.Now;
            return delta.TotalMilliseconds < 500;
        }

        /// <summary>
        /// if their items are paged,use it
        /// </summary>
        protected override async Task<IList<T>> LoadMoreItemsOverrideAsync(CancellationToken c, uint count)
        {
            if (IsInTime())
            {
                return null;
            }

            var newItems = await this.LoadItemsAsync();

            //update page state
            if (newItems != null)
            {
                _currentPage++;
            }

            //we check if there is any new post
            this._hasMoreItems = newItems != null && newItems.Count > 0;

            return newItems;
        }

        protected void FireErrorEvent(int code)
        {
            if (this.DataRequestError != null)
            {
                this.DataRequestError(code);
            }
        }

        public event OnDataRequestError DataRequestError;

        protected override bool HasMoreItemsOverride()
        {
            return _hasMoreItems;
        }

        protected abstract Task<IList<T>> LoadItemsAsync();

        protected int _currentPage = 1;
        protected bool _hasMoreItems = true;
    }
}
