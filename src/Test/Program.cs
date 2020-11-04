using EazyPageQuery;
using EazyPageQuery.Basic;
using EazyPageQuery.Basic.QueryModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using UnitTestProject1;

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
        [DynamicJudge]
        public EazyJudgeValue<int> Id { get; set; }

        public int? Id2 { get; set; }

        public string? Name { get; set; }
    }
    class Program
    {
        static void Main(string[] args)
        {
            EXISTSINQuery existsInQuery = new EXISTSINQuery
            {
                ClassIds = new List<int> { 1 }
            };
            var students = DataStore.CreaetStudents();
            var page = students.AsQueryable().PageQuery(existsInQuery);
            page = students.AsQueryable().PageQuery(existsInQuery);
            Assert.IsTrue(page.Rows.Count == 1 && page.Total == 1 && page.Rows[0].SeatId == 1);

            existsInQuery.ClassIds = new List<int> { 2 };
            page = students.AsQueryable().PageQuery(existsInQuery);
            Assert.IsTrue(page.Rows.Count == 2 && page.Total == 2);


        }
    }
}
