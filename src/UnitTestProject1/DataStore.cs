using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTestProject1
{
    public static class DataStore
    {


        public static List<Student> CreaetStudents() => Create();



        static List<Student> Create()
        {
            List<Student> students = new List<Student>();
            students.Add(new Student
            {
                Id = 1,
                ClassId = 1,
                SeatId = 1,
                Name = "何坤",
                Address = "湖南衡阳",
                CreateTime = DateTime.Now,
            });

            students.Add(new Student
            {
                Id = 2,
                ClassId = 2,
                SeatId = 1,
                Name = "范易凡",
                Address = "湖北荆州",
                CreateTime = DateTime.Now,
            });

            students.Add(new Student
            {
                Id = 3,
                Name = "吴惠格",
                ClassId = 2,
                SeatId = 2,
                Address = "湖北荆州",
                CreateTime = DateTime.Now,
            });

            return students;
        }
    }
}
