using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using SqlKata;
using Storm.Execution;
using Storm.Helpers;
using Storm.Schema;
using Storm.SQLParser;

namespace Storm.Execution
{
    public class SelectCommand : Command<SelectCommand>
    {
        const String valudationPath = @"^([^ .{},[\]*]\.?)*([^*.[\]]+|\*)$";
        protected List<SelectNode> selectFields = new List<SelectNode>();
        internal Query coreQuery = null;

        public SelectCommand(Context ctx, string from) : base(ctx, from) { }

        public SelectCommand Select(string requestPath)
        {
            var _requestPath = new EntityPath(from.root.Entity.ID, requestPath).Path;
            var p = SelectCommandHelper.ValidatePath(_requestPath);

            foreach (var item in p)
            {
                var x = from.Resolve(item.Item1);
                IEnumerable<SelectNode> fields;
                if (item.Item2 != "*") 
                {
                    fields = x.Entity.entityFields
                        .Where(ef => ef.CodeName == item.Item2)
                        .Select(ef => {
                            return new SelectNode()
                            {
                                FullPath = new FieldPath(x.FullPath.Root, x.FullPath.Path, ef.CodeName),
                                EntityField = ef,
                                FromNode = x
                            };
                        });
                }
                else //wildcard = select all
                {
                    fields = x.Entity.entityFields
                        .Select(ef => {
                            return new SelectNode()
                            {
                                FullPath = new FieldPath(x.FullPath.Root, x.FullPath.Path, ef.CodeName),
                                EntityField = ef,
                                FromNode = x
                            };
                        });
                }
                selectFields.AddRange(fields);
            }

            ((BaseCommand)this).CommandLog(LogLevel.Info, "SelectCommand", $"{{\"Action\":\"Select\", \"Path\":\"{requestPath}\"}}");
            return this;
        }

        internal override void ParseSQL()
        {
            SQLWhereParser whereParser = new SQLWhereParser(from, where, ctx, query);
            query = whereParser.Parse();

            SQLFromParser fromParser = new SQLFromParser(from, ctx, query);
            query = fromParser.Parse();

            foreach (var field in selectFields)
            {
                base.query.Select($"{field.Alias}.{field.DBName} AS {field.Alias}${field.CodeName}");
            }

            if (paging != (-1,-1))
            {
                coreQuery = query.Clone();
                SQLPagingParser pagingParser = new SQLPagingParser(paging, ctx, query);
                query = pagingParser.Parse();
            }

            if (orderBy != null && orderBy.Any())
            {
                SQLOrderByParser orderByParser = new SQLOrderByParser(orderBy, ctx, query);
                query = orderByParser.Parse();
            } 
            else if (paging != (-1, -1))
            {
                var pk = new SelectNode()
                {
                    EntityField = this.from.root.Entity.PrimaryKey,
                    FullPath = new FieldPath(this.rootEntity, String.Empty, this.from.root.Entity.PrimaryKey.CodeName),
                    FromNode = this.from.root
                };
                SQLOrderByParser orderByParser = 
                    new SQLOrderByParser(new List<(SelectNode, bool)>() { { (pk, true) } }, ctx, query);
                query = orderByParser.Parse();
            }

            
        }

        internal override object Read(IDataReader dataReader)
        {
            StormDataSet sr = new StormDataSet(this.rootEntity);
            sr.ReadData(dataReader, this.selectFields);
            return sr;
        }

        public new StormDataSet Execute()
        {
            return (StormDataSet)base.Execute();
        }


        internal override void PopulateCommandText(IDbCommand cmd, SqlResult result)
        {
            if (this.paging != (-1, -1))
            {
                SqlResult coreQueryResult = compiler.Compile(coreQuery);
                string coreSql = coreQueryResult.Sql;
                string fullSql = @"SELECT COUNT(*) " + Environment.NewLine
                    + "(" + Environment.NewLine
                    + coreSql
                    + ") A;";
                cmd.CommandText = result.Sql + ";" + Environment.NewLine + fullSql;
            }
            else
            {
                base.PopulateCommandText(cmd, result);
            }
        }

        protected override object ExecuteQuery(IDbCommand cmd)
        {
            if (this.paging != (-1, -1))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    this.CommandLog(LogLevel.Trace, "Command", $"{{\"Action\":\"Executed\", \"Time\":\"{sw.ElapsedMilliseconds}\" }}");
                    StormDataSet r = (StormDataSet)Read(reader);

                    if (reader.NextResult() && reader.Read())
                    {
                        r.rowCount = reader.GetInt32(0);
                    }
                    return r;
                }

            }
            else
            {
                return base.ExecuteQuery(cmd);
            }
        }
    }
}
