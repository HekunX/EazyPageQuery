using System;
using System.Collections.Generic;
using System.Text;

namespace EazyPageQuery.Basic
{
    public class PageQuery:IQuery,IOrder
    {
        /// <summary>
        /// 页大小
        /// </summary>
        [NoQuery]
        public int PageSize { get; set; }
        /// <summary>
        /// 当前页
        /// </summary>
        [NoQuery]
        public int CurrentPage { get; set; }
        public PageQuery()
        {
            PageSize = 20;
            CurrentPage = 1;
        }
    }
}
