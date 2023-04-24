using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using SqlKata;
using Storm.Helpers;
using Storm.Schema;
using Storm.SQLParser;

namespace Storm.Execution
{
    public class SelectCommand : Command<SelectCommand>
    {
        protected List<SelectNode> selectFields = new List<SelectNode>();
        internal Query CoreQuery = null;

        internal SelectCommand(Context ctx, string from) : base(ctx, from) { }

        /// <summary>
        /// Define the output column of the command by declaring the path of the field to get such my.path.to.field. 
        /// To request multiple columns invoke this method that many times.
        /// </summary>
        /// <remarks>
        /// Allow also the following syntax my.path.to.{field1, field2} to extract more field at once and my.path.to.* to get all
        /// </remarks>
        /// <param name="requestPath"></param>
        /// <returns></returns>
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

            ((BaseCommand)this).CommandLog(LogLevel.Info, "SelectCommand", $"{{\"Action\":\"Select\", \"Path\":\"{Util.JSONClean(requestPath)}\"}}");
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
                CoreQuery = query.Clone();
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

        public async Task<StormDataSet> Execute()
        {
            return await base.InternalExecute<StormDataSet>();
        }


        internal override void PopulateCommandText(IDbCommand cmd, SqlResult result)
        {
            if (this.paging != (-1, -1))
            {
                SqlResult coreQueryResult = compiler.Compile(CoreQuery);
                string coreSql = coreQueryResult.Sql;
                string fullSql = @"SELECT COUNT(*) " + Environment.NewLine
                    + "FROM (" + Environment.NewLine
                    + coreSql
                    + ") A;";
                cmd.CommandText = result.Sql + ";" + Environment.NewLine + fullSql;
            }
            else
            {
                base.PopulateCommandText(cmd, result);
            }
        }

        protected override async Task<T> ExecuteQuery<T>(DbCommand cmd)
        {
            if (this.paging != (-1, -1))
            {
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    this.CommandLog(LogLevel.Trace, "Command", $"{{\"Action\":\"Executed\", \"Time\":\"{sw.ElapsedMilliseconds}\" }}");
                    StormDataSet r = (StormDataSet)Read(reader);

                    if (reader.NextResult() && reader.Read())
                    {
                        r.rowCount = reader.GetInt32(0);
                    }
                    return (T)(object)r;
                }

            }
            else
            {
                return await base.ExecuteQuery<T>(cmd);
            }
        }
    }
}
