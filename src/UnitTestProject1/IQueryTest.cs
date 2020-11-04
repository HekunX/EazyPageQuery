using Microsoft.VisualStudio.TestTools.UnitTesting;
using EazyPageQuery;
using System.Linq;
using System;
using System.Collections.Generic;
using EazyPageQuery.Basic.QueryModel;
using EazyPageQuery.Basic;

namespace UnitTestProject1
{
    [TestClass]
    public class IQueryTest
    {
        [TestMethod]
        public void TestQueryOne()
        {
            //��ѯIdΪ1��ͬѧ�б�
            StudentQuery studentQuery = new StudentQuery
            {
                Id = 1
            };
            var students = DataStore.CreaetStudents();
            var list = students.AsQueryable().Where(studentQuery).ToList();
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(1, list[0].Id);
        }

        [TestMethod]
        public void TestQueryMany()
        {
            //��ѯ�༶IdΪ2��ͬѧ
            StudentQuery studentQuery = new StudentQuery
            {
                ClassId = 2
            };
            var students = DataStore.CreaetStudents();
            var list = students.AsQueryable().Where(studentQuery).ToList();
            Assert.AreEqual(2, list.Count);
            foreach(var item in list) Assert.AreEqual(2,item.ClassId);
        }
        [TestMethod]
        public void TestQueryMany2()
        {
            //��ѯ�༶IdΪ2����λIdΪ1��ͬѧ
            StudentQuery studentQuery = new StudentQuery
            {
                SeatId = 1,
                ClassId = 2
            };
            var students = DataStore.CreaetStudents();
            var list = students.AsQueryable().Where(studentQuery).ToList();
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(2, list[0].ClassId);
            Assert.AreEqual(1, list[0].SeatId);
        }

        [TestMethod]
        public void TestOrderBy()
        {
            //������ʱ������
            StudentOrderByCreateTimeQuery studentOrderByCreateTimeQuery = new StudentOrderByCreateTimeQuery
            {

            };
            var students = DataStore.CreaetStudents();
            var list = students.AsQueryable().OrderBy(studentOrderByCreateTimeQuery).ToList();
            var stu = students.OrderBy(x => x.CreateTime).First();
            Assert.IsTrue(stu == list[0]);
        }

        [TestMethod]
        public void TestOrderByDesc()
        {
            var stu = new Student
            {
                Id = 100,
                CreateTime = DateTime.Now
            };
            var students = DataStore.CreaetStudents();
            students[0] = stu;

            StudentOrderByCreateTimeDescQuery studentOrderByCreateTimeDescQuery = new StudentOrderByCreateTimeDescQuery
            {

            };

            var list = students.AsQueryable().OrderBy(studentOrderByCreateTimeDescQuery).ToList();
            Assert.IsTrue(stu == list.Last());
        }
        [TestMethod]
        public void TestMutiOrder()
        {
            //�Ȱ��հ༶���������У�Ȼ������λ�Ž�������
            StudentMutiOrder studentMutiOrder = new StudentMutiOrder
            {
                
            };
            var students = DataStore.CreaetStudents();
            var list1 = students.AsQueryable().OrderBy(studentMutiOrder).ToList();
            var list2 = students.OrderBy(x => x.ClassId).ThenByDescending(x => x.SeatId).ToList();
            for(int i = 0;i < list1.Count;i ++)
            {
                Assert.IsTrue(list1[i] == list2[i]);
            }
        }
        [TestMethod]
        public void QueryForAttributeTest()
        {
            //��ѯIdΪ1��ѧ��
            StudentQueryForQueryFor studentQueryForQueryFor = new StudentQueryForQueryFor
            {
                StudentId = 1
            };

            var students = DataStore.CreaetStudents();
            var stu = students.AsQueryable().Where(studentQueryForQueryFor).ToList();
            Assert.IsTrue(stu.Count == 1);
            Assert.IsTrue(stu[0].Id == 1);
        }
        [TestMethod]
        public void PageQueryTest()
        {
            StudentPageQuery studentPageQuery = new StudentPageQuery
            {
                PageSize = 10,
                CurrentPage = 1,
                Address = "���Ϻ���"
            };
            var students = DataStore.CreaetStudents();
            var page = students.AsQueryable().PageQuery(studentPageQuery);
            Assert.IsTrue(page.CurrentPage == 1 && page.PageSize == 10 && page.Total == 1 && page.Rows[0].Address == "���Ϻ���");
        }

