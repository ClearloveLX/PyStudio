using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PyStudio.Web.Extends
{

    #region 分页处理
    /// <summary>
    /// 分页Option属性
    /// </summary>
    public class PyPagerOption
    {
        /// <summary>
        /// 当前页 *
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// 总条数 *
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// 每页条数  默认15条
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 路由地址 /Controller/Action
        /// </summary>
        public string RouteUrl { get; set; }

        /// <summary>
        /// 分页样式，默认bootstrap
        /// </summary>
        public int StyleNum { get; set; }

        /// <summary>
        /// 地址与分页数拼接符
        /// </summary>
        [Obsolete]
        public string JoinOperateCode { get; set; }
        /// <summary>
        /// 页面传值
        /// </summary>
        public string PageParameter { get; set; }
    }

    /// <summary>
    /// PC分页标签
    /// </summary>
    public class PagerTagHelper : TagHelper
    {
        public PyPagerOption PagerOption { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";

            if (PagerOption.PageSize <= 0) { PagerOption.PageSize = 15; }
            if (PagerOption.CurrentPage <= 0) { PagerOption.CurrentPage = 1; }
            if (PagerOption.Total <= 0) { return; }
            if (!string.IsNullOrEmpty(PagerOption.PageParameter)) { PagerOption.PageParameter = "?" + PagerOption.PageParameter; }

            //总页数
            var totalPage = PagerOption.Total / PagerOption.PageSize + (PagerOption.Total % PagerOption.PageSize > 0 ? 1 : 0);
            if (totalPage <= 0) { return; }
            else if (totalPage <= PagerOption.CurrentPage)
            {
                PagerOption.CurrentPage = totalPage;
            }

            //当前路由地址
            if (string.IsNullOrEmpty(PagerOption.RouteUrl))
            {
                if (!string.IsNullOrEmpty(PagerOption.RouteUrl))
                {
                    var listIndex = PagerOption.RouteUrl.LastIndexOf("/");
                    PagerOption.RouteUrl = PagerOption.RouteUrl.Substring(0, listIndex);
                }
            }

            //构造分页样式
            var sbPage = new StringBuilder(string.Empty);

            switch (PagerOption.StyleNum)
            {
                case 2:
                    {
                        break;
                    }

                default:
                    {
                        #region 默认样式
                        PagerOption.RouteUrl = PagerOption.RouteUrl.TrimEnd('/');
                        sbPage.Append("<nav>");
                        sbPage.Append("  <ul class=\"pagination\">");
                        //返回首页/第一页（第一页时禁用）
                        sbPage.AppendFormat("       <li class={3}><a href=\"{0}{2}{1}{4}\" aria-label=\"First\"><span aria-hidden=\"true\">&laquo;</span></a></li>",
                                                PagerOption.RouteUrl,
                                                PagerOption.CurrentPage - 1 <= 0 ? 1 : PagerOption.CurrentPage - 1,
                                                "/",
                                                PagerOption.CurrentPage - 1 <= 0 ? "disabled" : "",
                                                PagerOption.PageParameter);
                        //上一页（第一页时禁用）
                        sbPage.AppendFormat("       <li class={3}><a href=\"{0}{2}{1}{4}\" aria-label=\"Previous\"><i class=\"fa-angle-left\" aria-hidden=\"true\"></i></a></li>",
                                                PagerOption.RouteUrl,
                                                1,
                                                "/",
                                                PagerOption.CurrentPage - 1 <= 0 ? "disabled" : "",
                                                PagerOption.PageParameter);
                        #region 判断分页省略
                        //小于20页不做省略
                        if (totalPage <= 20)
                        {
                            for (int i = 1; i <= totalPage; i++)
                            {
                                sbPage.AppendFormat("       <li {1}><a href=\"{2}{3}{0}{4}\">{0}</a></li>",
                                    i,
                                    i == PagerOption.CurrentPage ? "class=\"active\"" : "",
                                    PagerOption.RouteUrl,
                                    "/",
                                    PagerOption.PageParameter);
                            }
                        }
                        else
                        {
                            //尾部省略，保留前十位
                            if ((PagerOption.CurrentPage - 6) <= 0)
                            {
                                for (int i = 1; i <= 10; i++)
                                {
                                    sbPage.AppendFormat("       <li {1}><a href=\"{2}{3}{0}{4}\">{0}</a></li>",
                                    i,
                                    i == PagerOption.CurrentPage ? "class=\"active\"" : "",
                                    PagerOption.RouteUrl,
                                    "/", PagerOption.PageParameter);
                                }
                                sbPage.Append("<li class=\"disabled\"><a aria-label=\"Home\"><span aria-hidden=\"true\"><strong>...</strong></span></a></li>");
                            }
                            //前段省略，保留后十位
                            else if ((PagerOption.CurrentPage + 6) >= totalPage)
                            {
                                sbPage.Append("<li class=\"disabled\"><a aria-label=\"Previous\"><span aria-hidden=\"true\"><strong>...</strong></span></a></li>");
                                for (int i = totalPage - 10; i <= totalPage; i++)
                                {
                                    sbPage.AppendFormat("       <li {1}><a href=\"{2}{3}{0}{4}\">{0}</a></li>",
                                    i,
                                    i == PagerOption.CurrentPage ? "class=\"active\"" : "",
                                    PagerOption.RouteUrl,
                                    "/",
                                    PagerOption.PageParameter);
                                }
                            }
                            //中间保留七位，首尾省略
                            else
                            {
                                sbPage.Append("<li class=\"disabled\"><a aria-label=\"Previous\"><span aria-hidden=\"true\"><strong>...</strong></span></a></li>");
                                for (int i = PagerOption.CurrentPage - 3; i >= PagerOption.CurrentPage - 3 && i < PagerOption.CurrentPage + 4; i++)
                                {
                                    sbPage.AppendFormat("       <li {1}><a href=\"{2}{3}{0}{4}\">{0}</a></li>",
                                    i,
                                    i == PagerOption.CurrentPage ? "class=\"active\"" : "",
                                    PagerOption.RouteUrl,
                                    "/",
                                    PagerOption.PageParameter);
                                }
                                sbPage.Append("<li class=\"disabled\"><a aria-label=\"Previous\"><span aria-hidden=\"true\"><strong>...</strong></span></a></li>");
                            }
                        }
                        #endregion

                        //下一页（最后一页时禁用）
                        sbPage.AppendFormat("       <li class={3}><a href=\"{0}{2}{1}{4}\" aria-label=\"Next\"><i class=\"fa-angle-right\" aria-hidden=\"true\"></i></a></li>",
                                            PagerOption.RouteUrl,
                                            PagerOption.CurrentPage + 1 > totalPage ? PagerOption.CurrentPage : PagerOption.CurrentPage + 1,
                                            "/",
                                            PagerOption.CurrentPage + 1 >= totalPage ? "disabled" : "",
                                            PagerOption.PageParameter);
                        //尾页/最后一页（最后一页时禁用）
                        sbPage.AppendFormat("       <li class={3}><a href=\"{0}{2}{1}{4}\" aria-label=\"Last\"><span aria-hidden=\"true\">&raquo;</span></a></li>",
                                            PagerOption.RouteUrl,
                                            totalPage,
                                            "/",
                                            PagerOption.CurrentPage + 1 >= totalPage ? "disabled" : "",
                                            PagerOption.PageParameter);
                        sbPage.Append("   </ul>");
                        sbPage.Append($"<div style=\"float: right;margin: 25px;\"><span>共{totalPage}页/{PagerOption.Total}条</span></div>");
                        sbPage.Append("</nav>");

                        #endregion
                    }
                    break;
            }
            output.Content.SetHtmlContent(sbPage.ToString());
        }
    }

    /// <summary>
    /// Moblie分页标签
    /// </summary>
    public class MobliePagerTagHelper : TagHelper
    {
        public PyPagerOption PagerOption { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";

            if (PagerOption.PageSize <= 0) { PagerOption.PageSize = 15; }
            if (PagerOption.CurrentPage <= 0) { PagerOption.CurrentPage = 1; }
            if (PagerOption.Total <= 0) { return; }

            //总页数
            var totalPage = PagerOption.Total / PagerOption.PageSize + (PagerOption.Total % PagerOption.PageSize > 0 ? 1 : 0);
            if (totalPage <= 0) { return; }
            else if (totalPage <= PagerOption.CurrentPage)
            {
                PagerOption.CurrentPage = totalPage;
            }

            //当前路由地址
            if (string.IsNullOrEmpty(PagerOption.RouteUrl))
            {
                if (!string.IsNullOrEmpty(PagerOption.RouteUrl))
                {
                    var listIndex = PagerOption.RouteUrl.LastIndexOf("/");
                    PagerOption.RouteUrl = PagerOption.RouteUrl.Substring(0, listIndex);
                }
            }

            //构造分页样式
            var sbPage = new StringBuilder(string.Empty);

            switch (PagerOption.StyleNum)
            {
                case 2:
                    {
                        break;
                    }

                default:
                    {
                        #region 默认样式
                        sbPage.Append("<nav>");
                        sbPage.Append("  <ul class=\"pagination\">");
                        //返回首页/第一页（第一页时禁用）
                        sbPage.AppendFormat("       <li class={3}><a href=\"{0}{2}{1}{4}\" aria-label=\"First\"><span aria-hidden=\"true\">&laquo;</span></a></li>",
                                                PagerOption.RouteUrl,
                                                PagerOption.CurrentPage - 1 <= 0 ? 1 : PagerOption.CurrentPage - 1,
                                                "/",
                                                PagerOption.CurrentPage - 1 <= 0 ? "disabled" : "",
                                                PagerOption.PageParameter);
                        //上一页（第一页时禁用）
                        sbPage.AppendFormat("       <li class={3}><a href=\"{0}{2}{1}{4}\" aria-label=\"Previous\"><i class=\"fa-angle-left\" aria-hidden=\"true\"></i></a></li>",
                                                PagerOption.RouteUrl,
                                                1,
                                                "/",
                                                PagerOption.CurrentPage - 1 <= 0 ? "disabled" : "",
                                                PagerOption.PageParameter);


                        //下一页（最后一页时禁用）
                        sbPage.AppendFormat("       <li class={3}><a href=\"{0}{2}{1}{4}\" aria-label=\"Next\"><i class=\"fa-angle-right\" aria-hidden=\"true\"></i></a></li>",
                                            PagerOption.RouteUrl,
                                            PagerOption.CurrentPage + 1 > totalPage ? PagerOption.CurrentPage : PagerOption.CurrentPage + 1,
                                            "/",
                                            PagerOption.CurrentPage + 1 >= totalPage ? "disabled" : "",
                                            PagerOption.PageParameter);
                        //尾页/最后一页（最后一页时禁用）
                        sbPage.AppendFormat("       <li class={3}><a href=\"{0}{2}{1}{4}\" aria-label=\"Last\"><span aria-hidden=\"true\">&raquo;</span></a></li>",
                                            PagerOption.RouteUrl,
                                            totalPage,
                                            "/",
                                            PagerOption.CurrentPage + 1 >= totalPage ? "disabled" : "",
                                            PagerOption.PageParameter);
                        sbPage.Append("   </ul>");
                        sbPage.Append($"<div style=\"float: right;margin: 25px;\"><span>共{totalPage}页/{PagerOption.Total}条</span></div>");
                        sbPage.Append("</nav>");

                        #endregion
                    }
                    break;
            }
            output.Content.SetHtmlContent(sbPage.ToString());
        }
    }

    #endregion
}
