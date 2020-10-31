using EazyPageQuery;
using EazyPageQuery.Basic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Student> students = DataStore.CreaetStudents();
            StudentPageQuery studentPageQuery = new StudentPageQuery
            {
                PageSize = 10,
                CurrentPage = 2,
            };
            Page<Student> page = students.AsQueryable().PageQeury(studentPageQuery);
        }
    }
}
