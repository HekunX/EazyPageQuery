using EazyPageQuery.Basic;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1
{
    public class StudentPageQuery:PageQuery
    {
        public int? Id { get; set; }
        public int? ClassId { get; set; }
        public int? SeatId { get; set; }
        public string Name { get; set; }
        public DateTime? CreateTime { get; set; }
        [Like]
        public string Address { get; set; }
        [OrderChoice]
        public bool IdIsAsc { get; set; }
    }
}
