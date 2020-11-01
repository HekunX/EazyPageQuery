using EazyPageQuery.Basic;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTestProject1
{
    public class StudentQuery:IQuery
    {
        public int? Id { get; set; }
        public int? ClassId { get; set; }
        public int? SeatId { get; set; }
        public string Name { get; set; }
        public DateTime? CreateTime { get; set; }
        public string Address { get; set; }
    }

    public class StudentOrderByCreateTimeQuery:IQuery,IOrder
    {
        public int? Id { get; set; }
        public int? ClassId { get; set; }
        public int? SeatId { get; set; }
        public string Name { get; set; }
        [OrderBy]
        public DateTime? CreateTime { get; set; }
        public string Address { get; set; }
    }

    public class StudentOrderByCreateTimeDescQuery : IQuery, IOrder
    {
        public int? Id { get; set; }
        public int? ClassId { get; set; }
        public int? SeatId { get; set; }
        public string Name { get; set; }
        [OrderBy(Order = 0,OrderType = OrderType.Descending)]
        public DateTime? CreateTime { get; set; }
        public string Address { get; set; }
    }

    public class StudentMutiOrder:IQuery,IOrder
    {
        public int? Id { get; set; }
        [OrderBy(Order = 0, OrderType = OrderType.Ascending)]
        public int? ClassId { get; set; }
        [OrderBy(Order = 1, OrderType = OrderType.Descending)]
        public int? SeatId { get; set; }
        public string Name { get; set; }

        public DateTime? CreateTime { get; set; }
        public string Address { get; set; }
    }

    public class StudentQueryForQueryFor : IQuery
    {
        [QueryFor("Id")]
        public int? StudentId { get; set; }
        public int? ClassId { get; set; }
        public int? SeatId { get; set; }
        public string Name { get; set; }
        public DateTime? CreateTime { get; set; }
        public string Address { get; set; }
    }

    public class StudentPageQuery : PageQuery
    {
        [QueryFor("Id")]
        public int? StudentId { get; set; }
        public int? ClassId { get; set; }
        public int? SeatId { get; set; }
        public string Name { get; set; }
        [OrderBy(Order = 0,OrderType = OrderType.Descending)]
        public DateTime? CreateTime { get; set; }
        public string Address { get; set; }

    }

    public class StudentPageQueryNotFound : PageQuery
    {
        [QueryFor("test")]
        public int? StudentId { get; set; }
        public int? ClassId { get; set; }
        public int? SeatId { get; set; }
        public string Name { get; set; }
        [OrderBy(Order = 0, OrderType = OrderType.Descending)]
        public DateTime? CreateTime { get; set; }
        public string Address { get; set; }
    }

    public class LikeQuery : PageQuery
    {
        [QueryFor("Id")]
        public int? StudentId { get; set; }
        public int? ClassId { get; set; }
        public int? SeatId { get; set; }
        public string Name { get; set; }
        [OrderBy(Order = 0, OrderType = OrderType.Descending)]
        public DateTime? CreateTime { get; set; }
        [Like]
        public string Address { get; set; }
    }


    public class ChoiceOrderQuery : PageQuery
    {
        [QueryFor("Id")]
        public int? StudentId { get; set; }
        public int? ClassId { get; set; }
        public int? SeatId { get; set; }
        public string Name { get; set; }
        public DateTime? CreateTime { get; set; }
        [Like]
        public string Address { get; set; }
        [OrderChoice(Order = 0)]
        public bool SeatIdIsAsc { get; set; }
    }
}
