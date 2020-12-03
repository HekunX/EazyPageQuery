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
        gt = 0,
        /// <summary>
        /// 等于(equal to)
        /// </summary>
        eq = 1,
        /// <summary>
        /// 小于(less than)
        /// </summary>
        lt = 2,
        /// <summary>
        /// 小于等于(less than or equal to)
        /// </summary>
        le = 3,
        /// <summary>
        /// 不等于(not equal to)
        /// </summary>
        ne = 4,
        /// <summary>
        /// 大于或等于 (greater than or equal to)
        /// </summary>
        ge = 5
    }
    public class JudgeAttribute:Attribute
    {
        public JudgeType JudgeType { get; set; }
    }

    public class DynamicJudgeAttribute:Attribute
    {

    }
}
