using EazyPageQuery.Basic;
using EazyPageQuery.Basic.QueryModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1
{
    public class StudentPageQuery:PageQuery
    {
        [EXISTSIN]
        public IEnumerable<int> Ids { get; set; }
        public int? ClassId { get; set; }
        public int? SeatId { get; set; }
        public string Name { get; set; }
        public DateTime? CreateTime { get; set; }
        public string Address { get; set; }
    }
}
