using Storm.Schema;

namespace Storm
{
    public class Context
    {
        private readonly SchemaNavigator schemaNavigator;
        private readonly Logger logger;

        internal Context(SchemaNavigator schemaNavigator, Logger logger)
        {
            this.schemaNavigator = schemaNavigator;
            this.logger = logger;
        }

        internal SchemaNavigator Navigator => schemaNavigator;

        internal Logger GetLogger() => logger;
    }
}