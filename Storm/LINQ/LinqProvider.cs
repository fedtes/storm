using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Parsing.Structure;
using SqlKata;
using SqlKata.Compilers;
using Storm.Execution;

namespace Storm.Linq
{
    public class StormQuerable<T> : QueryableBase<T>
    {
        public StormQuerable(IQueryProvider provider) : base(provider)
        {
        }

        public StormQuerable(IQueryParser queryParser, IQueryExecutor executor) : base(queryParser, executor)
        {
        }

        public StormQuerable(IQueryProvider provider, Expression expression) : base(provider, expression)
        {
        }
    }

    internal class StormQueryExecutor : QueryModelVisitorBase, IQueryExecutor
    {
        private readonly StormConnection connection;
        private readonly string entityIdentifier;

        private GetCommand command;

        public StormQueryExecutor(StormConnection connection, string EntityIdentifier){
            this.connection = connection;
            entityIdentifier = EntityIdentifier;
        }

        public IEnumerable<T> ExecuteCollection<T>(QueryModel queryModel)
        {
            command = new GetCommand(connection.ctx, entityIdentifier);
            this.VisitQueryModel(queryModel);
            command.ParseSQL();
            var compiler = new SqlServerCompiler();
            SqlResult result = compiler.Compile(command.query);
            
            return (IEnumerable<T>)new List<Entity>() {new Entity() {Result= result.Sql}};
        }

        public T ExecuteScalar<T>(QueryModel queryModel)
        {
            throw new NotImplementedException();
        }

        public T ExecuteSingle<T>(QueryModel queryModel, bool returnDefaultWhenEmpty)
        {
            throw new NotImplementedException();
        }

        public override void VisitWhereClause(WhereClause whereClause, QueryModel queryModel, int index)
        {
            WhereClauseVisitor _wVisitor = new WhereClauseVisitor();
            var f = _wVisitor.Parse(whereClause, whereClause.Predicate, queryModel, index);
            command.Where((_) => f);
        }

        public override void VisitAdditionalFromClause(AdditionalFromClause fromClause, QueryModel queryModel, int index)
        {
            base.VisitAdditionalFromClause(fromClause, queryModel, index);
        }
        protected override void VisitBodyClauses(ObservableCollection<IBodyClause> bodyClauses, QueryModel queryModel)
        {
            base.VisitBodyClauses(bodyClauses, queryModel);
        }

        public override void VisitGroupJoinClause(GroupJoinClause groupJoinClause, QueryModel queryModel, int index)
        {
            base.VisitGroupJoinClause(groupJoinClause, queryModel, index);
        }

        public override void VisitJoinClause(JoinClause joinClause, QueryModel queryModel, GroupJoinClause groupJoinClause)
        {
            base.VisitJoinClause(joinClause, queryModel, groupJoinClause);
        }

        public override void VisitJoinClause(JoinClause joinClause, QueryModel queryModel, int index)
        {
            base.VisitJoinClause(joinClause, queryModel, index);
        }

        public override void VisitMainFromClause(MainFromClause fromClause, QueryModel queryModel)
        {
            base.VisitMainFromClause(fromClause, queryModel);
        }

        public override void VisitOrderByClause(OrderByClause orderByClause, QueryModel queryModel, int index)
        {
            base.VisitOrderByClause(orderByClause, queryModel, index);
        }

        public override void VisitOrdering(Ordering ordering, QueryModel queryModel, OrderByClause orderByClause, int index)
        {
            base.VisitOrdering(ordering, queryModel, orderByClause, index);
        }

        protected override void VisitOrderings(ObservableCollection<Ordering> orderings, QueryModel queryModel, OrderByClause orderByClause)
        {
            base.VisitOrderings(orderings, queryModel, orderByClause);
        }

        public override void VisitResultOperator(ResultOperatorBase resultOperator, QueryModel queryModel, int index)
        {
            base.VisitResultOperator(resultOperator, queryModel, index);
        }

        protected override void VisitResultOperators(ObservableCollection<ResultOperatorBase> resultOperators, QueryModel queryModel)
        {
            base.VisitResultOperators(resultOperators, queryModel);
        }

        public override void VisitSelectClause(SelectClause selectClause, QueryModel queryModel)
        {
            base.VisitSelectClause(selectClause, queryModel);
        }
    }


    public class Entity
    {
        public EntityProperty this[string path] => new EntityProperty();

        public class EntityProperty
        {

            public static bool operator > (EntityProperty left, object right) => true;
            public static bool operator < (EntityProperty left, object right) => true;
            public static bool operator >= (EntityProperty left, object right) => true;
            public static bool operator <= (EntityProperty left, object right) => true;
            public static bool operator ==(EntityProperty left, object right) => true;
            public static bool operator !=(EntityProperty left, object right) => true;
            public bool Like(string s) => true;
            public bool NotLike(string s) => true;
            public bool IsNull() => true;
            public bool IsNotNull() => true;

            public override bool Equals(object obj) => base.Equals(obj);
            public override int GetHashCode() => base.GetHashCode();
        }

        public string Result {get; internal set;}
    }
}