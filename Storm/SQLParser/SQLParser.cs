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
    class ParseContext
    {
        public Query query;
        public SchemaNavigator navigator;
        public Command command;
    }

    class SQLParser
    {
        private ParseContext ctx;

        public void BuildFrom(TableTree fromTree)
        {
            var _q = new Query($"{fromTree.Entity.DBName} as {fromTree.Alias}");
            _q = fromTree.children.Aggregate(_q, (q, n) => ParseTableTree(fromTree, n, q));
            ctx.query = _q;
        }

        internal void BuildWhere(TableTree fromTree, Filter where)
        {
            
        }

        private Query ParseFilter(Filter filter, Query query)
        {
            switch (filter)
            {
                case AndFilter andFilter:
                    break;
                case OrFilter andFilter:
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
                            query.Where(ParseReferenceStringLeft(f),"<>", data.value);
                        else
                            query.WhereColumns(ParseReferenceStringLeft(f), "<>", ParseReferenceStringRight(f));
                        break;
                    }
                case GreaterToFilter f:
                    {
                        if (f.Right is DataFilterValue data)
                            query.Where(ParseReferenceStringLeft(f),">", data.value);
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
            var x = ctx.command.nodes[f.Left.Path];
            var name = f.Left.Path.Split('.').Last();
            var field = x.Entity.entityFields.FirstOrDefault(ef => ef.CodeName.ToLowerInvariant() == name.ToLowerInvariant());
            return $"{x.Alias}.{field.DBName}";
        }

        private string ParseReferenceStringRight(MonoFilter f)
        {
            var x = ctx.command.nodes[((ReferenceFilterValue)f.Right).Path];
            var name = f.Left.Path.Split('.').Last();
            var field = x.Entity.entityFields.FirstOrDefault(ef => ef.CodeName.ToLowerInvariant() == name.ToLowerInvariant());
            return $"{x.Alias}.{field.DBName}";
        }

        private Query ParseTableTree(TableTree parentTree, TableTree subTree, Query query)
        {
            if (subTree.Edge.OnExpression is null)
            {
                String sourceCol = resolveSourceColumnName(subTree);
                String targetCol = resolveTargetColumnName(subTree);
                query.LeftJoin($"{subTree.Entity.DBName} as {subTree.Alias}", $"{parentTree.Alias}.{sourceCol}", $"{subTree.Alias}.{targetCol}");
            }
            else
            {

            }
            return subTree.children.Aggregate(query, (q, n) => ParseTableTree(subTree, n, q));
        }

        private string resolveTargetColumnName(TableTree subTree)
        {
            string targetCol = string.Empty;
            var target = ctx.navigator.GetEntity(subTree.Edge.TargetID);
            if (target.entityFields != null && target.entityFields.Any())
            {
                var field = target
                    .entityFields
                    .FirstOrDefault(ef => ef.CodeName == subTree.Edge.On.Item2);
                if (field != null)
                    targetCol = field.DBName;
                else
                    throw new ArgumentException($"Error on parsing join condition for table {subTree.Entity.DBName}: no field found on entity {subTree.Edge.TargetID} with name {subTree.Edge.On.Item2}.");
            }
            else
            {
                targetCol = subTree.Edge.On.Item2;
            }

            return targetCol;
        }

        private string resolveSourceColumnName(TableTree subTree)
        {
            string sourceCol = string.Empty;
            var source = ctx.navigator.GetEntity(subTree.Edge.SourceID);
            if (source.entityFields != null && source.entityFields.Any())
            {
                var field = source
                    .entityFields
                    .FirstOrDefault(ef => ef.CodeName == subTree.Edge.On.Item1);
                if (field != null)
                    sourceCol = field.DBName;
                else
                    throw new ArgumentException($"Error on parsing join condition for table {subTree.Entity.DBName}: no field found on entity {subTree.Edge.SourceID} with name {subTree.Edge.On.Item1}.");
            }
            else
            {
                sourceCol = subTree.Edge.On.Item1;
            }

            return sourceCol;
        }
    }
}
