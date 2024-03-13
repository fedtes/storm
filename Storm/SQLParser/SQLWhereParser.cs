using SqlKata;
using Storm.Execution;
using Storm.Filters;
using Storm.Schema;
using System;
using System.Collections;
using System.Linq;
using Storm.Origins;

namespace Storm.SQLParser
{
    class SQLWhereParser : SQLParser
    {
        protected OriginTree fromTree;
        protected Filter filter;

        public SQLWhereParser(OriginTree fromTree, Filter filter, Context ctx, Query query) : base(ctx, query)
        {
            this.fromTree = fromTree;
            this.filter = filter;
        }

        public override Query Parse()
        {
            base.query = ParseFilter<Query>(filter, base.query, Op.And);
            return query;
        }

        protected enum Op
        {
            And = 0,
            Or = 1
        }

        protected Q ParseFilter<Q>(Filter filter, BaseQuery<Q> query, Op op) where Q : BaseQuery<Q>
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
                                query.Where(ParseReferenceStringLeft(f), "like", "%" + (data.value == null ? "" : data.value) + "%");
                            else
                                query.WhereColumns(ParseReferenceStringLeft(f), "like", ParseReferenceStringRight(f));
                        else
                            if (f.Right is DataFilterValue data)
                                query.OrWhere(ParseReferenceStringLeft(f), "like", "%" + (data.value == null ? "" : data.value));
                            else
                                query.OrWhereColumns(ParseReferenceStringLeft(f), "like", ParseReferenceStringRight(f));
                        break;
                    }
                case NotLikeFilter f:
                    {
                        if (op == Op.And)
                            if (f.Right is DataFilterValue data)
                                query.Where(ParseReferenceStringLeft(f), "not like", "%" + (data.value == null ? "" : data.value) + "%");
                            else
                                query.WhereColumns(ParseReferenceStringLeft(f), "not like", ParseReferenceStringRight(f));
                        else
                            if (f.Right is DataFilterValue data)
                                query.OrWhere(ParseReferenceStringLeft(f), "not like", "%" + (data.value == null ? "" : data.value) + "%");
                            else
                                query.OrWhereColumns(ParseReferenceStringLeft(f), "not like", ParseReferenceStringRight(f));
                        break;
                    }
                case InFilter f:
                    {
                        if (op == Op.And)
                            if (f.Right is DataFilterValue data)
                                query.WhereIn<Object>(ParseReferenceStringLeft(f), ((IEnumerable)data.value).Cast<Object>());
                            else if (f.Right is SubQueryFilterValue subQueryExpr)
                            {
                                var nestedCmd = subQueryExpr.subquery(new SubQueryContext(this.ctx));
                                nestedCmd.ParseSQL();
                                query.WhereIn(ParseReferenceStringLeft(f), nestedCmd.query);
                            }
                            else
                                throw new Exception("Code should not pass here");
                        else
                            if (f.Right is DataFilterValue data)
                                query.OrWhereIn<Object>(ParseReferenceStringLeft(f), ((IEnumerable)data.value).Cast<Object>());
                            else if (f.Right is SubQueryFilterValue subQueryExpr)
                            {
                                var nestedCmd = subQueryExpr.subquery(new SubQueryContext(this.ctx));
                                nestedCmd.ParseSQL();
                                query.OrWhereIn(ParseReferenceStringLeft(f), nestedCmd.query);
                            }
                            else
                                throw new Exception("Code should not pass here");
                        break;
                    }
                case NotInFilter f:
                    {
                        if (op == Op.And)
                            if (f.Right is DataFilterValue data)
                                query.WhereNotIn<Object>(ParseReferenceStringLeft(f), ((IEnumerable)data.value).Cast<Object>());
                            else if (f.Right is SubQueryFilterValue subQueryExpr)
                            {
                                var nestedCmd = subQueryExpr.subquery(new SubQueryContext(this.ctx));
                                nestedCmd.ParseSQL();
                                query.WhereNotIn(ParseReferenceStringLeft(f), nestedCmd.query);
                            }
                            else
                                throw new Exception("Code should not pass here");
                        else
                            if (f.Right is DataFilterValue data)
                                query.OrWhereNotIn<Object>(ParseReferenceStringLeft(f), ((IEnumerable)data.value).Cast<Object>());
                            else if (f.Right is SubQueryFilterValue subQueryExpr)
                            {
                                var nestedCmd = subQueryExpr.subquery(new SubQueryContext(this.ctx));
                                nestedCmd.ParseSQL();
                                query.OrWhereNotIn(ParseReferenceStringLeft(f), nestedCmd.query);
                            }
                            else
                                throw new Exception("Code should not pass here");
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
            return (Q)query;
        }

        protected string ParseReferenceStringLeft(MonoFilter f)
        {
            ReferenceFilterValue rfv = (ReferenceFilterValue)f.Left;
            return ParseReferenceFilterValue(rfv);
        }
        protected string ParseReferenceStringRight(MonoFilter f)
        {
            ReferenceFilterValue rfv = (ReferenceFilterValue)f.Right;
            return ParseReferenceFilterValue(rfv);
        }

        protected virtual string ParseReferenceFilterValue(ReferenceFilterValue rfv)
        {
            var ps = rfv.Path.Split('.');
            var path = ps.Take(ps.Length - 1);
            var name = ps.Last();
            var fn = fromTree.Resolve(path);
            var field = fn.Entity.SimpleProperties.FirstOrDefault(ef => ef.CodeName.ToLowerInvariant() == name.ToLowerInvariant());
            var alias = String.IsNullOrEmpty(fn.Alias) ? "" : fn.Alias + ".";
            return $"{alias}{field.DBName}";
        }
    }
}
