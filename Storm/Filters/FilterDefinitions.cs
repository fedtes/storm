using System;
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

    public class AndFilter : MultiFilter
    {

    }

    public class OrFilter : MultiFilter
    {

    }

    public class EqualToFilter : MonoFilter { }
    public class NotEqualToFilter : MonoFilter { }
    public class GreaterToFilter : MonoFilter { }
    public class GreaterOrEqualToFilter : MonoFilter { }
    public class LessToFilter : MonoFilter { }
    public class LessOrEqualToFilter : MonoFilter { }
    public class LikeFilter : MonoFilter { }
    public class NotLikeFilter : MonoFilter { }
    public class InFilter : MonoFilter { }
    public class NotInFilter : MonoFilter { }
    public class IsNullFilter : MonoFilter { }
    public class IsNotNullFilter : MonoFilter { }

}
