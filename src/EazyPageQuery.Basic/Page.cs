using System;
using System.Collections.Generic;

namespace EazyPageQuery.Basic
{
    public class Page<T>
    {
        /// <summary>
        /// 页大小
        /// </summary>
        public int PageSize { get; private set; }
        /// <summary>
        /// 当前页
        /// </summary>
        public int CurrentPage { get; private set; }
        /// <summary>
        /// 总数
        /// </summary>
        public int Total { get; private set; }
        /// <summary>
        /// 当前页数据
        /// </summary>
        public List<T> Rows { get; private set; }

        public Page(PageQuery pageQuery, List<T> rows, int total)
        {
            PageSize = pageQuery.PageSize;
            CurrentPage = pageQuery.CurrentPage;
            Rows = rows;
            Total = total;
        }
    }
}
