using Storm.Execution;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Storm.Filters
{
    public abstract class FilterValue
    {

    }

    public class ReferenceFilterValue : FilterValue
    {
        internal string _path;

        public String Path => _path;

        public ReferenceFilterValue(string path)
        {
            _path = path;
        }
    }

    public class DataFilterValue : FilterValue
    {
        internal object value;

        public DataFilterValue(object value)
        {
            this.value = value;
        }
    }

    public class SubQueryFilterValue : FilterValue
    {
        internal readonly Func<SubQueryContext, NestedCommand> subquery;

        public SubQueryFilterValue(Func<SubQueryContext, NestedCommand> subquery)
        {
            this.subquery = subquery;
        }
    }

    public class FilterContext
    {
        public Operator this[string path] => continueSintax(path);
        public Operator Filter(string path) => continueSintax(path);
        private Operator continueSintax(string path) => new Operator(path);

        public class Operator
        {
            private readonly string _path;

            internal Operator(string path)
            {
                _path = path;
            }

            public ValueOperand EqualTo => new ValueOperand(new EqualToFilter() { Left = new ReferenceFilterValue(_path) });

            public ValueOperand NotEqualTo => new ValueOperand(new NotEqualToFilter() { Left = new ReferenceFilterValue(_path) });

            public ValueOperand GreaterTo => new ValueOperand(new GreaterToFilter() { Left = new ReferenceFilterValue(_path) });

            public ValueOperand GreaterOrEqualTo => new ValueOperand(new GreaterOrEqualToFilter() { Left = new ReferenceFilterValue(_path) });

            public ValueOperand LessTo => new ValueOperand(new LessToFilter() { Left = new ReferenceFilterValue(_path) });

            public ValueOperand LessOrEqualTo => new ValueOperand(new LessOrEqualToFilter() { Left = new ReferenceFilterValue(_path) });

            public ValueOperand Like => new ValueOperand(new LikeFilter() { Left = new ReferenceFilterValue(_path) });

            public ValueOperand NotLike => new ValueOperand(new NotLikeFilter() { Left = new ReferenceFilterValue(_path) });

            public EnumerableOperand In => new EnumerableOperand(new InFilter() { Left = new ReferenceFilterValue(_path) });

            public EnumerableOperand NotIn => new EnumerableOperand(new NotInFilter() { Left = new ReferenceFilterValue(_path) });

            public Filter IsNull => new IsNullFilter() { Left = new ReferenceFilterValue(_path) }; 
            
            public Filter IsNotNull => new IsNotNullFilter() { Left = new ReferenceFilterValue(_path) };

        }

        public class ValueOperand
        {
            private readonly Filter _filter;

            internal ValueOperand(Filter filter)
            {
                _filter = filter;
            }

            public Filter Ref(string path)
            {
                ((MonoFilter)_filter).Right = new ReferenceFilterValue(path);
                return _filter;
            }

            public Filter Val(object value)
            {
                ((MonoFilter)_filter).Right = new DataFilterValue(value);
                return _filter;
            }
        }

        public class EnumerableOperand
        {
            private readonly Filter _filter;
            internal EnumerableOperand(Filter filter)
            {
                _filter = filter;
            }

            public Filter Val(IEnumerable value)
            {
                ((MonoFilter)_filter).Right = new DataFilterValue(value);
                return _filter;
            }

            public Filter SubQuery(Func<SubQueryContext, NestedCommand> subquery)
            {
                ((MonoFilter)_filter).Right = new SubQueryFilterValue(subquery);
                return _filter;
            }

        }
    }


    public class SubQueryContext
    {
        private readonly Context ctx;

        internal SubQueryContext(Context ctx)
        {
            this.ctx = ctx;
        }

        public NestedCommand From(String EntityIdentifier)
        {
            return new NestedCommand(ctx, EntityIdentifier);
        }
    }

    public class JoinContext
    {
        public Operator this[string path] => continueSintax(path);
        public Operator Filter(string path) => continueSintax(path);
        private Operator continueSintax(string path) => new Operator(path);

        public class Operator
        {
            private readonly string _path;

            internal Operator(string path)
            {
                this._path = path;
            }

            public ValueOperand EqualTo => new ValueOperand(new EqualToFilter() { Left = new ReferenceFilterValue(_path) });

            public ValueOperand NotEqualTo => new ValueOperand(new NotEqualToFilter() { Left = new ReferenceFilterValue(_path) });

            public ValueOperand GreaterTo => new ValueOperand(new GreaterToFilter() { Left = new ReferenceFilterValue(_path) });

            public ValueOperand GreaterOrEqualTo => new ValueOperand(new GreaterOrEqualToFilter() { Left = new ReferenceFilterValue(_path) });

            public ValueOperand LessTo => new ValueOperand(new LessToFilter() { Left = new ReferenceFilterValue(_path) });

            public ValueOperand LessOrEqualTo => new ValueOperand(new LessOrEqualToFilter() { Left = new ReferenceFilterValue(_path) });

            public ValueOperand Like => new ValueOperand(new LikeFilter() { Left = new ReferenceFilterValue(_path) });

            public ValueOperand NotLike => new ValueOperand(new NotLikeFilter() { Left = new ReferenceFilterValue(_path) });

            public Filter IsNull => new IsNullFilter() { Left = new ReferenceFilterValue(_path) };

            public Filter IsNotNull => new IsNotNullFilter() { Left = new ReferenceFilterValue(_path) };

        }

        public class ValueOperand
        {
            private readonly Filter _filter;

            internal ValueOperand(Filter filter)
            {
                _filter = filter;
            }

            public Filter Ref(string path)
            {
                ((MonoFilter)_filter).Right = new ReferenceFilterValue(path);
                return _filter;
            }

            public Filter Val(object value)
            {
                ((MonoFilter)_filter).Right = new DataFilterValue(value);
                return _filter;
            }
        }
    }


}
