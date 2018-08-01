using System;
using System.Linq.Expressions;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace Cosmos.SqlBuilder
{
    public interface ISubModelBuilder<T>
    {
        ISubModelBuilder<T> Select<TRes>(Expression<Func<T, TRes>> func);
        ISubModelBuilder<T> Select<TRes>(Expression<Func<T, TRes>> func, Action<ISubModelBuilder<TRes>> subModel);
    }

    class SubModelBuilder<T> : ISubModelBuilder<T>
    {
        private Dictionary<string, string> _selectCols;
        private string _prefix;
        public SubModelBuilder(string prefix)
        {
            _prefix = prefix;
            _selectCols = new Dictionary<string, string>();
        }

        public ISubModelBuilder<T> Select<TRes>(Expression<Func<T, TRes>> func)
        {
            var body = func.Body.ToString().Split(".");
            _selectCols.Add($"\"{body.Last()}\"", $"{_prefix}.{body.Last()}");
            return this;
        }

        public ISubModelBuilder<T> Select<TRes>(Expression<Func<T, TRes>> func, Action<ISubModelBuilder<TRes>> subModel)
        {            
            var body = func.Body.ToString().Split(".");
            var propName = body.Last();
            SubModelBuilder<TRes> innerBuilder = new SubModelBuilder<TRes>($"{_prefix}.{propName}");

            subModel(innerBuilder);

            var subQuery = innerBuilder.GetSubQuery();
            
            _selectCols.Add($"\"{propName}\"", $"{subQuery}");
            return this;
        }
        internal string GetSubQuery()
        {
            StringBuilder builder = new StringBuilder("{ ");

            foreach(var item in _selectCols)
            {
                builder.Append(item.Key);
                builder.Append(": ");
                builder.Append(item.Value);
                builder.Append(", ");
            }
            builder.Remove(builder.Length - 2, 2);
            builder.Append(" }");
            return builder.ToString();
        }
    }
}