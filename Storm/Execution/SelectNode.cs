using Storm.Helpers;
using Storm.Schema;
using System;
using System.Collections.Generic;
using System.Text;

namespace Storm.Execution
{
    public class SelectNode
    {
        public FromNode FromNode;
        public SchemaNode OwnerEntity => FromNode.Entity;
        public EntityField EntityField;
        public FieldPath FullPath;
        public String Alias => FromNode.Alias;
        public String CodeName { get => EntityField.CodeName; }
        public String DBName { get => EntityField.DBName; }
        public object FieldDefault => EntityField.DefaultIfNull != null ? 
            EntityField.DefaultIfNull : 
            (EntityField.CodeType.IsValueType ? TypeHelper.DefValues[EntityField.CodeType] : null);
    }
}
