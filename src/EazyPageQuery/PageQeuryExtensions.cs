using EazyPageQuery.Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace EazyPageQuery
{
    public static class PageQeuryExtensions
    {
        public static IQueryable<T> Where<T, TQuery>(this IQueryable<T> source,TQuery query) where TQuery:IQuery
        {
            return source.TranslateWhere(query);
        }

        public static IQueryable<T> WhereWhen<T>(this IQueryable<T> source, bool condition, Expression<Func<T, bool>> expression) => condition ? source.Where(expression) : source;
        public static IQueryable<T> WhereWhenNN<T>(this IQueryable<T> source, Expression<Func<T, bool>> expression) => source.WhereWhen(source != null, expression);

        public static IQueryable<T> OrderBy<T, TQuery>(this IQueryable<T> source,TQuery query) where TQuery:IOrder
        {
            return Core.TranslateOrder(source, query);
        }

        public static Page<T> PageQuery<T,TQuery>(this IQueryable<T> source, TQuery query) where TQuery:PageQuery
        {
            if (query.CurrentPage < 1) throw new ArgumentException($"CurrentPage can not less than one!",nameof(query.CurrentPage));
            if (query.PageSize < 1) throw new ArgumentException($"PageSize can not less than one!",nameof(query.PageSize));

            var exp = source.Where(query);
            var totalCount = exp.Count();
            var rows = exp.OrderBy(query).Skip((query.CurrentPage - 1) * query.PageSize).Take(query.PageSize).ToList();
            return new Page<T>(query,rows, totalCount);
        }
    }
}
