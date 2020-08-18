using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Storm.Filters
{
    public abstract class Filter
    {
        public static Filter operator *(Filter left, Filter right)
        {
            switch (left)
            {
                case AndFilter andLeft:
                    switch (right)
                    {
                        case AndFilter andRight:
                            return new AndFilter() { filters = andLeft.filters.Concat(andRight.filters) };
                        case OrFilter orRight:
                            return new AndFilter() { filters = andLeft.filters.Concat(new[] { orRight }) };
                        case MonoFilter monoRight:
                            return new AndFilter() { filters = andLeft.filters.Concat(new[] { monoRight }) };
                        default:
                            throw new Exception("Unupported Filter type");
                    }
                case OrFilter orLeft:
                    switch (right)
                    {
                        case AndFilter andRight:
                            return new AndFilter() { filters = (new[] { orLeft }).Concat(andRight.filters) };
                        case OrFilter orRight:
                            return new AndFilter() { filters = (new[] { orLeft }).Cast<Filter>().Concat(new[] { orRight }) };
                        case MonoFilter monoRight:
                            return new AndFilter() { filters = (new[] { orLeft }).Cast<Filter>().Concat(new[] { monoRight }) };
                        default:
                            throw new Exception("Unupported Filter type");
                    }
                case MonoFilter monoLeft:
                    switch (right)
                    {
                        case AndFilter andRight:
                            return new AndFilter() { filters = (new[] { monoLeft }).Concat(andRight.filters) };
                        case OrFilter orRight:
                            return new AndFilter() { filters = (new[] { monoLeft }).Cast<Filter>().Concat(new[] { orRight }) };
                        case MonoFilter monoRight:
                            return new AndFilter() { filters = (new[] { monoLeft }).Cast<Filter>().Concat(new[] { monoRight }) };
                        default:
                            throw new Exception("Unupported Filter type");
                    }
                default:
                    throw new Exception("Unupported Filter type");
            }
        }

        public static Filter operator +(Filter left, Filter right)
        {
            switch (left)
            {
                case AndFilter andLeft:
                    switch (right)
                    {
                        case AndFilter andRight:
                            return new OrFilter() { filters = (new[] { andLeft }).Cast<Filter>().Concat(new[] { andRight }) };
                        case OrFilter orRight:
                            return new OrFilter() { filters = (new[] { andLeft }).Cast<Filter>().Concat(orRight.filters) };
                        case MonoFilter monoRight:
                            return new OrFilter() { filters = (new[] { andLeft }).Cast<Filter>().Concat(new[] { monoRight }) };
                        default:
                            throw new Exception("Unupported Filter type");
                    }
                case OrFilter orLeft:
                    switch (right)
                    {
                        case AndFilter andRight:
                            return new OrFilter() { filters = orLeft.filters.Concat(new[] { andRight }) };
                        case OrFilter orRight:
                            return new OrFilter() { filters = orLeft.filters.Concat(orRight.filters) };
                        case MonoFilter monoRight:
                            return new OrFilter() { filters = orLeft.filters.Concat(new[] { monoRight }) };
                        default:
                            throw new Exception("Unupported Filter type");
                    }
                case MonoFilter monoLeft:
                    switch (right)
                    {
                        case AndFilter andRight:
                            return new OrFilter() { filters = (new[] { monoLeft }).Cast<Filter>().Concat(new[] { andRight }) };
                        case OrFilter orRight:
                            return new OrFilter() { filters = (new[] { monoLeft }).Concat(orRight.filters) };
                        case MonoFilter monoRight:
                            return new OrFilter() { filters = (new[] { monoLeft }).Cast<Filter>().Concat(new[] { monoRight }) };
                        default:
                            throw new Exception("Unupported Filter type");
                    }
                default:
                    throw new Exception("Unupported Filter type");
            }
        }
    }

    public abstract class MultiFilter : Filter
    {
        internal IEnumerable<Filter> filters;
    }

    public class MonoFilter : Filter
    {
        public ReferenceFilterValue Left;
        public FilterValue Right;
    }
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

    public class Expression
    {
        public FluentOperationSelectorSyntax this[string path] => continueSintax(path);

        private FluentOperationSelectorSyntax continueSintax(string path)
        {
            return new FluentOperationSelectorSyntax(path);
        }

        public class FluentOperationSelectorSyntax
        {
            private readonly string _path;

            internal FluentOperationSelectorSyntax(string path)
            {
                _path = path;
            }

            public FluentValueSelecetorSintax EqualTo => new FluentValueSelecetorSintax(new EqualToFilter() { Left = new ReferenceFilterValue(_path) });

            public FluentValueSelecetorSintax NotEqualTo => new FluentValueSelecetorSintax(new NotEqualToFilter() { Left = new ReferenceFilterValue(_path) });

            public FluentValueSelecetorSintax GreaterTo => new FluentValueSelecetorSintax(new GreaterToFilter() { Left = new ReferenceFilterValue(_path) });

            public FluentValueSelecetorSintax GreaterOrEqualTo => new FluentValueSelecetorSintax(new GreaterOrEqualToFilter() { Left = new ReferenceFilterValue(_path) });

            public FluentValueSelecetorSintax LessTo => new FluentValueSelecetorSintax(new LessToFilter() { Left = new ReferenceFilterValue(_path) });

            public FluentValueSelecetorSintax LessOrEqualTo => new FluentValueSelecetorSintax(new LessOrEqualToFilter() { Left = new ReferenceFilterValue(_path) });

            public FluentValueSelecetorSintax Like => new FluentValueSelecetorSintax(new LikeFilter() { Left = new ReferenceFilterValue(_path) });

            public FluentValueSelecetorSintax NotLike => new FluentValueSelecetorSintax(new NotLikeFilter() { Left = new ReferenceFilterValue(_path) });

            public FluentIEnumerableValueSelectorSintax In => new FluentIEnumerableValueSelectorSintax(new InFilter() { Left = new ReferenceFilterValue(_path) });

            public FluentIEnumerableValueSelectorSintax NotIn => new FluentIEnumerableValueSelectorSintax(new NotInFilter() { Left = new ReferenceFilterValue(_path) });

            public Filter IsNull => new IsNullFilter() { Left = new ReferenceFilterValue(_path) }; 
            
            public Filter IsNotNull => new IsNotNullFilter() { Left = new ReferenceFilterValue(_path) };

        }

        public class FluentValueSelecetorSintax
        {
            private readonly Filter _filter;

            internal FluentValueSelecetorSintax(Filter filter)
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

        public class FluentIEnumerableValueSelectorSintax
        {
            private readonly Filter _filter;
            internal FluentIEnumerableValueSelectorSintax(Filter filter)
            {
                _filter = filter;
            }

            public Filter Val(IEnumerable value)
            {
                ((MonoFilter)_filter).Right = new DataFilterValue(value);
                return _filter;
            }

        }
    }
}
