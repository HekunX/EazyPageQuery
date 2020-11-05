using EazyPageQuery;
using EazyPageQuery.Basic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp1
{
    class Program
    {
        
        public static List<Student> GetStudents()
        {
            List<Student> students = new List<Student>()
            {
                new Student
                {
                    Id = 1,
                    Address = "湖南衡阳",
                    ClassId = 1,
                    CreateTime = DateTime.Now,
                    Name = "张三",
                    SeatId = 1
                },
                new Student
                {
                    Id = 2,
                    Address = "湖北荆州",
                    ClassId = 2,
                    CreateTime = DateTime.Now,
                    Name = "李四",
                    SeatId = 1
                },
                new Student
                {
                    Id = 3,
                    Address = "湖北荆州",
                    ClassId = 2,
                    CreateTime = DateTime.Now,
                    Name = "王五",
                    SeatId = 2
                },
            };
            return students;
        }
        static void Main(string[] args)
        {
            var dataSotre = new DataStore();
            if(dataSotre.Students.Count() == 0)
            {
                dataSotre.Students.AddRange(GetStudents());
                dataSotre.SaveChanges();
            }

            var students = dataSotre.Students;
            StudentPageQuery studentPageQuery = new StudentPageQuery
            {
                PageSize = 10,
                CurrentPage = 1,
                Id = new EazyPageQuery.Basic.QueryModel.EazyJudgeValue<int> 
                {
                    JudgeType = JudgeType.ge,
                    Value = 1
                }
            };
            Page<Student> page = students.PageQuery(studentPageQuery);
        }
    }
}
