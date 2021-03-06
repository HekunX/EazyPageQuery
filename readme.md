# 目录 <!-- omit in toc -->
- [EazyPageQuery（中文文档）](#eazypagequery中文文档)
  - [特点](#特点)
  - [开始使用](#开始使用)
    - [1.首先创建一个实体类](#1首先创建一个实体类)
    - [2.创建一个数据仓储](#2创建一个数据仓储)
  - [3.简单例子](#3简单例子)
    - [1. 创建查询实体类](#1-创建查询实体类)
    - [2.过滤查询](#2过滤查询)
    - [3.排序查询](#3排序查询)
    - [4.多重排序查询](#4多重排序查询)
    - [5.模糊查询](#5模糊查询)
    - [5.分页查询](#5分页查询)
    - [6.动态排序分页查询](#6动态排序分页查询)
    - [7.集合查询](#7集合查询)
    - [8.静态范围查询](#8静态范围查询)
    - [9.动态范围查询](#9动态范围查询)
- [默认约定参照表](#默认约定参照表)
- [作者](#作者)
# EazyPageQuery（中文文档）

Create The Page-Query Command easily For EntityFramework and EF Core!

## 特点
- 简单使用，只需使用几个特性便可，无需任何配置类。
- 方便扩展，对原有代码入侵性小。
- 只需一个查询实体类，便可快速生成分页查询接口，支持模糊查询、排序查询、多重排序查询，动态排序查询。
  
## 开始使用
在Nuget包管理器上搜索`EazyPageQuery`，然后安装即可。
| NuGet | Status|
|--| -- |
|[![](https://img.shields.io/nuget/v/EazyPageQuery)](https://www.nuget.org/packages/EazyPageQuery)| ![example workflow name](https://github.com/HekunX/EazyPageQuery/workflows/.NET%20Core/badge.svg?branch=main)|
***
&emsp;&emsp;若想快速上手，使用如下命令安装Demo模板即可。
```Shell
dotnet new -i EazyPageQuery.Demo.Templates
```
&emsp;&emsp;然后在您想要创建解决方案的目录下使用如下命令
```Shell
dotnet new EazyDemo
```
&emsp;&emsp;等待模板安装完毕，在当前目录下，使用如下命令运行Demo
```Shell
dotnet run --project .\Demo\
```
&emsp;&emsp;默认监听`http://localhost:5000`，访问该地址将跳转到SwaggerUI页面。
***
### 1.首先创建一个实体类
```C#
    public class Student
    {
        public int Id { get; set; }
        public int ClassId { get; set; }
        public int SeatId { get; set; }
        public string Name { get; set; }
        public DateTime CreateTime { get; set; }
        public string Address { get; set; }
    }
```
### 2.创建一个数据仓储
```C#
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
                Name = "张三",
                Address = "湖南衡阳",
                CreateTime = DateTime.Now,
            });

            students.Add(new Student
            {
                Id = 2,
                ClassId = 2,
                SeatId = 1,
                Name = "李四",
                Address = "湖北荆州",
                CreateTime = DateTime.Now,
            });

            students.Add(new Student
            {
                Id = 3,
                Name = "王五",
                ClassId = 2,
                SeatId = 2,
                Address = "湖北荆州",
                CreateTime = DateTime.Now,
            });
            return students;
        }
    }
```
## 3.简单例子
### 1. 创建查询实体类
```C#
    public class StudentQuery
    {
        public int? Id { get; set; }
        public int? ClassId { get; set; }
        public int? SeatId { get; set; }
        public string Name { get; set; }
        public DateTime? CreateTime { get; set; }
        public string Address { get; set; }
    }
```
:loudspeaker:：EazyPageQuery默认按照查询实体属性名与目标实体属性名进行匹配然后筛选，如果实体属性名与目标属性名不同，请使用`[QueryFor]`特性显示标注。

:loudspeaker:：请注意查询实体所有属性最好都是**可为空**(`Nullable`)的类型，这样如果查询实体类属性为`NULL`，EazyPageQuery才能忽略此属性而不进行过滤。
### 2.过滤查询
如下所示，将查询所有`ClassId == 1`的学生，
```C#
    static void Main(string[] args)
    {
        List<Student> students = DataStore.CreaetStudents();
        StudentQuery studentQuery = new StudentQuery
        {
            ClassId = 1
        };
        var list = students.AsQueryable().Where(studentQuery).ToList();
    }
```
### 3.排序查询
:loudspeaker:：需要支持排序行为的查询实体类需要继承`IOrder`接口。

在需要排序的属性上添加`[OrderBy]`特性，
```C#
    public class StudentQuery:IQuery,IOrder
    {
        public int? Id { get; set; }
        public int? ClassId { get; set; }
        [OrderBy(Order = 0 ,OrderType = OrderType.Descending)]
        public int? SeatId { get; set; }
        public string Name { get; set; }
        public DateTime? CreateTime { get; set; }
        [Like]
        public string Address { get; set; }
    }
```
如下所示，将查询所有`ClassId == 1 && Name == "李四"`的学生，然后按照座位号降序排列。
```C#
    static void Main(string[] args)
    {
        List<Student> students = DataStore.CreaetStudents();
        StudentQuery studentQuery = new StudentQuery
        {
            ClassId = 1,
            Name = "李四"
        };
        var list = students.AsQueryable().Where(studentQuery).ToList();
    }
```
### 4.多重排序查询
在`Id`属性上添加`[OrderBy]`特性，并设置Order和排序类型，
```C#
    public class StudentQuery:IQuery,IOrder
    {
        [OrderBy(Order = 0, OrderType = OrderType.Ascending)]
        public int? Id { get; set; }
        public int? ClassId { get; set; }
        [OrderBy(Order = 1 ,OrderType = OrderType.Descending)]
        public int? SeatId { get; set; }
        public string Name { get; set; }
        public DateTime? CreateTime { get; set; }
        [Like]
        public string Address { get; set; }
    }
```
如下所示，将查询所有学生，并先按照学生号升序排列，如果学生号相同，则按照座位号降序排列。
```C#
    static void Main(string[] args)
    {
        List<Student> students = DataStore.CreaetStudents();
        StudentQuery studentQuery = new StudentQuery
        {

        };
        var list = students.AsQueryable().Where(studentQuery).ToList();
    }
```

### 5.模糊查询
首先在需要模糊查询的**字符串**属性上添加`[Like]`特性，
如下所示：
```C#
    public class StudentQuery:IQuery
    {
        public int? Id { get; set; }
        public int? ClassId { get; set; }
        public int? SeatId { get; set; }
        public string Name { get; set; }
        public DateTime? CreateTime { get; set; }
        [Like]
        public string Address { get; set; }
    }
```
然后添加条件，如下所示，将查询所有`ClassId == 1 && Name == "李四" && Address.Contains("州")`的学生。
```C#
    static void Main(string[] args)
    {
        List<Student> students = DataStore.CreaetStudents();
        StudentQuery studentQuery = new StudentQuery
        {
            ClassId = 1,
            Name = "李四",
            Address = "州"
        };
        var list = students.AsQueryable().Where(studentQuery).ToList();
    }
```
### 5.分页查询
:pushpin:：PageQuery类为所有分页查询实体类的基类，需要支持分页查询的查询实体类，请继承PageQuery基类，PageQuery结构如下：
```C#
    public class PageQuery:IQuery,IOrder
    {
        [NoQuery]
        public int PageSize { get; set; }
        [NoQuery]
        public int CurrentPage { get; set; }
        public PageQuery()
        {
            PageSize = 20;
            CurrentPage = 1;
        }
    }
```
创建分页查询实体类，
```C#
    public class StudentPageQuery:PageQuery
    {
        [OrderBy(Order = 0, OrderType = OrderType.Ascending)]
        public int? Id { get; set; }
        public int? ClassId { get; set; }
        [OrderBy(Order = 1, OrderType = OrderType.Descending)]
        public int? SeatId { get; set; }
        public string Name { get; set; }
        public DateTime? CreateTime { get; set; }
        [Like]
        public string Address { get; set; }
    }
}
```
如下所示设置，页大小为10，查询第2页。
```
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
```
返回对象是一个`Page<T>`泛型类型，结构如下：
```
    public class Page<T>
    {
        public int PageSize { get; private set; }
        public int CurrentPage { get; private set; }
        public int Total { get; private set; }
        public List<T> Rows { get; private set; }

        public Page(PageQuery pageQuery, List<T> rows, int total)
        {
            PageSize = pageQuery.PageSize;
            CurrentPage = pageQuery.CurrentPage;
            Rows = rows;
            Total = total;
        }
    }
```
其中`Total`属性表明总共有多少条符合条件的数据，`PageSize`表示当前页大小，`CurrentPage`表示当前页号，`Rows`表示当前页的数据。
### 6.动态排序分页查询
在查询实体类中添加一个布尔类型的属性，并添加`[OrderChoice]`特性。举个例子，如果学生号支持动态排序，则在查询实体类中添加Id==IsAsc==属性，注意命名规则，默认情况下，这个属性应由{目标属性名}IsAsc组成，若想更换默认后缀，使用Core.Suffix属性设置，或者直接使用`[QueryFor]`特性显示指明目标属性名。

:pushpin:：按照约定，如果该属性值为`TRUE`，则按照该字段升序排列，否则，降序排列。

如下所示：
```C#
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
```
使用该查询实体类创建查询对象，结果将按照`Id`降序排列。
```C#
    static void Main(string[] args)
    {
        List<Student> students = DataStore.CreaetStudents();
        StudentPageQuery studentPageQuery = new StudentPageQuery
        {
            PageSize = 10,
            CurrentPage = 1,
            IdIsAsc = false
        };
        Page<Student> page = students.AsQueryable().PageQeury(studentPageQuery);
    }
```
### 7.集合查询
:pushpin:：按照约定，查询实体属性名默认为目标属性名，末尾加`s`，若想自定义查询实体属性名称，请使用`[QueryFor]`特性显示指定目标属性名。

集合查询需要一个实现了IEnumrable接口的实例，如List，然后在该属性上添加`[EXISTSIN]`特性即可，如下所示
```C#
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
```
使用以下查询命令，将分页查询所有Id在(1,2)集合中的数据。
```C#
    var students = dataSotre.Students;
    StudentPageQuery studentPageQuery = new StudentPageQuery
    {
        PageSize = 10,
        CurrentPage = 1,
        Ids = new List<int> {1,2}
    };
    Page<Student> page = students.PageQuery(studentPageQuery);
```
### 8.静态范围查询
使用特性`[Judge]`便可指定静态范围查询，JudgeType可选枚举值如下：
枚举值|意义
:--:|:--:
eq|等于
ne|不等于
lt|小于
le|小于等于
gt|大于
ge|大于等于
ne|不等于

查询实体如下：
```C#
    public class StudentPageQuery:PageQuery
    {
        [Judge(JudgeType = JudgeType.ne)]
        public int Id { get; set; }
        public int? ClassId { get; set; }
        public int? SeatId { get; set; }
        public string Name { get; set; }
        public DateTime? CreateTime { get; set; }
        public string Address { get; set; }
    }
```
使用以下查询命令，将查询Id不等于`1`的学生
```C#
    var students = dataSotre.Students;
    StudentPageQuery studentPageQuery = new StudentPageQuery
    {
        PageSize = 10,
        CurrentPage = 1,
        Id = 1
    };
    Page<Student> page = students.PageQuery(studentPageQuery);
```
### 9.动态范围查询
动态范围查询可动态指定判断类型，使用时需要用到一个泛型包装类——`EazyJudgeValue`，使用该类型定义属性，并在此属性上添加`[DynamicJudge]`特性，如下所示。
```C#
    public class StudentPageQuery:PageQuery
    {
        [DynamicJudge]
        public EazyJudgeValue<int> Id { get; set; }
        public int? ClassId { get; set; }
        public int? SeatId { get; set; }
        public string Name { get; set; }
        public DateTime? CreateTime { get; set; }
        public string Address { get; set; }
    }
```
使用以下查询命令，将查询`Id >= 1`的学生。
```C#
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
    Page<Student> page = students.PageQuery(studentPageQuery)
```
# 默认约定参照表
查询类型 | 是否需要特性 | 需要的特性 | 默认属性命名规则 | 查询属性类型| 是否支持自定义属性名
:--: | :--: | :--: | :--: | :--: | :--: 
过滤查询 | :heavy_multiplication_x: | 无 | 与目标属性名相同 | 为目标属性类型的可空类型 | :heavy_check_mark:
静态排序查询 | :heavy_check_mark: | OrderBy | 与目标属性名相同 | 与目标属性类型相同 | :heavy_check_mark:
动态排序查询 | :heavy_check_mark: | OrderChoice | 目标属性名+'IsAsc' | Boolean | :heavy_check_mark:
模糊查询 | :heavy_check_mark: | Like | 与目标属性名相同 | String | :heavy_check_mark:
集合查询 | :heavy_check_mark: | EXISTSIN | 目标属性名+'s' | IEnumerable\<T> (T为目标类型) | :heavy_check_mark:
静态范围查询 | :heavy_check_mark: | Judge | 与目标属性名相同 | 与目标属性类型相同 | :heavy_check_mark:
动态范围查询 | :heavy_check_mark: | DynamicJudge | 与目标属性名相同 | EazyJudgeValue\<T> (T为目标类型) | :heavy_check_mark:
# 作者
白烟染黑墨
邮箱：935467953@qq.com