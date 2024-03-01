using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using Storm.Filters;

namespace Storm.Linq 
{
    internal abstract class WhereClauseVisitorBase<T> where T : Expression
    {

        protected bool IsCallFromQuerySource(MethodCallExpression  methodCall)
        {
            if (methodCall.Object.GetType() == typeof(QuerySourceReferenceExpression)){
                return true;
            } else {
                if (typeof(MethodCallExpression).IsAssignableFrom(methodCall.Object.GetType())) {
                    return IsCallFromQuerySource((MethodCallExpression)methodCall.Object);
                } 
            }
            return false;
        }

        protected bool IsSourceAccess(Expression exp)
        {
            MethodCallExpression me = (MethodCallExpression)exp;
            return me.Method == typeof(Entity).GetMethod("get_Item") && IsCallFromQuerySource(me);
        }

        protected FilterValue GetSourceAccessArgument(Expression exp)
        {
            MethodCallExpression me = (MethodCallExpression)exp;
            var arg = (ConstantExpression)me.Arguments.First();
            return new ReferenceFilterValue(arg.Value.ToString());
        }

        protected bool IsOperation_IsNull(Expression exp)
        {
            MethodCallExpression me = (MethodCallExpression)exp;
            return me.Method == typeof(Entity.EntityProperty).GetMethod("IsNull") && IsCallFromQuerySource(me);
        }

        protected bool IsOperation_IsNotNull(Expression exp)
        {
            MethodCallExpression me = (MethodCallExpression)exp;
            return me.Method == typeof(Entity.EntityProperty).GetMethod("IsNotNull") && IsCallFromQuerySource(me);
        }

        protected bool IsOperation_Like(Expression exp)
        {
            MethodCallExpression me = (MethodCallExpression)exp;
            return me.Method == typeof(Entity.EntityProperty).GetMethod("Like") && IsCallFromQuerySource(me);
        }

        protected bool IsOperation_NotLike(Expression exp)
        {
            MethodCallExpression me = (MethodCallExpression)exp;
            return me.Method == typeof(Entity.EntityProperty).GetMethod("NotLike") && IsCallFromQuerySource(me);
        }

        
        protected FilterValue GetReferenceOrData(Expression right)
        {
            return right.NodeType switch 
            {
                ExpressionType.Constant => new DataFilterValue(((ConstantExpression)right).Value),

                _ => GetReference(right)
            }; 
        }

        protected ReferenceFilterValue GetReference(Expression left)
        {
            if (left.NodeType == ExpressionType.Call) 
            {
                MethodCallExpression me = (MethodCallExpression)left;
                if (me.Method == typeof(Entity).GetMethod("get_Item") && me.Object.GetType() == typeof(QuerySourceReferenceExpression))
                {
                    var arg = (ConstantExpression)me.Arguments.First();
                    return new ReferenceFilterValue(arg.Value.ToString());
                }
            }

            throw new NotImplementedException();
        }


        public abstract Filter Parse(WhereClause whereClause,T Predicate, QueryModel queryModel, int index);
    }

    internal class WhereClauseVisitor : WhereClauseVisitorBase<Expression>
    {
        public override Filter Parse(WhereClause whereClause,Expression Predicate, QueryModel queryModel, int index)
        {
            return Predicate switch
            {
                BinaryExpression p => new BinaryWhereClauseVisitor().Parse(whereClause, p, queryModel, index),

                UnaryExpression p => new UnaryWhereClauseVisitor().Parse(whereClause, p, queryModel, index),

                MethodCallExpression p => new MethodCallWhereClauseVisitor().Parse(whereClause, p, queryModel, index),

                _ => throw new NotImplementedException(),
            };
        }
    }


    internal class BinaryWhereClauseVisitor : WhereClauseVisitorBase<BinaryExpression>
    {
        public override Filter Parse(WhereClause w,BinaryExpression p, QueryModel q, int i)
        {
            return p.NodeType switch
            {
                ExpressionType.AndAlso => new WhereClauseVisitor().Parse(w, p.Left, q, i) * new WhereClauseVisitor().Parse(w, p.Right, q, i),

                ExpressionType.OrElse => new WhereClauseVisitor().Parse(w, p.Left, q, i) + new WhereClauseVisitor().Parse(w, p.Right, q, i),

                ExpressionType.Equal => new EqualToFilter() { Left = GetReference(p.Left), Right = GetReferenceOrData(p.Right)},

                ExpressionType.NotEqual => new NotEqualToFilter() { Left = GetReference(p.Left), Right = GetReferenceOrData(p.Right) },

                ExpressionType.GreaterThan => new GreaterToFilter() { Left = GetReference(p.Left), Right = GetReferenceOrData(p.Right) },

                ExpressionType.GreaterThanOrEqual => new GreaterOrEqualToFilter() { Left = GetReference(p.Left), Right = GetReferenceOrData(p.Right) },

                ExpressionType.LessThan => new LessToFilter() { Left = GetReference(p.Left), Right = GetReferenceOrData(p.Right) },

                ExpressionType.LessThanOrEqual => new LessOrEqualToFilter() { Left = GetReference(p.Left), Right = GetReferenceOrData(p.Right) },

                _ => throw new NotImplementedException(),
            };
        }


    }

    internal class UnaryWhereClauseVisitor : WhereClauseVisitorBase<UnaryExpression>
    {
        public override Filter Parse(WhereClause whereClause,UnaryExpression Predicate, QueryModel queryModel, int index)
        {
            throw new NotImplementedException();
        }
    }

    internal class MethodCallWhereClauseVisitor : WhereClauseVisitorBase<MethodCallExpression>
    {
        public override Filter Parse(WhereClause whereClause, MethodCallExpression Predicate, QueryModel queryModel, int index)
        {
            return Predicate switch {
                MethodCallExpression p when this.IsOperation_IsNull(p) => new IsNullFilter() {Left = this.GetReference(p.Object) },

                MethodCallExpression p when this.IsOperation_IsNotNull(p) => new IsNotNullFilter() {Left = this.GetReference(p.Object) },

                MethodCallExpression p when this.IsOperation_Like(p) => new LikeFilter() {Left = this.GetReference(p.Object), Right = GetReferenceOrData(p.Arguments.First()) },

                MethodCallExpression p when this.IsOperation_NotLike(p) => new NotLikeFilter() {Left = this.GetReference(p.Object), Right = GetReferenceOrData(p.Arguments.First()) },

                _ => throw new NotImplementedException()
            };
        }
    }


}