        [TestMethod]
        public void QueryForNotFoundTest()
        {
            //��ѯIdΪ1��ѧ��
            StudentPageQueryNotFound studentQueryForQueryFor = new StudentPageQueryNotFound
            {
                StudentId = 1
            };

            var students = DataStore.CreaetStudents();
            try
            {
                var stu = students.AsQueryable().Where(studentQueryForQueryFor).ToList();
            }
            catch(PropertyNotFoundException e)
            {
                StringAssert.Contains(e.Message, "Please Check if it is a property type or the property is missing");
                return;
            }
            Assert.Fail($"The expected {nameof(PropertyNotFoundException)} was not throw. ");
        }

        [TestMethod]
        public void PageQueryParameterErrorTest()
        {
            StudentPageQuery studentPageQuery = new StudentPageQuery
            {
                PageSize = -1
            };
            var students = DataStore.CreaetStudents();
            try
            {
                var stu = students.AsQueryable().PageQuery(studentPageQuery);
            }
            catch (ArgumentException e)
            {
                StringAssert.Contains(e.Message, "PageSize can not less than one");
                return;
            }
            Assert.Fail($"The expected {nameof(ArgumentException)} was not throw. ");
        }

        [TestMethod]
        public void PageQueryParameterErrorTest2()
        {
            StudentPageQuery studentPageQuery = new StudentPageQuery
            {
                CurrentPage = -1
            };
            var students = DataStore.CreaetStudents();
            try
            {
                var stu = students.AsQueryable().PageQuery(studentPageQuery);
            }
            catch (ArgumentException e)
            {
                StringAssert.Contains(e.Message, "CurrentPage can not less than one");
                return;
            }
            Assert.Fail($"The expected {nameof(ArgumentException)} was not throw. ");
        }
        [TestMethod]
        public void LikeTset()
        {
            LikeQuery likeQuery = new LikeQuery
            {
                Address = "����"
            };
            var students = DataStore.CreaetStudents();
            var page = students.AsQueryable().PageQuery(likeQuery);
            Assert.IsTrue(page.Total == 2 && page.Rows.All(x => x.Address.Contains("����")));
        }

        [TestMethod]
        public void ChoiceOrderTest()
        {
            ChoiceOrderQuery choiceOrderQuery = new ChoiceOrderQuery
            {
                SeatIdIsAsc = false
            };
            var students = DataStore.CreaetStudents();
            var page = students.AsQueryable().PageQuery(choiceOrderQuery);
            Assert.IsTrue(page.Rows.Count == 3 && page.Total == 3 && page.Rows[0].SeatId == 2);
        }

        [TestMethod]
        public void EXISTSINTest()
        {
            EXISTSINQuery existsInQuery = new EXISTSINQuery
            {
                ClassIds = new List<int> { 1}
            };
            var students = DataStore.CreaetStudents();
            var page = students.AsQueryable().PageQuery(existsInQuery);
            page = students.AsQueryable().PageQuery(existsInQuery);
            Assert.IsTrue(page.Rows.Count == 1 && page.Total == 1 && page.Rows[0].SeatId == 1);

            existsInQuery.ClassIds = new List<int> { 2};
            page = students.AsQueryable().PageQuery(existsInQuery);
            Assert.IsTrue(page.Rows.Count == 2 && page.Total == 2);
        }

