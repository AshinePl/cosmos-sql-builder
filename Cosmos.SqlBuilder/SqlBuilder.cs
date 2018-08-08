using System;
using System.Linq.Expressions;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace Cosmos.SqlBuilder
{
    public class SqlBuilder : ISqlBuilder {
        protected StringBuilder sql;
        protected int? top;
        protected List<string> selectCols;
        protected List<string> whereCases;
        protected string orderBy;

        public SqlBuilder()
        {
            sql = new StringBuilder();
            selectCols = new List<string>();
            whereCases = new List<string>();
        }

        public ISqlBuilderSelectTopOrAllOrSome<T> From<T>()
        {
            return new SqlBuilder<T>();
        }

        public virtual string Build() {

            sql.Append("SELECT ");

            if(top.HasValue)
            {
                sql.Append($"TOP {top.Value} ");
            }

            sql.Append(string.Join(", ", selectCols));
            sql.Append(" FROM c");

            if(whereCases.Any())
            {
                sql.Append(" WHERE ");
                sql.Append(string.Join(" AND ", whereCases));
            }

            if(!string.IsNullOrWhiteSpace(orderBy))
            {
                sql.Append(" ORDER BY ").Append(orderBy);
            }
                        
            return sql.ToString();
        }
    }

    public class SqlBuilder<T> : SqlBuilder, ISqlBuilderAll<T>
    {

        public SqlBuilder() : base()
        {
            //whereCases.Add(new WhereStatement()
            //{
            //    Type = JoinType.AND,
            //    Statement = $"c.Entity = '{nameof(T)}'"
            //});
        }

        

        public ISqlBuilderWhereOrOrderBy<T> SelectAll()
        {
            selectCols.Add("*");
            return this;
        }

        public ISqlBuilderSelectSomeOrWhereOrOrderBy<T> Select<TRes>(Expression<Func<T, TRes>> func)
        {
            selectCols.Add($"c.{GetPropertyFullName(func)}");
            return this;
        }
        public ISqlBuilderSelectSomeOrWhereOrOrderBy<T> Select<TRes>(Expression<Func<T, TRes>> func, Action<ISubModelBuilder<TRes>> subModel)
        {
            string propName = GetPropertyFullName(func);

            SubModelBuilder<TRes> innerBuilder = new SubModelBuilder<TRes>($"c.{propName}");

            subModel(innerBuilder);

            var subQuery = innerBuilder.GetSubQuery();

            selectCols.Add($"{subQuery} as {propName}");
            return this;
        }
        
        public ISqlBuilderWhereOrOrderBy<T> Where(Expression<Func<T, bool>> func)
        {
            var lambda = func.Body as BinaryExpression;

            string parameter = func.Parameters[0].Name;

            if (lambda == null && func.Body.NodeType == ExpressionType.MemberAccess && func.Body.Type == typeof(Boolean))
            {
                whereCases.Add(CheckSide(func.Body, parameter));
                return this;
            }

            if (lambda == null && func.Body.NodeType == ExpressionType.Not && func.Body.Type == typeof(Boolean))
            {
                var notState = $"NOT {CheckSide(func.Body, parameter)}";

                whereCases.Add(notState);
                return this;
            }

            string statement = GetQueryFromExpression(lambda, parameter);

            whereCases.Add(statement);
            return this;
        }
        
        public ISqlBuilderSelectAllOrSome<T> Top(int number)
        {
            top = number;
            return this;
        }

        public ISqlBuilderBuild OrderByAscending<TRes>(Expression<Func<T, TRes>> func)
        {
            orderBy = $"{OrderBy(func)} ASC";
            return this;
        }

        public ISqlBuilderBuild OrderByDescending<TRes>(Expression<Func<T, TRes>> func)
        {
            orderBy = $"{OrderBy(func)} DESC";
            return this;
        }
        
        private string OrderBy<TRes>(Expression<Func<T, TRes>> func)
        {
            return $"c.{GetPropertyFullName(func)}";
        }

        private string GetQueryFromExpression(BinaryExpression expression, string parameterName)
        {
            var left = CheckSide(expression.Left, parameterName);
            var right = CheckSide(expression.Right, parameterName);

            switch (expression.NodeType)
            {
                case ExpressionType.AndAlso:
                    return $"({left}) AND ({right})";
                case ExpressionType.OrElse:
                    return $"({left}) OR ({right})";
                case ExpressionType.Equal:
                    return $"{left} = {right}";
                case ExpressionType.NotEqual:
                    return $"{left} != {right}";
                case ExpressionType.LessThan:
                    return $"{left} < {right}";
                case ExpressionType.LessThanOrEqual:
                    return $"{left} <= {right}";
                case ExpressionType.GreaterThan:
                    return $"{left} > {right}";
                case ExpressionType.GreaterThanOrEqual:
                    return $"{left} >= {right}";
            }

            return "";
        }

        private string CheckSide(Expression expression, string parameterName)
        {
            if (expression as BinaryExpression != null)
            {
                return GetQueryFromExpression(expression as BinaryExpression, parameterName);
            }
            else if (expression as ConstantExpression != null)
            {
                var constExp = expression as ConstantExpression;
                var constString = constExp.ToString();
                if (constExp.Type == typeof(string))
                {
                    return $"'{constString.Substring(1, constString.Length - 2)}'";
                }
                if (constExp.Type == typeof(Boolean))
                {
                    return constString.ToLower();
                }
                return constString;
            }
            else if (expression as UnaryExpression != null)
            {
                var unary = expression as UnaryExpression;
                return $"c{unary.Operand.ToString().Substring(parameterName.Length)}";
            }
            var expressionString = expression.ToString();
            return $"c{expressionString.Substring(parameterName.Length)}";
        }
        
        private string GetPropertyFullName<TRes>(Expression<Func<T, TRes>> func)
        {
            var body = func.Body.ToString().Split('.');
            var propName = string.Join(".", body.Skip(1));
            return propName;
        }
    }
}