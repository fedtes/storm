using SqlKata;
using Storm.Execution;
using Storm.Filters;
using Storm.Schema;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Storm.SQLParser
{
    class SQLWhereParser : SQLParser
    {
        protected FromTree fromTree;
        protected Filter where;

        public SQLWhereParser(FromTree fromTree, Filter where, SchemaNavigator schemaNavigator, Query query) : base(schemaNavigator, query)
        {
            this.fromTree = fromTree;
            this.where = where;
        }

        public override Query Parse()
        {
            base.query = ParseFilter(where, base.query);
            return query;
        }

        private Query ParseFilter(Filter filter, Query query)
        {
            switch (filter)
            {
                case AndFilter f:
                    query.Where(q => ParseFilter(f, q));
                    break;
                case OrFilter f:
                    query.OrWhere(q => ParseFilter(f, q));
                    break;
                case EqualToFilter f:
                    {
                        if (f.Right is DataFilterValue data)
                            query.Where(ParseReferenceStringLeft(f), data.value);
                        else
                            query.WhereColumns(ParseReferenceStringLeft(f), "=", ParseReferenceStringRight(f));
                        break;
                    }
                case NotEqualToFilter f:
                    {
                        if (f.Right is DataFilterValue data)
                            query.Where(ParseReferenceStringLeft(f), "<>", data.value);
                        else
                            query.WhereColumns(ParseReferenceStringLeft(f), "<>", ParseReferenceStringRight(f));
                        break;
                    }
                case GreaterToFilter f:
                    {
                        if (f.Right is DataFilterValue data)
                            query.Where(ParseReferenceStringLeft(f), ">", data.value);
                        else
                            query.WhereColumns(ParseReferenceStringLeft(f), ">", ParseReferenceStringRight(f));
                        break;
                    }
                case GreaterOrEqualToFilter f:
                    {
                        if (f.Right is DataFilterValue data)
                            query.Where(ParseReferenceStringLeft(f), ">=", data.value);
                        else
                            query.WhereColumns(ParseReferenceStringLeft(f), ">=", ParseReferenceStringRight(f));
                        break;
                    }
                case LessToFilter f:
                    {
                        if (f.Right is DataFilterValue data)
                            query.Where(ParseReferenceStringLeft(f), "<", data.value);
                        else
                            query.WhereColumns(ParseReferenceStringLeft(f), "<", ParseReferenceStringRight(f));
                        break;
                    }
                case LessOrEqualToFilter f:
                    {
                        if (f.Right is DataFilterValue data)
                            query.Where(ParseReferenceStringLeft(f), "<=", data.value);
                        else
                            query.WhereColumns(ParseReferenceStringLeft(f), "<=", ParseReferenceStringRight(f));
                        break;
                    }
                case LikeFilter f:
                    {
                        if (f.Right is DataFilterValue data)
                            query.WhereLike(ParseReferenceStringLeft(f), data.value);
                        else
                            query.WhereColumns(ParseReferenceStringLeft(f), "like", ParseReferenceStringRight(f));
                        break;
                    }
                case NotLikeFilter f:
                    {
                        if (f.Right is DataFilterValue data)
                            query.WhereNotLike(ParseReferenceStringLeft(f), data.value);
                        else
                            query.WhereColumns(ParseReferenceStringLeft(f), "not like", ParseReferenceStringRight(f));
                        break;
                    }
                case InFilter f:
                    {
                        if (f.Right is DataFilterValue data)
                            query.WhereIn<Object>(ParseReferenceStringLeft(f), ((IEnumerable)data.value).Cast<Object>());
                        else
                            query.WhereColumns(ParseReferenceStringLeft(f), "in", ParseReferenceStringRight(f));
                        break;
                    }
                case NotInFilter f:
                    {
                        if (f.Right is DataFilterValue data)
                            query.WhereNotIn<Object>(ParseReferenceStringLeft(f), ((IEnumerable)data.value).Cast<Object>());
                        else
                            query.WhereColumns(ParseReferenceStringLeft(f), "not in", ParseReferenceStringRight(f));
                        break;
                    }
                case IsNullFilter f:
                    {
                        query.WhereNull(ParseReferenceStringLeft(f));
                        break;
                    }
                case IsNotNullFilter f:
                    {
                        query.WhereNotNull(ParseReferenceStringLeft(f));
                        break;
                    }
                default:
                    break;
            }
            return query;
        }

        private string ParseReferenceStringLeft(MonoFilter f)
        {
            ReferenceFilterValue rfv = (ReferenceFilterValue)f.Left;
            return ParseReferenceFilterValue(rfv);
        }
        private string ParseReferenceStringRight(MonoFilter f)
        {
            ReferenceFilterValue rfv = (ReferenceFilterValue)f.Right;
            return ParseReferenceFilterValue(rfv);
        }

        private string ParseReferenceFilterValue(ReferenceFilterValue rfv)
        {
            var ps = rfv.Path.Split('.');
            var path = ps.Take(ps.Length - 1);
            var name = ps.Last();
            var fn = fromTree.Resolve(path);
            var field = fn.Entity.entityFields.FirstOrDefault(ef => ef.CodeName.ToLowerInvariant() == name.ToLowerInvariant());
            return $"{fn.Alias}.{field.DBName}";
        }
    }
}
