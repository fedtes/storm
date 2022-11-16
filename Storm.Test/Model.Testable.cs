using System;
using System.Collections.Generic;
using System.Text;
using Storm.Schema;

namespace Storm.Test
{
    public class TestModel
    {
        [StormPrimaryKey]
        public int ID;

        public string SomeProperty { get; set; }

        [StormIgnore]
        public string IgnorableField;

        [StormColumnName("DifferentName")]
        public string SomeName;

        [StormDefaultIfNull("Some Default Value")]
        public string DefaultedField;

        [StormColumnType(System.Data.DbType.String, 50)]
        public string SpecifiedType;

        [StormColumnAccess(ColumnAccess.ReadOnly)]
        public string ReadOnlyField;

        [StormColumnAccess(ColumnAccess.CanInsert)]
        public string InsertOnly;

        [StormColumnAccess(ColumnAccess.Full)]
        public string FullControl;

    }

    public class NoPrimaryKeyTestModel
    {
        public string ThisShouldBePrimary;
    }
}
