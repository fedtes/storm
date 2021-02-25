using System;
using System.Collections.Generic;
using System.Text;

namespace Storm
{
    public class NoPrimaryKeySpecifiedException : System.Exception
    {
        public NoPrimaryKeySpecifiedException(string message) : base(message)
        { }
    }

    public class SchemaOutOfDateException : System.Exception
    {

    }

    public class WrongSchemaDefinitionException : System.Exception
    {
        public WrongSchemaDefinitionException(string message) : base(message) { }
    }
}
