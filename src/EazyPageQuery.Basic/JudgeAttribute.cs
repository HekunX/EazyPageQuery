using System;
using System.Collections.Generic;
using System.Text;

namespace EazyPageQuery.Basic
{
    /// <summary>
    /// 如何判断数据类型
    /// </summary>
    public enum JudgeType : int
    {

        /// <summary>
        /// 大于(greater than)
        /// </summary>
        gt,
        /// <summary>
        /// 等于(equal to)
        /// </summary>
        eq,
        /// <summary>
        /// 小于(less than)
        /// </summary>
        lt,
        /// <summary>
        /// 小于等于(less than or equal to)
        /// </summary>
        le,
        /// <summary>
        /// 不等于(not equal to)
        /// </summary>
        ne,
        /// <summary>
        /// 大于或等于 (greater than or equal to)
        /// </summary>
        ge
    }
    public class JudgeAttribute:Attribute
    {
        public JudgeType JudgeType { get; set; }
    }

    public class DynamicJudgeAttribute:Attribute
    {

    }
}
