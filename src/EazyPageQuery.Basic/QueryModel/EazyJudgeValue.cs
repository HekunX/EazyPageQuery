using System;
using System.Collections.Generic;
using System.Text;

namespace EazyPageQuery.Basic.QueryModel
{
    public class EazyJudgeValue<T>
    {
        public T Value { get; set; }
        public JudgeType JudgeType { get; set; }
    }
}
