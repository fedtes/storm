using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Storm.Filters
{
    public abstract class FilterExpression
    {
        public static FilterExpression operator *(FilterExpression left, FilterExpression right)
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
                            return new AndFilter() { filters = (new[] { orLeft }).Cast<FilterExpression>().Concat(new[] { orRight }) };
                        case MonoFilter monoRight:
                            return new AndFilter() { filters = (new[] { orLeft }).Cast<FilterExpression>().Concat(new[] { monoRight }) };
                        default:
                            throw new Exception("Unupported Filter type");
                    }
                case MonoFilter monoLeft:
                    switch (right)
                    {
                        case AndFilter andRight:
                            return new AndFilter() { filters = (new[] { monoLeft }).Concat(andRight.filters) };
                        case OrFilter orRight:
                            return new AndFilter() { filters = (new[] { monoLeft }).Cast<FilterExpression>().Concat(new[] { orRight }) };
                        case MonoFilter monoRight:
                            return new AndFilter() { filters = (new[] { monoLeft }).Cast<FilterExpression>().Concat(new[] { monoRight }) };
                        default:
                            throw new Exception("Unupported Filter type");
                    }
                default:
                    throw new Exception("Unupported Filter type");
            }
        }

        public static FilterExpression operator +(FilterExpression left, FilterExpression right)
        {
            switch (left)
            {
                case AndFilter andLeft:
                    switch (right)
                    {
                        case AndFilter andRight:
                            return new OrFilter() { filters = (new[] { andLeft }).Cast<FilterExpression>().Concat(new[] { andRight }) };
                        case OrFilter orRight:
                            return new OrFilter() { filters = (new[] { andLeft }).Cast<FilterExpression>().Concat(orRight.filters) };
                        case MonoFilter monoRight:
                            return new OrFilter() { filters = (new[] { andLeft }).Cast<FilterExpression>().Concat(new[] { monoRight }) };
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
                            return new OrFilter() { filters = (new[] { monoLeft }).Cast<FilterExpression>().Concat(new[] { andRight }) };
                        case OrFilter orRight:
                            return new OrFilter() { filters = (new[] { monoLeft }).Concat(orRight.filters) };
                        case MonoFilter monoRight:
                            return new OrFilter() { filters = (new[] { monoLeft }).Cast<FilterExpression>().Concat(new[] { monoRight }) };
                        default:
                            throw new Exception("Unupported Filter type");
                    }
                default:
                    throw new Exception("Unupported Filter type");
            }
        }
    }

    public abstract class MultiFilter : FilterExpression
    {
        internal IEnumerable<FilterExpression> filters;
    }

    public class AndFilter : MultiFilter
    {
        
    }

    public class OrFilter : MultiFilter
    {

    }

    public class MonoFilter : FilterExpression
    {

    }

    public class Expression
    {

    }
}
