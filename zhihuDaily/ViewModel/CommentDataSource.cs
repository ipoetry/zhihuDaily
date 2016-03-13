using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using zhihuDaily.DataService;
using zhihuDaily.Model;

namespace zhihuDaily.ViewModel
{
    class CommentDataSource : DataSourceBase<Comment>
    {
        ICommonService<Comment> _commentService;
        public CommentDataSource(ICommonService<Comment> commentService)
        {
            _commentService = commentService;
        }

        protected override Task<IList<Comment>> LoadItemsAsync()
        {
            throw new NotImplementedException();
        }
    }
}
