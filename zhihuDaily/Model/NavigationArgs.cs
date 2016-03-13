using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zhihuDaily.Model
{
    public class NavigationArgs
    {
        public object ClickItem { get; set; }
        public List<string> CurrentList { get; set; } = new List<string>();
        public int CurrentIndex { get; set; }
        
    }
}
