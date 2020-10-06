﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Storm.Execution.Results;
using Storm.Helpers;
using Storm.Schema;

namespace Storm.Execution
{
    public class SelectCommand : Command<SelectCommand>
    {
        public class SelectField
        {
            public String fullPath;
            public String CodeName { get => entityField.CodeName; }
            public String DBName { get => entityField.DBName; }
            public FromNode node;
            public EntityField entityField;
        }

        const String valudationPath = @"^([^ .{},[\]*]\.?)*([^*.[\]]+|\*)$";
        protected List<SelectField> selectFields = new List<SelectField>();

        public SelectCommand(SchemaNavigator navigator, string from) : base(navigator, from) { }

        internal IEnumerable<ValueTuple<String[], String>> ValidateSelectPath(string requestPath)
        {
            if (!Regex.IsMatch(requestPath, valudationPath))
            {
                throw new ArgumentException($"Invalid select path {requestPath}");
            }
            var pt = requestPath.LastIndexOf('.');
            var head = requestPath.Substring(0, pt == -1 ? 0 : pt);
            var tail = requestPath.Substring(pt == -1 ? 0 : pt);

            if (tail.IndexOf("{") != -1 && tail.IndexOf("}") != -1) // residual{list, of, fields}
            {
                var residual = tail.Substring(0, tail.IndexOf("{")).Trim('.');
                var tail_1 = tail.Substring(tail.IndexOf("{") + 1);
                var stringFields = tail_1.Substring(0, tail_1.IndexOf("}"));
                var fields = stringFields.Split(',');
                var path = (head + residual).Split('.').Select(s => s.Trim()).ToArray();
                return fields.Select(f => f.Trim()).Select(f => (path, f));
            }
            else if (tail.IndexOf("{") == -1 && tail.IndexOf("}") == -1)
            {
                var path = head.Split('.').Select(s => s.Trim()).ToArray();
                var field = tail.Trim('.');
                return (new[] { field }).Select(f => f.Trim()).Select(f => (path, f));
            }
            else
            {
                throw new ArgumentException($"Invalid select path {requestPath}");
            }
        }

        public SelectCommand Select(string requestPath)
        {
            var p = ValidateSelectPath(requestPath);

            foreach (var item in p)
            {
                var x = from.Resolve(item.Item1);
                IEnumerable<SelectField> fields;
                if (item.Item2 != "*") 
                {
                    fields = x.Entity.entityFields
                        .Where(ef => ef.CodeName == item.Item2)
                        .Select(ef => new SelectField() { fullPath = $"{x.FullPath}.{ef.CodeName}", entityField=ef, node = x });
                }
                else //wildcard = select all
                {
                    fields = x.Entity.entityFields
                        .Select(ef => new SelectField() { fullPath = $"{x.FullPath}.{ef.CodeName}", entityField=ef, node = x });
                }
                selectFields.AddRange(fields);
            }
            return this;
        }

        internal override void ParseSQL()
        {
            base.ParseSQL();

            foreach (var field in selectFields)
            {
                base.query.Select($"{field.node.Alias}.{field.DBName} AS {field.node.Alias}${field.CodeName}");
            }
        }

        internal override object Read(IDataReader dataReader)
        {
            StormResult sr = new StormResult(this.rootEntity);
            var metadata = this.selectFields.Select(f => new ReaderMetadata() { FullPath = f.fullPath, EntityField = f.entityField, Alias = f.node.Alias });
            sr.ReadData(dataReader, metadata);
            return sr;
        }

        public new StormResult Execute()
        {
            return (StormResult)base.Execute();
        }
    }
}