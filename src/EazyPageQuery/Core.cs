using EazyPageQuery.Basic;
using EazyPageQuery.Basic.QueryModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace EazyPageQuery
{
    public static class  Core
    {
        public static string Suffix = "IsAsc";
        /// <summary>
        /// 把TQuery对象中需要筛选的字段转换成Where表达式树
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TQuery"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        internal static IQueryable<TSource> TranslateWhere<TSource, TQuery>(this IQueryable<TSource> sources,TQuery query) where TQuery : IQuery
        {
            Type queryType = typeof(TQuery), destinationType = typeof(TSource);
            PropertyInfo[] queryPropertyInfos = queryType.GetProperties(), destinationPropertyinfos = destinationType.GetProperties();

            //x => { x.p1 == query.p1 && x.p2 == query.p2 && ....... } 中的参数节点
            ParameterExpression parameterExpression = Expression.Parameter(typeof(TSource), "x");
            Expression conditionExp = Expression.Constant(true);
            //接下来构造{} lambda表达式树节点
            foreach (var queryProp in queryPropertyInfos)
            {
                object queryPropValue = queryProp.GetValue(query);
                if (queryProp.IsDefined(typeof(NoQueryAttribute)) || (queryProp.CustomAttributes.Count() == 0 && queryPropValue == null)) continue;  //如果该值不为NULL，如果为NULL则不生成表达式
                if (queryProp.IsDefined(typeof(OrderChoiceAttribute))) continue;

                PropertyInfo desProp = null;
                if (queryProp.IsDefined(typeof(QueryForAttribute)))                                         //如果显示指定了要筛选的字段
                {
                    var attr = queryProp.GetCustomAttribute<QueryForAttribute>();
                    desProp = Utility.GetPropertyInfo(destinationPropertyinfos,attr.Name) ?? throw new PropertyNotFoundException(attr.Name);
                }


                if (queryProp.IsDefined(typeof(LikeAttribute)))
                {
                    if (queryPropValue is null) continue;
                    desProp = desProp ?? Utility.GetPropertyInfo(destinationPropertyinfos, queryProp.Name) ?? throw new PropertyNotFoundException(queryProp.Name);
                    var p = Expression.Call(Expression.Property(parameterExpression, desProp), "Contains", new Type[] { }, Expression.Property(Expression.Constant(query), queryProp));
                    conditionExp = Expression.AndAlso(p, conditionExp);
                }
                else if (queryProp.IsDefined(typeof(EXISTSINAttribute)))
                {
                    ExceptionHelper.ThrowArgNullExceptionIfNull(queryProp.Name, queryPropValue);
                    desProp = desProp ?? Utility.GetPropertyInfoTrimEnd(destinationPropertyinfos, queryProp.Name,"s");
                    if (desProp is null) ExceptionHelper.ThrowPropertyNameError($"the '{queryProp.Name}s' can not be found in the target object!");
          
                    var p = Expression.Call(null,GetContainsMethodInfoInEnumerabl(desProp.PropertyType), Expression.Property(Expression.Constant(query),queryProp),Expression.Property(parameterExpression, desProp));
                    conditionExp = Expression.AndAlso(p, conditionExp);
                }
                else if(queryProp.IsDefined(typeof(JudgeAttribute)))
                {
                    ExceptionHelper.ThrowArgNullExceptionIfNull(queryProp.Name, queryPropValue);
                    JudgeAttribute judgeAttribute = queryProp.GetCustomAttribute<JudgeAttribute>();
                    desProp = desProp ?? Utility.GetPropertyInfo(destinationPropertyinfos, queryProp.Name) ?? throw new PropertyNotFoundException(queryProp.Name);
                    var leftExp = Expression.Property(parameterExpression, desProp);
                    var rightExp = Expression.Convert(Expression.Property(Expression.Constant(query), queryProp), desProp.PropertyType);
                    conditionExp = Expression.AndAlso(GetJudgeExpression(leftExp,rightExp,judgeAttribute.JudgeType), conditionExp);
                }
                else if (queryProp.IsDefined(typeof(DynamicJudgeAttribute)))
                {
                    ExceptionHelper.ThrowArgNullExceptionIfNull(queryProp.Name, queryPropValue);
                    if (!queryProp.PropertyType.IsGenericType || queryProp.PropertyType.GetGenericTypeDefinition() != typeof(EazyJudgeValue<>)) ExceptionHelper.ThrowPropertyTypeError($"property type must be derived from EazyJudgeValue! Your type is {queryProp.PropertyType}");
                    object eazyValue = queryProp.GetValue(query);
                    var props = eazyValue.GetType().GetProperties();
                    JudgeType judgeType = (JudgeType)props.First(x => x.PropertyType == typeof(JudgeType)).GetValue(eazyValue);
                    object value = props.First(x => x.Name == "Value").GetValue(eazyValue);
                    desProp = desProp ?? Utility.GetPropertyInfo(destinationPropertyinfos, queryProp.Name) ?? throw new PropertyNotFoundException(queryProp.Name);
                    var leftExp = Expression.Property(parameterExpression, desProp);
                    var rightExp = Expression.Constant(value);
                    conditionExp = Expression.AndAlso(GetJudgeExpression(leftExp, rightExp, judgeType), conditionExp);
                }
                else
                {
                    if (queryPropValue is null) continue;
                    desProp = desProp ?? Utility.GetPropertyInfo(destinationPropertyinfos, queryProp.Name) ?? throw new PropertyNotFoundException(queryProp.Name);
                    var leftExp = Expression.Property(parameterExpression, desProp);
                    var rightExp = Expression.Convert(Expression.Property(Expression.Constant(query), queryProp), desProp.PropertyType);
                    var predicate = Expression.Equal(leftExp, rightExp);
                    conditionExp = Expression.AndAlso(predicate, conditionExp);
                }
            }
            var exp = Expression.Lambda<Func<TSource, bool>>(conditionExp, parameterExpression);
            return sources.Where(exp);
        }

        internal static Expression GetJudgeExpression(Expression leftExp, Expression rightExp, JudgeType judgeType)
        {
            Expression predicate;
            switch (judgeType)
            {
                case JudgeType.gt:
                    predicate = Expression.GreaterThan(leftExp, rightExp);
                    break;
                case JudgeType.eq:
                    predicate = Expression.Equal(leftExp, rightExp);
                    break;
                case JudgeType.lt:
                    predicate = Expression.LessThan(leftExp, rightExp);
                    break;
                case JudgeType.le:
                    predicate = Expression.LessThanOrEqual(leftExp, rightExp);
                    break;
                case JudgeType.ne:
                    predicate = Expression.NotEqual(leftExp, rightExp);
                    break;
                case JudgeType.ge:
                    predicate = Expression.GreaterThanOrEqual(leftExp, rightExp);
                    break;
                default:
                    ExceptionHelper.ThrowNotSupportedError($"Unknow JudgeType {judgeType}");
                    predicate = null;
                    break;
            }

            return predicate;
        }

        private  sealed class OrderInfo
        {
            public int Order;
            public OrderType OrderType;
            public PropertyInfo PropertyInfo;
            public OrderInfo() { }
            public OrderInfo(int order,OrderType orderType,PropertyInfo propertyInfo)
            {
                Order = order;
                OrderType = orderType;
                PropertyInfo = propertyInfo;
            }
        }

        
    
        internal static IOrderedQueryable<T> TranslateOrder<T,TQuery>(IQueryable<T> source,TQuery query) where TQuery:IOrder
        {
            Type queryType = typeof(TQuery), destinationType = typeof(T);
            PropertyInfo[] queryPropertyInfos = queryType.GetProperties(), destinationPropertyinfos = destinationType.GetProperties();
            List<OrderInfo> orderPropInfos = new List<OrderInfo>();

            foreach (var queryProp in queryPropertyInfos)
            {
                if (queryProp.IsDefined(typeof(NoQueryAttribute))) continue;
                OrderInfo orderInfo = new OrderInfo();
                bool CanChoice = false;
                if (queryProp.IsDefined(typeof(OrderByAttribute)))
                {
                    var orderAttr = queryProp.GetCustomAttribute<OrderByAttribute>();
                    orderInfo.Order = orderAttr.Order;
                    orderInfo.OrderType = orderAttr.OrderType;
                }
                else if (queryProp.IsDefined(typeof(OrderChoiceAttribute)))
                {
                    //Check value if it is bool
                    if (queryProp.PropertyType != typeof(bool)) ExceptionHelper.ThrowPropertyTypeError($"The {queryProp.Name} Property type is not bool,Please fix it or remove OrderChoiceAttribute.");
                    var choiceOrderAttr = queryProp.GetCustomAttribute<OrderChoiceAttribute>();
                    orderInfo.Order = choiceOrderAttr.Order;
                    //If value is true than order type is Ascending otherwise descending
                    orderInfo.OrderType = (bool)queryProp.GetValue(query) ? OrderType.Ascending : OrderType.Descending;
                    CanChoice = true;
                }
                else continue;

                PropertyInfo desProp;
                if (queryProp.IsDefined(typeof(QueryForAttribute)))
                {
                    var attr = queryProp.GetCustomAttribute<QueryForAttribute>();
                    desProp = destinationPropertyinfos.FirstOrDefault(x => x.Name == attr.Name);
                    if (desProp is null) ExceptionHelper.ThrowPropertyNotFound(attr.Name);
                }
                else if(CanChoice)
                {
                    //Check the name if it is end with suffix string.
                    if (!queryProp.Name.EndsWith(Suffix)) ExceptionHelper.ThrowPropertyNameError($"the property name do not end with suffix string '{Suffix}',please correct it or  using Core.Suffix to  change the suffix string.");
                    desProp = destinationPropertyinfos.FirstOrDefault(x => x.Name == queryProp.Name.TrimEnd(Suffix.ToArray()));
                }
                else desProp = destinationPropertyinfos.FirstOrDefault(x => x.Name == queryProp.Name);

                if (desProp is null) ExceptionHelper.ThrowPropertyNotFound(queryProp.Name);

                orderInfo.PropertyInfo = desProp;
                orderPropInfos.Add(orderInfo);
            }

            //按指定的顺序排序
            return LinkOrder(source,orderPropInfos);
        }

        private static IOrderedQueryable<T> LinkOrder<T>(IQueryable<T> source,OrderType orderType,PropertyInfo propertyInfo)
        {
            var parameter = Expression.Parameter(typeof(T));      //x
            var body = Expression.Property(parameter, propertyInfo);                //x.prop
            var lambda = Expression.Lambda(body, parameter);         //x => x.prop

            switch (orderType)
            {
                case OrderType.Ascending:
                    return (IOrderedQueryable<T>)source.Provider.CreateQuery<T>(Expression.Call(null, GetOrderByMethodInfo(typeof(T), propertyInfo.PropertyType), source.Expression, Expression.Quote(lambda)));
                case OrderType.Descending:
                    return (IOrderedQueryable<T>)source.Provider.CreateQuery<T>(Expression.Call(null, GetOrderByDescMethodInfo(typeof(T), propertyInfo.PropertyType), source.Expression, Expression.Quote(lambda)));
                default:
                    throw new ArgumentException($"Order type error,the value is {(int)orderType}");
            }
        }

        private static IOrderedQueryable<T> LinkOrderThen<T>(IQueryable<T> source, OrderType orderType, PropertyInfo propertyInfo)
        {
            var parameter = Expression.Parameter(typeof(T));      //x
            var body = Expression.Property(parameter, propertyInfo);                //x.prop
            var lambda = Expression.Lambda(body, parameter);         //x => x.prop

            switch (orderType)
            {
                case OrderType.Ascending:
                    return (IOrderedQueryable<T>)source.Provider.CreateQuery<T>(Expression.Call(null, GetThenByMethodInfo(typeof(T), propertyInfo.PropertyType), source.Expression, Expression.Quote(lambda)));
                case OrderType.Descending:
                    return (IOrderedQueryable<T>)source.Provider.CreateQuery<T>(Expression.Call(null, GetThenByDescMethodInfo(typeof(T), propertyInfo.PropertyType), source.Expression, Expression.Quote(lambda)));
                default:
                    throw new ArgumentException($"Order type error,the value is {(int)orderType}");
            }
        }
        private static IOrderedQueryable<T> LinkOrder<T>(IQueryable<T> source,List<OrderInfo> orderInfos)
        {
            if (orderInfos.Count == 0) return (IOrderedQueryable<T>)source;
            source = LinkOrder(source, orderInfos[0].OrderType, orderInfos[0].PropertyInfo);
            for (int i = 1; i< orderInfos.Count;i++) source = LinkOrderThen(source, orderInfos[i].OrderType, orderInfos[i].PropertyInfo);
            return (IOrderedQueryable <T>) source;
        }

        private static MethodInfo _orderByMethodInfo;

        private static MethodInfo GetOrderByMethodInfo(Type TSource, Type TKey) =>
             (_orderByMethodInfo ??
             (_orderByMethodInfo = new Func<IQueryable<object>, Expression<Func<object, object>>, IOrderedQueryable<object>>(Queryable.OrderBy).GetMethodInfo().GetGenericMethodDefinition()))
              .MakeGenericMethod(TSource, TKey);

        private static MethodInfo _orderByDescMethodInfo;
        private static MethodInfo GetOrderByDescMethodInfo(Type TSource, Type TKey) =>
         (_orderByDescMethodInfo ??
         (_orderByDescMethodInfo = new Func<IQueryable<object>, Expression<Func<object, object>>, IOrderedQueryable<object>>(Queryable.OrderByDescending)
            .GetMethodInfo().GetGenericMethodDefinition()))
          .MakeGenericMethod(TSource, TKey);

        private static MethodInfo _containsMethodInfo;

        private static MethodInfo GetContainsMethodInfoInEnumerabl(Type TSource) => 
            (_containsMethodInfo ??
            (_containsMethodInfo = new Func<IEnumerable<object>, object, bool>(Enumerable.Contains)
            .GetMethodInfo().GetGenericMethodDefinition())).MakeGenericMethod(TSource);

        private static MethodInfo _thenByMethodInfo;

        private static MethodInfo GetThenByMethodInfo(Type TSource, Type TKey) =>
             (_thenByMethodInfo ??
             (_thenByMethodInfo = new Func<IOrderedQueryable<object>, Expression<Func<object, object>>, IOrderedQueryable<object>>(Queryable.ThenBy).GetMethodInfo().GetGenericMethodDefinition()))
              .MakeGenericMethod(TSource, TKey);

        private static MethodInfo _thenByDescMethodInfo;

        private static MethodInfo GetThenByDescMethodInfo(Type TSource, Type TKey) =>
             (_thenByDescMethodInfo ??
             (_thenByDescMethodInfo = new Func<IOrderedQueryable<object>, Expression<Func<object, object>>, IOrderedQueryable<object>>(Queryable.ThenByDescending).GetMethodInfo().GetGenericMethodDefinition()))
              .MakeGenericMethod(TSource, TKey);

        internal static Expression<Func<T, bool>> GetExpression<T, TQuery>(Type desType, TQuery queryEntity)
        {
            Type sType = typeof(TQuery), dType = desType;
            PropertyInfo[] sProps = typeof(TQuery).GetProperties(), dProps = dType.GetProperties();
            ParameterExpression parameterExpression = Expression.Parameter(typeof(T), "x");
            Expression predicateExp = Expression.Constant(true);
            PropertyInfo p = typeof(T).GetProperties().First(x => x.PropertyType == desType);
            foreach (var sProp in sProps)
            {
                if (!sProp.IsDefined(typeof(NoQueryAttribute)) && sProp.GetValue(queryEntity) != null)
                {
                    PropertyInfo dProp = null;
                    if (sProp.IsDefined(typeof(QueryForAttribute)))
                    {
                        string mapForName = sProp.GetCustomAttribute<QueryForAttribute>().Name;
                        dProp = dProps.FirstOrDefault(x => x.Name == mapForName) ?? throw new PropertyNotFoundException(mapForName);
                    }
                    else
                    {
                        dProp = dProps.FirstOrDefault(x => x.Name == sProp.Name);
                    }
                    if (dProp == null) throw new PropertyNotFoundException(sProp.Name);
                    var leftExp = Expression.MakeMemberAccess(Expression.MakeMemberAccess(parameterExpression, p), dProp);
                    var rightExp = Expression.Convert(Expression.MakeMemberAccess(Expression.Constant(queryEntity), sProp), dProp.PropertyType);
                    var exp = Expression.Equal(leftExp, rightExp);
                    predicateExp = Expression.AndAlso(predicateExp, exp);

                }
            }
            Expression<Func<T, bool>> e = Expression.Lambda<Func<T, bool>>(predicateExp, parameterExpression);
            return e;
        }
        internal static Expression<Func<T, bool>> GetExpression<T, TQuery>(TQuery queryEntity)
        {
            Type sType = typeof(TQuery), dType = typeof(T);
            PropertyInfo[] sProps = typeof(TQuery).GetProperties(), dProps = typeof(T).GetProperties();
            ParameterExpression parameterExpression = Expression.Parameter(dType, "x");
            Expression predicateExp = Expression.Constant(true);
            foreach (var sProp in sProps)
            {
                if (!sProp.IsDefined(typeof(NoQueryAttribute)) && sProp.GetValue(queryEntity) != null)
                {
                    PropertyInfo dProp = null;
                    if (sProp.IsDefined(typeof(QueryForAttribute)))
                    {
                        string mapForName = sProp.GetCustomAttribute<QueryForAttribute>().Name;
                        dProp = dProps.FirstOrDefault(x => x.Name == mapForName) ?? throw new PropertyNotFoundException(mapForName);
                    }
                    else
                    {
                        dProp = dProps.FirstOrDefault(x => x.Name == sProp.Name);
                    }
                    if (dProp == null) throw new PropertyNotFoundException(sProp.Name);
                    var leftExp = Expression.MakeMemberAccess(parameterExpression, dProp);
                    var rightExp = Expression.Convert(Expression.MakeMemberAccess(Expression.Constant(queryEntity), sProp), dProp.PropertyType);
                    var exp = Expression.Equal(leftExp, rightExp);
                    predicateExp = Expression.AndAlso(predicateExp, exp);

                }
            }
            Expression<Func<T, bool>> e = Expression.Lambda<Func<T, bool>>(predicateExp, parameterExpression);
            return e;
        }
    }
}
