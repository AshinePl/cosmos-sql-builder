using System;
using System.Linq.Expressions;

namespace Cosmos.SqlBuilder
{
    public interface ISqlBuilder
    {
        ISqlBuilderSelectTopOrAllOrSome<T> From<T>();
    }

    public interface ISqlBuilderAll<T> : ISqlBuilderSelectTopOrAllOrSome<T>, ISqlBuilderSelectSomeOrWhereOrOrderBy<T>, ISqlBuilderWhereOrOrderBy<T>
    { }

    public interface ISqlBuilderSelectTopOrAllOrSome<T> : ISqlBuilderSelectAllOrSome<T>, ISqlBuilderTop<T>
    { }

    public interface ISqlBuilderSelectAllOrSome<T> : ISqlBuilderSelectAll<T>, ISqlBuilderSelectSome<T>
    { }

    public interface ISqlBuilderSelectSomeOrWhereOrOrderBy<T> : ISqlBuilderWhere<T>, ISqlBuilderSelectSome<T>, ISqlBuilderOrderBy<T>
    { }

    public interface ISqlBuilderWhereOrOrderBy<T> : ISqlBuilderWhere<T>, ISqlBuilderOrderBy<T>
    { }

    public interface ISqlBuilderBuild
    {
        string Build();
    }

    public interface ISqlBuilderTop<T> : ISqlBuilderBuild
    {
        ISqlBuilderSelectAllOrSome<T> Top(int number);
    }

    public interface ISqlBuilderSelectAll<T> : ISqlBuilderBuild
    {
        ISqlBuilderWhereOrOrderBy<T> SelectAll();
    }

    public interface ISqlBuilderSelectSome<T> : ISqlBuilderBuild
    {
        ISqlBuilderSelectSomeOrWhereOrOrderBy<T> Select<TRes>(Expression<Func<T, TRes>> func);
        ISqlBuilderSelectSomeOrWhereOrOrderBy<T> Select<TRes>(Expression<Func<T, TRes>> func, Action<ISubModelBuilder<TRes>> subModel);
    }

    public interface ISqlBuilderWhere<T> : ISqlBuilderBuild
    {
        ISqlBuilderWhereOrOrderBy<T> Where(Expression<Func<T, bool>> func);
    }

    public interface ISqlBuilderOrderBy<T> : ISqlBuilderBuild
    {
        ISqlBuilderBuild OrderByAscending<TRes>(Expression<Func<T, TRes>> func);
        ISqlBuilderBuild OrderByDescending<TRes>(Expression<Func<T, TRes>> func);
    }
}
