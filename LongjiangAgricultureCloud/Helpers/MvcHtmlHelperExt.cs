using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LongjiangAgricultureCloud.Helpers
{
    public static class MvcHtmlHelperExt
    {
        public static MvcHtmlString ToContestStatus<TModel>(this HtmlHelper<TModel> self, DateTime begin, DateTime? rest_begin, DateTime? rest_end, DateTime end)
        {
            return new MvcHtmlString(Time.ToContestStatus(begin, rest_begin, rest_end, end));
        }

        public static MvcHtmlString ToTimeTip<TModel>(this HtmlHelper<TModel> self, DateTime time)
        {
            return new MvcHtmlString(Time.ToTimeTip(time));
        }

        public static MvcHtmlString ToTimeLength<TModel>(this HtmlHelper<TModel> self, DateTime time1, DateTime time2)
        {
            return new MvcHtmlString(Time.ToTimeLength(time1, time2));
        }

        public static MvcHtmlString ToVagueTimeLength<TModel>(this HtmlHelper<TModel> self, DateTime time1, DateTime time2)
        {
            return new MvcHtmlString(Time.ToVagueTimeLength(time1, time2));
        }

        public static MvcHtmlString Sanitized<TModel>(this HtmlHelper<TModel> self, string html)
        {
            if (html == null) return new MvcHtmlString("");
            return new MvcHtmlString(HtmlFilter.Instance.SanitizeHtml(html));
        }

        public static MvcHtmlString ToTimeStamp<TModel>(this HtmlHelper<TModel> self, DateTime time)
        {
            return new MvcHtmlString(Helpers.Time.ToTimeStamp(time).ToString());
        }

        public static MvcHtmlString MakePager<TModel>(this HtmlHelper<TModel> self)
        {
            var tmp = (PagerInfo)self.ViewBag.PageInfo;
            var p = Convert.ToInt32(self.ViewContext.HttpContext.Request.QueryString["p"]);
            var html = "<div class=\"pager\">";
            for (var i = tmp.Start; i <= tmp.End; i++)
            {
                var str = "?p=" + i;
                foreach (string k in self.ViewContext.HttpContext.Request.QueryString.Keys)
                {
                    if (k == "p") continue;
                    str += "&" + k + "=" + self.ViewContext.HttpContext.Request.QueryString[k];
                }
                html += "<a class=\"pager-item\" href=\"" + str + "\">" + i + "</a>";
            }
            html += "</div>";
            return new MvcHtmlString(html);
        }
    }
}