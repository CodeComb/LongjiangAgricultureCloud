using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LongjiangAgricultureCloud.Helpers
{
    public class PagerInfo
    {
        public int PageSize { get; set; }
        public int PageCount { get; set; }
        public int Start { get; set; }
        public int End { get; set; }
    }

    public static class PagerHelper
    {
        public static  PagerInfo Do<T>(ref IEnumerable<T> src, int PageSize, int Page = 1)
        {
            var ret = new PagerInfo();
            ret.PageCount = (src.Count() + PageSize - 1) / PageSize;
            ret.PageSize = PageSize;
            ret.Start = (Page - PageSize) < 1 ? 1 : (Page - PageSize);
            ret.End = (ret.Start + 10) > ret.PageCount ? ret.PageCount : (ret.Start + 10);
            if (ret.End < ret.Start) ret.End = ret.Start;
            src = src.Skip((Page - 1) * PageSize).Take(PageSize).ToList();
            return ret;
        }
    }
}