        [TestMethod]
        public void JudgeTypeTest()
        {
            DynamicJudgeTypeQuery judgeTypeQuery = new DynamicJudgeTypeQuery
            {
                Id = new EazyJudgeValue<int>
                {
                    Value = 1
                }
            };

            var students = DataStore.CreaetStudents();

            judgeTypeQuery.Id.JudgeType = JudgeType.eq;
            var page = students.AsQueryable().PageQuery(judgeTypeQuery);
            Assert.IsTrue(page.Rows.Count == 1 && page.Total == 1);

            judgeTypeQuery.Id.JudgeType = JudgeType.ge;
            page = students.AsQueryable().PageQuery(judgeTypeQuery);
            Assert.IsTrue(page.Rows.Count == 3 && page.Total == 3);

            judgeTypeQuery.Id.JudgeType = JudgeType.gt;
            page = students.AsQueryable().PageQuery(judgeTypeQuery);
            Assert.IsTrue(page.Rows.Count == 2 && page.Total == 2);

            judgeTypeQuery.Id.JudgeType = JudgeType.le;
            page = students.AsQueryable().PageQuery(judgeTypeQuery);
            Assert.IsTrue(page.Rows.Count == 1 && page.Total == 1);

            judgeTypeQuery.Id.JudgeType = JudgeType.lt;
            page = students.AsQueryable().PageQuery(judgeTypeQuery);
            Assert.IsTrue(page.Rows.Count == 0 && page.Total == 0);

            judgeTypeQuery.Id.JudgeType = JudgeType.ne;
            page = students.AsQueryable().PageQuery(judgeTypeQuery);
            Assert.IsTrue(page.Rows.Count == 2 && page.Total == 2);

            ErrorJudgeTypeQuery errorJudgeTypeQuery = new ErrorJudgeTypeQuery() { };
            try
            {
                page = students.AsQueryable().PageQuery(errorJudgeTypeQuery);

            }
            catch(ArgumentNullException e)
            {
                StringAssert.Contains(e.Message,"Id");
                return;
            }
            Assert.Fail($"The expected {nameof(PropertyTypeErrorException)} was not throw. ");
        }
        [TestMethod]

        public void JudgeTypeTest2()
        {
            var students = DataStore.CreaetStudents();

            ErrorJudgeTypeQuery errorJudgeTypeQuery = new ErrorJudgeTypeQuery() 
            {
                Id = "1"
            };
            try
            {
               var page = students.AsQueryable().PageQuery(errorJudgeTypeQuery);

            }
            catch (PropertyTypeErrorException e)
            {
                StringAssert.Contains(e.Message, "String");
                return;
            }
            Assert.Fail($"The expected {nameof(PropertyTypeErrorException)} was not throw. ");
        }

        [TestMethod]
        public void StaticJudgeTypeQueryTest()
        {
            StaticJudgeTypeQuery staticJudgeTypeQuery = new StaticJudgeTypeQuery
            {
                Id = 10
            };

            var students = DataStore.CreaetStudents();
            var page = students.AsQueryable().PageQuery(staticJudgeTypeQuery);

            Assert.IsTrue(page.Rows.Count == 0 && page.Total == 0) ;
        }

        [TestMethod]
        public void StaticJudgeTypeQuery_NotFindErrorTest()
        {
            StaticJudgeTypeQuery_NotFindError staticJudgeTypeQuery_NotFindError = new StaticJudgeTypeQuery_NotFindError { };
            var students = DataStore.CreaetStudents();
            try
            {
                var page = students.AsQueryable().PageQuery(staticJudgeTypeQuery_NotFindError);
            }
            catch(PropertyNotFoundException e)
            {
                return;
            }

            Assert.Fail(GetNotThrowString(nameof(PropertyNotFoundException)));
        }

        public static string GetNotThrowString(string exceptionTypeName) => $"The expected {exceptionTypeName} was not throw. ";
    }
}
