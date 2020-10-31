﻿using EazyPageQuery.Basic;
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
        /// <summary>
        /// 把TQuery对象中需要筛选的字段转换成Where表达式树
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TQuery"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        internal static Expression<Func<T, bool>> TranslateWhere<T, TQuery>(TQuery query) where TQuery:IQuery
        {
            Type queryType = typeof(TQuery), destinationType = typeof(T);
            PropertyInfo[] queryPropertyInfos = queryType.GetProperties(), destinationPropertyinfos = destinationType.GetProperties();

            //x => { x.p1 == query.p1 && x.p2 == query.p2 && ....... } 中的参数节点
            ParameterExpression parameterExpression = Expression.Parameter(typeof(T), "x");
            Expression conditionExp = Expression.Constant(true);
            //接下来构造{} lambda表达式树节点
            foreach(var queryProp in queryPropertyInfos)
            {
                if (queryProp.IsDefined(typeof(NoQueryAttribute)) || queryProp.GetValue(query) == null) continue;  //如果该值不为NULL，如果为NULL则不生成表达式

                if (queryProp.IsDefined(typeof(QueryForAttribute)))                                         //如果显示指定了要筛选的字段
                {
                    var attr = queryProp.GetCustomAttribute<QueryForAttribute>();
                    var desProp = destinationPropertyinfos.FirstOrDefault(x => x.Name == attr.Name) ?? throw new PropertyNotFoundException(attr.Name);

                    var leftExp = Expression.Property(parameterExpression, desProp);                                                 //x.desProp
                    var rightExp = Expression.Convert(Expression.Property(Expression.Constant(query), queryProp),desProp.PropertyType);                                //query.queryProp
                    var predicate = Expression.Equal(leftExp, rightExp);                                                                    //x.desProp == query.queryProp
                    conditionExp = Expression.AndAlso(predicate, conditionExp);                                                   //  x.desProp == query.queryProp && 之前的表达式
                }
                else
                {
                    var desProp = destinationPropertyinfos.FirstOrDefault(x => x.Name == queryProp.Name) ?? throw new PropertyNotFoundException(queryProp.Name);
                    if (desProp != null)
                    {
                        if (queryProp.IsDefined(typeof(LikeAttribute)))
                        {
                            var p = Expression.Call(Expression.Property(parameterExpression, desProp), "Contains", new Type[] { }, Expression.Property(Expression.Constant(query), queryProp));
                            conditionExp = Expression.AndAlso(p, conditionExp);
                            continue;
                        }
                        var leftExp = Expression.Property(parameterExpression, desProp);                                                 //x.desProp
                        var rightExp = Expression.Convert(Expression.Property(Expression.Constant(query), queryProp), desProp.PropertyType);                                //query.queryProp
                        var predicate = Expression.Equal(leftExp, rightExp);                                                                    //x.desProp == query.queryProp
                        conditionExp = Expression.AndAlso(predicate, conditionExp);                                                   //  x.desProp == query.queryProp && 之前的表达式
                    }
                }
            }

            var exp = Expression.Lambda<Func<T, bool>>(conditionExp, parameterExpression);
            return exp;
        } 

        private  sealed class OrderInfo
        {
            public int Order;
            public OrderType OrderType;
            public PropertyInfo PropertyInfo;
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

            Expression expression = Expression.Constant(source);

            foreach (var queryProp in queryPropertyInfos)
            {
                if(queryProp.IsDefined(typeof(OrderByAttribute)))
                {
                    var orderAttr = queryProp.GetCustomAttribute<OrderByAttribute>();
                    PropertyInfo desProp;
                    if (queryProp.IsDefined(typeof(QueryForAttribute)))
                    {
                        var attr = queryProp.GetCustomAttribute<QueryForAttribute>();
                        desProp = destinationPropertyinfos.FirstOrDefault(x => x.Name == attr.Name);
                        if (desProp is null) ExceptionHelper.ThrowPropertyNotFound(attr.Name);
                    }
                    else desProp = destinationPropertyinfos.FirstOrDefault(x => x.Name == queryProp.Name);

                    if (desProp is null) ExceptionHelper.ThrowPropertyNotFound(queryProp.Name);
                    orderPropInfos.Add(new OrderInfo(orderAttr.Order, orderAttr.OrderType, desProp));
                }
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
        public static MethodInfo GetOrderByDescMethodInfo(Type TSource, Type TKey) =>
         (_orderByDescMethodInfo ??
         (_orderByDescMethodInfo = new Func<IQueryable<object>, Expression<Func<object, object>>, IOrderedQueryable<object>>(Queryable.OrderByDescending).GetMethodInfo().GetGenericMethodDefinition()))
          .MakeGenericMethod(TSource, TKey);


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