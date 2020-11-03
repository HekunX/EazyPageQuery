using EazyPageQuery;
using EazyPageQuery.Basic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Test
{

    public class Test
    {
        public int Id { get; set; }
        public int Id2 { get; set; }

        public string Name { get; set; }
    }

    public class TestPageQuery:PageQuery
    {
        [EXISTSIN]
        public List<int> Ids { get; set; }
        public int? Id { get; set; }
        public int? Id2 { get; set; }
        [Like]
        public string? Name { get; set; }
    }

    public class TestQuery:IQuery,IOrder
    {
        [OrderBy]
        public int? Id { get; set; }
        [OrderBy(Order = 1,OrderType = OrderType.Ascending)]
        public int? Id2 { get; set; }
        [Like]
        public string? Name { get; set; }
    }
    class Program
    {
        static void Main(string[] args)
        {
            List<Test> tests = new List<Test> { };
            tests.Add(new Test 
            {
                Id = 1,
                Id2 = 1,
                Name = "hk"
            });
            tests.Add(new Test 
            {
                Id = 1,
                Id2 = 2,
                Name = "fyf"
            });
            tests.Add(new Test
            {
                Id = 2,
                Id2 = 2,
                Name = "zxc"
            });

            TestPageQuery query = new TestPageQuery
            {
                Ids = new List<int> { 2},
            };


            var res = tests.AsQueryable().PageQuery(query);


        }
    }
}
