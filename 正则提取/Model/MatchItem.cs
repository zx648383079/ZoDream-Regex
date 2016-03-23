using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 正则提取.Model
{
    public class MatchItem
    {
        public int Tag { get; set; }

        public string Content { get; set; }

        public MatchItem()
        {
            
        }

        public MatchItem(int tag, string content)
        {
            Tag = tag;
            Content = content;
        }
    }
}
