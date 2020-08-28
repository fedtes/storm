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
            base.query = ParseFilter(where, base.query, Op.And);
            return query;
        }

        private enum Op
        {
            And = 0,
            Or = 1
        }

        private Query ParseFilter(Filter filter, Query query, Op op)
        {
            switch (filter)
            {
                case AndFilter f:
                    if (op==Op.And)
                        query.Where(q => f.filters.Aggregate(q, (a,x) => ParseFilter(x,a,Op.And)));
                    else
                        query.OrWhere(q => f.filters.Aggregate(q, (a, x) => ParseFilter(x, a, Op.And)));
                    break;
                case OrFilter f:
                    if (op == Op.And)
                        query.Where(q => f.filters.Aggregate(q, (a, x) => ParseFilter(x, a, Op.Or)));
                    else
                        query.OrWhere(q => f.filters.Aggregate(q, (a, x) => ParseFilter(x, a, Op.Or)));
                    break;
                case EqualToFilter f:
                    {
                        if (op == Op.And)
                            if (f.Right is DataFilterValue data)
                                query.Where(ParseReferenceStringLeft(f), data.value);
                            else
                                query.WhereColumns(ParseReferenceStringLeft(f), "=", ParseReferenceStringRight(f));
                        else
                            if (f.Right is DataFilterValue data)
                                query.OrWhere(ParseReferenceStringLeft(f), data.value);
                            else
                                query.OrWhereColumns(ParseReferenceStringLeft(f), "=", ParseReferenceStringRight(f));
                        break;
                    }
                case NotEqualToFilter f:
                    {
                        if (op == Op.And)
                            if (f.Right is DataFilterValue data)
                                query.Where(ParseReferenceStringLeft(f), "<>", data.value);
                            else
                                query.WhereColumns(ParseReferenceStringLeft(f), "<>", ParseReferenceStringRight(f));
                        else
                            if (f.Right is DataFilterValue data)
                                query.OrWhere(ParseReferenceStringLeft(f), "<>", data.value);
                            else
                                query.OrWhereColumns(ParseReferenceStringLeft(f), "<>", ParseReferenceStringRight(f));

                        break;
                    }
                case GreaterToFilter f:
                    {
                        if (op == Op.And)
                            if (f.Right is DataFilterValue data)
                                query.Where(ParseReferenceStringLeft(f), ">", data.value);
                            else
                                query.WhereColumns(ParseReferenceStringLeft(f), ">", ParseReferenceStringRight(f));
                        else
                            if (f.Right is DataFilterValue data)
                                query.OrWhere(ParseReferenceStringLeft(f), ">", data.value);
                            else
                                query.OrWhereColumns(ParseReferenceStringLeft(f), ">", ParseReferenceStringRight(f));

                        break;
                    }
                case GreaterOrEqualToFilter f:
                    {
                        if (op == Op.And)
                            if (f.Right is DataFilterValue data)
                                query.Where(ParseReferenceStringLeft(f), ">=", data.value);
                            else
                                query.WhereColumns(ParseReferenceStringLeft(f), ">=", ParseReferenceStringRight(f));
                        else
                            if (f.Right is DataFilterValue data)
                                query.OrWhere(ParseReferenceStringLeft(f), ">=", data.value);
                            else
                                query.OrWhereColumns(ParseReferenceStringLeft(f), ">=", ParseReferenceStringRight(f));

                        break;
                    }
                case LessToFilter f:
                    {
                        if (op == Op.And)
                            if (f.Right is DataFilterValue data)
                                query.Where(ParseReferenceStringLeft(f), "<", data.value);
                            else
                                query.WhereColumns(ParseReferenceStringLeft(f), "<", ParseReferenceStringRight(f));
                        else
                            if (f.Right is DataFilterValue data)
                            query.OrWhere(ParseReferenceStringLeft(f), "<", data.value);
                            else
                                query.OrWhereColumns(ParseReferenceStringLeft(f), "<", ParseReferenceStringRight(f));

                        break;
                    }
                case LessOrEqualToFilter f:
                    {
                        if (op == Op.And)
                            if (f.Right is DataFilterValue data)
                                query.Where(ParseReferenceStringLeft(f), "<=", data.value);
                            else
                                query.WhereColumns(ParseReferenceStringLeft(f), "<=", ParseReferenceStringRight(f));
                        else
                            if (f.Right is DataFilterValue data)
                                query.OrWhere(ParseReferenceStringLeft(f), "<=", data.value);
                            else
                                query.OrWhereColumns(ParseReferenceStringLeft(f), "<=", ParseReferenceStringRight(f));
                        break;
                    }
                case LikeFilter f:
                    {
                        if (op == Op.And)
                            if (f.Right is DataFilterValue data)
                                query.Where(ParseReferenceStringLeft(f), "like", data.value);
                            else
                                query.WhereColumns(ParseReferenceStringLeft(f), "like", ParseReferenceStringRight(f));
                        else
                            if (f.Right is DataFilterValue data)
                                query.OrWhere(ParseReferenceStringLeft(f), "like", data.value);
                            else
                                query.OrWhereColumns(ParseReferenceStringLeft(f), "like", ParseReferenceStringRight(f));
                        break;
                    }
                case NotLikeFilter f:
                    {
                        if (op == Op.And)
                            if (f.Right is DataFilterValue data)
                                query.Where(ParseReferenceStringLeft(f), "not like", data.value);
                            else
                                query.WhereColumns(ParseReferenceStringLeft(f), "not like", ParseReferenceStringRight(f));
                        else
                            if (f.Right is DataFilterValue data)
                                query.OrWhere(ParseReferenceStringLeft(f), "not like", data.value);
                            else
                                query.OrWhereColumns(ParseReferenceStringLeft(f), "not like", ParseReferenceStringRight(f));
                        break;
                    }
                case InFilter f:
                    {
                        if (op == Op.And)
                            if (f.Right is DataFilterValue data)
                                query.WhereIn<Object>(ParseReferenceStringLeft(f), ((IEnumerable)data.value).Cast<Object>());
                            else
                                query.WhereColumns(ParseReferenceStringLeft(f), "in", ParseReferenceStringRight(f));
                        else
                            if (f.Right is DataFilterValue data)
                                query.OrWhereIn<Object>(ParseReferenceStringLeft(f), ((IEnumerable)data.value).Cast<Object>());
                            else
                                query.OrWhereColumns(ParseReferenceStringLeft(f), "in", ParseReferenceStringRight(f));
                        break;
                    }
                case NotInFilter f:
                    {
                        if (op == Op.And)
                            if (f.Right is DataFilterValue data)
                                query.WhereNotIn<Object>(ParseReferenceStringLeft(f), ((IEnumerable)data.value).Cast<Object>());
                            else
                                query.WhereColumns(ParseReferenceStringLeft(f), "not in", ParseReferenceStringRight(f));
                        else
                            if (f.Right is DataFilterValue data)
                            query.OrWhereNotIn<Object>(ParseReferenceStringLeft(f), ((IEnumerable)data.value).Cast<Object>());
                        else
                            query.OrWhereColumns(ParseReferenceStringLeft(f), "not in", ParseReferenceStringRight(f));
                        break;
                    }
                case IsNullFilter f:
                    {
                        if (op == Op.And)
                            query.WhereNull(ParseReferenceStringLeft(f));
                        else
                            query.OrWhereNull(ParseReferenceStringLeft(f));
                        break;
                    }
                case IsNotNullFilter f:
                    {
                        if (op== Op.And)
                            query.WhereNotNull(ParseReferenceStringLeft(f));
                        else
                            query.OrWhereNotNull(ParseReferenceStringLeft(f));
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
