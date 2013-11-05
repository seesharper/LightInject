using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
#if NET35
using System.Collections.ObjectModel;
#endif

namespace ExpressionTools
{
    /// <summary>
    /// Represents a set of extension methods that adds search and replace capabilities to expression trees.   
    /// </summary>
    public static class ExpressionExtensions
    {
        /// <summary>
        /// Returns a list of <typeparamref name="TExpression"/> instances
        /// that matches the <paramref name="predicate"/>.
        /// </summary>
        /// <typeparam name="TExpression">The type of <see cref="Expression"/>
        /// to search for.</typeparam>
        /// <param name="expression">The <see cref="Expression"/> that represents the sub tree for which to start searching.</param>
        /// <param name="predicate">The <see cref="Func{T,TResult}"/> used to filter the result</param>
        /// <returns>A list of <see cref="Expression"/> instances that matches the given predicate.</returns>
        public static IEnumerable<TExpression> Find<TExpression>(this Expression expression, Func<TExpression, bool> predicate) where TExpression : Expression
        {
            var finder = new ExpressionFinder<TExpression>();
            return finder.Find(expression, predicate);
        }

        /// <summary>
        /// Searches for expressions using the given <paramref name="predicate"/> and replaces matches with
        /// the result from the <paramref name="replaceWith"/> delegate.
        /// </summary>
        /// <typeparam name="TTargetExpression">The type of <see cref="Expression"/> to search for.</typeparam>
        /// <param name="expression">The <see cref="Expression"/> that represents the sub tree
        /// for which to start searching.</param>
        /// <param name="predicate">The <see cref="Func{T,TResult}"/> used to filter the result</param>
        /// <param name="replaceWith">The <see cref="Func{T,TResult}"/> used to specify the replacement expression.</param>
        /// <returns>The modified <see cref="Expression"/> tree.</returns>
        public static Expression Replace<TTargetExpression>(this Expression expression, Func<TTargetExpression, bool> predicate, Func<TTargetExpression, Expression> replaceWith) where TTargetExpression : Expression
        {
            var replacer = new ExpressionReplacer<TTargetExpression>();
            return replacer.Replace(expression, predicate, replaceWith);
        }

        /// <summary>
        /// Determines if the <paramref name="predicate"/> yields a match in the target <paramref name="expression"/>.
        /// </summary>
        /// <typeparam name="TExpression">The type of <see cref="Expression"/>
        /// to search for.</typeparam>
        /// <param name="expression">The <see cref="Expression"/> that represents the sub tree for which to start searching.</param>
        /// <param name="predicate">The <see cref="Func{T,TResult}"/> used to filter the result</param>
        /// <returns>A list of <see cref="Expression"/> instances that matches the given predicate.</returns>
        public static bool Contains<TExpression>(this Expression expression, Func<TExpression, bool> predicate) where TExpression : Expression
        {
            var finder = new ExpressionFinder<TExpression>();
            return finder.Find(expression, predicate).Any();
        }

        /// <summary>
        /// Merges two <see cref="Expression{TDelegate}"/> instances.  
        /// </summary>
        /// <typeparam name="T">The type of delegate that the <see cref="Expression{TDelegate}"/> represents.</typeparam>
        /// <param name="first">The first expression.</param>
        /// <param name="second">The second expression.</param>
        /// <param name="merge">A function delegate used to merge the two expression.</param>
        /// <returns>A <see cref="Expression{TDelegate}"/> instance that represents the <paramref name="first"/> and <paramref name="second"/>
        /// expression merged into one.</returns>
        public static Expression<T> Merge<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
        {
            Expression<T> second1 = second;
            var map = first.Parameters.Select((f, i) => new { f, s = second1.Parameters[i] }).ToDictionary(p => p.s, p => p.f);
            second = (Expression<T>)second.Replace<ParameterExpression>(p => true, pe => map.ContainsKey(pe) ? map[pe] : pe);
            return Expression.Lambda<T>(merge(first.Body, second.Body), first.Parameters);
        }

        /// <summary>
        /// Creates a new <see cref="Expression{TDelegate}"/> instance that represents an AND between the<paramref name="first"/> and 
        /// <paramref name="second"/> expression.
        /// </summary>
        /// <typeparam name="T">The type of delegate that the <see cref="Expression{TDelegate}"/> represents.</typeparam>
        /// <param name="first">The first expression.</param>
        /// <param name="second">The second expression.</param>
        /// <returns>A <see cref="Expression{TDelegate}"/> instance that represents the <paramref name="first"/> and <paramref name="second"/>
        /// expression merged into one.</returns>
        public static Expression<T> And<T>(this Expression<T> first, Expression<T> second)
        {
            return first.Merge(second, Expression.AndAlso);
        }

        /// <summary>
        /// Creates a new <see cref="Expression{TDelegate}"/> instance that represents an OR between the<paramref name="first"/> and 
        /// <paramref name="second"/> expression.
        /// </summary>
        /// <typeparam name="T">The type of delegate that the <see cref="Expression{TDelegate}"/> represents.</typeparam>
        /// <param name="first">The first expression.</param>
        /// <param name="second">The second expression.</param>
        /// <returns>A <see cref="Expression{TDelegate}"/> instance that represents the <paramref name="first"/> and <paramref name="second"/>
        /// expression merged into one.</returns>
        public static Expression<T> Or<T>(this Expression<T> first, Expression<T> second)
        {
            return first.Merge(second, Expression.OrElse);
        }

    }




    /// <summary>
    /// A class used to search for <see cref="Expression"/> instances. 
    /// </summary>
    /// <typeparam name="TExpression">The type of <see cref="Expression"/> to search for.</typeparam>
    public class ExpressionFinder<TExpression> : ExpressionVisitor where TExpression : Expression
    {
        private readonly IList<TExpression> _result = new List<TExpression>();
        private Func<TExpression, bool> _predicate;

        /// <summary>
        /// Returns a list of <typeparamref name="TExpression"/> instances that matches the <paramref name="predicate"/>.
        /// </summary>
        /// <param name="expression">The <see cref="Expression"/> that represents the sub tree for which to start searching.</param>
        /// <param name="predicate">The <see cref="Func{T,TResult}"/> used to filter the result</param>
        /// <returns>A list of <see cref="Expression"/> instances that matches the given predicate.</returns>
        public IEnumerable<TExpression> Find(Expression expression, Func<TExpression, bool> predicate)
        {
            _result.Clear();
            _predicate = predicate;
            Visit(expression);
            return _result;
        }

        /// <summary>
        /// Visits each node of the <see cref="Expression"/> tree checks 
        /// if the current expression matches the predicate.         
        /// </summary>
        /// <param name="expression">The <see cref="Expression"/> currently being visited.</param>
        /// <returns><see cref="Expression"/></returns>
        public override Expression Visit(Expression expression)
        {
            if (IsTargetExpressionType(expression) && MatchesExpressionPredicate(expression))
                _result.Add((TExpression)expression);
            return base.Visit(expression);
        }

        private bool MatchesExpressionPredicate(Expression expression)
        {
            return _predicate((TExpression)expression);
        }

        private static bool IsTargetExpressionType(Expression expression)
        {
            return expression is TExpression;
        }
    }

    /// <summary>
    /// A class that is capable of doing a find and replace in an <see cref="Expression"/> tree.
    /// </summary>
    /// <typeparam name="TExpression">The type of <see cref="Expression"/> to find and replace.</typeparam>
    public class ExpressionReplacer<TExpression> : ExpressionVisitor where TExpression : Expression
    {

        private Func<TExpression, Expression> _replaceWith;
        private Func<TExpression, bool> _predicate;


        /// <summary>
        /// Searches for expressions using the given <paramref name="predicate"/> and 
        /// replaces matches with the result from the <paramref name="replaceWith"/> delegate.          
        /// </summary>
        /// <param name="expression">The <see cref="Expression"/> that 
        /// represents the sub tree for which to start searching.</param>
        /// <param name="predicate">The <see cref="Func{T,TResult}"/> used to filter the result</param>
        /// <param name="replaceWith">The <see cref="Func{T,TResult}"/> 
        /// used to specify the replacement expression.</param>
        /// <returns>The modified <see cref="Expression"/> tree.</returns>
        public Expression Replace(Expression expression, Func<TExpression, bool> predicate,
            Func<TExpression, Expression> replaceWith)
        {
            _replaceWith = replaceWith;
            _predicate = predicate;
            return Visit(expression);
        }

        /// <summary>
        /// Visits each node of the <see cref="Expression"/> tree checks 
        /// if the current expression matches the predicate. If a match is found 
        /// the expression will be replaced.        
        /// </summary>
        /// <param name="expression">The <see cref="Expression"/> currently being visited.</param>
        /// <returns><see cref="Expression"/></returns>        
        public override Expression Visit(Expression expression)
        {
            if (expression is TExpression)
                if (_predicate((TExpression) expression))
                    return _replaceWith((TExpression) expression);
            return base.Visit(expression);
        }
    }
#if NET35
    public abstract class ExpressionVisitor
    {
        protected ExpressionVisitor()
        {
        }

        public virtual Expression Visit(Expression exp)
        {
            if (exp == null)
                return exp;
            switch (exp.NodeType)
            {
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.ArrayLength:
                case ExpressionType.Quote:
                case ExpressionType.TypeAs:
                    return this.VisitUnary((UnaryExpression)exp);
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.Divide:
                case ExpressionType.Modulo:
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.Coalesce:
                case ExpressionType.ArrayIndex:
                case ExpressionType.RightShift:
                case ExpressionType.LeftShift:
                case ExpressionType.ExclusiveOr:
                    return this.VisitBinary((BinaryExpression)exp);
                case ExpressionType.TypeIs:
                    return this.VisitTypeIs((TypeBinaryExpression)exp);
                case ExpressionType.Conditional:
                    return this.VisitConditional((ConditionalExpression)exp);
                case ExpressionType.Constant:
                    return this.VisitConstant((ConstantExpression)exp);
                case ExpressionType.Parameter:
                    return this.VisitParameter((ParameterExpression)exp);
                case ExpressionType.MemberAccess:
                    return this.VisitMemberAccess((MemberExpression)exp);
                case ExpressionType.Call:
                    return this.VisitMethodCall((MethodCallExpression)exp);
                case ExpressionType.Lambda:
                    return this.VisitLambda((LambdaExpression)exp);
                case ExpressionType.New:
                    return this.VisitNew((NewExpression)exp);
                case ExpressionType.NewArrayInit:
                case ExpressionType.NewArrayBounds:
                    return this.VisitNewArray((NewArrayExpression)exp);
                case ExpressionType.Invoke:
                    return this.VisitInvocation((InvocationExpression)exp);
                case ExpressionType.MemberInit:
                    return this.VisitMemberInit((MemberInitExpression)exp);
                case ExpressionType.ListInit:
                    return this.VisitListInit((ListInitExpression)exp);
                default:
                    throw new Exception(string.Format("Unhandled expression type: '{0}'", exp.NodeType));
            }
        }

        protected virtual MemberBinding VisitBinding(MemberBinding binding)
        {
            switch (binding.BindingType)
            {
                case MemberBindingType.Assignment:
                    return this.VisitMemberAssignment((MemberAssignment)binding);
                case MemberBindingType.MemberBinding:
                    return this.VisitMemberMemberBinding((MemberMemberBinding)binding);
                case MemberBindingType.ListBinding:
                    return this.VisitMemberListBinding((MemberListBinding)binding);
                default:
                    throw new Exception(string.Format("Unhandled binding type '{0}'", binding.BindingType));
            }
        }

        protected virtual ElementInit VisitElementInitializer(ElementInit initializer)
        {
            ReadOnlyCollection<Expression> arguments = this.VisitExpressionList(initializer.Arguments);
            if (arguments != initializer.Arguments)
            {
                return Expression.ElementInit(initializer.AddMethod, arguments);
            }
            return initializer;
        }

        protected virtual Expression VisitUnary(UnaryExpression u)
        {
            Expression operand = this.Visit(u.Operand);
            if (operand != u.Operand)
            {
                return Expression.MakeUnary(u.NodeType, operand, u.Type, u.Method);
            }
            return u;
        }

        protected virtual Expression VisitBinary(BinaryExpression b)
        {
            Expression left = this.Visit(b.Left);
            Expression right = this.Visit(b.Right);
            Expression conversion = this.Visit(b.Conversion);
            if (left != b.Left || right != b.Right || conversion != b.Conversion)
            {
                if (b.NodeType == ExpressionType.Coalesce && b.Conversion != null)
                    return Expression.Coalesce(left, right, conversion as LambdaExpression);
                else
                    return Expression.MakeBinary(b.NodeType, left, right, b.IsLiftedToNull, b.Method);
            }
            return b;
        }

        protected virtual Expression VisitTypeIs(TypeBinaryExpression b)
        {
            Expression expr = this.Visit(b.Expression);
            if (expr != b.Expression)
            {
                return Expression.TypeIs(expr, b.TypeOperand);
            }
            return b;
        }

        protected virtual Expression VisitConstant(ConstantExpression c)
        {
            return c;
        }

        protected virtual Expression VisitConditional(ConditionalExpression c)
        {
            Expression test = this.Visit(c.Test);
            Expression ifTrue = this.Visit(c.IfTrue);
            Expression ifFalse = this.Visit(c.IfFalse);
            if (test != c.Test || ifTrue != c.IfTrue || ifFalse != c.IfFalse)
            {
                return Expression.Condition(test, ifTrue, ifFalse);
            }
            return c;
        }

        protected virtual Expression VisitParameter(ParameterExpression p)
        {
            return p;
        }

        protected virtual Expression VisitMemberAccess(MemberExpression m)
        {
            Expression exp = this.Visit(m.Expression);
            if (exp != m.Expression)
            {
                return Expression.MakeMemberAccess(exp, m.Member);
            }
            return m;
        }

        protected virtual Expression VisitMethodCall(MethodCallExpression m)
        {
            Expression obj = this.Visit(m.Object);
            IEnumerable<Expression> args = this.VisitExpressionList(m.Arguments);
            if (obj != m.Object || args != m.Arguments)
            {
                return Expression.Call(obj, m.Method, args);
            }
            return m;
        }

        protected virtual ReadOnlyCollection<Expression> VisitExpressionList(ReadOnlyCollection<Expression> original)
        {
            List<Expression> list = null;
            for (int i = 0, n = original.Count; i < n; i++)
            {
                Expression p = this.Visit(original[i]);
                if (list != null)
                {
                    list.Add(p);
                }
                else if (p != original[i])
                {
                    list = new List<Expression>(n);
                    for (int j = 0; j < i; j++)
                    {
                        list.Add(original[j]);
                    }
                    list.Add(p);
                }
            }
            if (list != null)
            {
                return list.AsReadOnly();
            }
            return original;
        }

        protected virtual MemberAssignment VisitMemberAssignment(MemberAssignment assignment)
        {
            Expression e = this.Visit(assignment.Expression);
            if (e != assignment.Expression)
            {
                return Expression.Bind(assignment.Member, e);
            }
            return assignment;
        }

        protected virtual MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding binding)
        {
            IEnumerable<MemberBinding> bindings = this.VisitBindingList(binding.Bindings);
            if (bindings != binding.Bindings)
            {
                return Expression.MemberBind(binding.Member, bindings);
            }
            return binding;
        }

        protected virtual MemberListBinding VisitMemberListBinding(MemberListBinding binding)
        {
            IEnumerable<ElementInit> initializers = this.VisitElementInitializerList(binding.Initializers);
            if (initializers != binding.Initializers)
            {
                return Expression.ListBind(binding.Member, initializers);
            }
            return binding;
        }

        protected virtual IEnumerable<MemberBinding> VisitBindingList(ReadOnlyCollection<MemberBinding> original)
        {
            List<MemberBinding> list = null;
            for (int i = 0, n = original.Count; i < n; i++)
            {
                MemberBinding b = this.VisitBinding(original[i]);
                if (list != null)
                {
                    list.Add(b);
                }
                else if (b != original[i])
                {
                    list = new List<MemberBinding>(n);
                    for (int j = 0; j < i; j++)
                    {
                        list.Add(original[j]);
                    }
                    list.Add(b);
                }
            }
            if (list != null)
                return list;
            return original;
        }

        protected virtual IEnumerable<ElementInit> VisitElementInitializerList(ReadOnlyCollection<ElementInit> original)
        {
            List<ElementInit> list = null;
            for (int i = 0, n = original.Count; i < n; i++)
            {
                ElementInit init = this.VisitElementInitializer(original[i]);
                if (list != null)
                {
                    list.Add(init);
                }
                else if (init != original[i])
                {
                    list = new List<ElementInit>(n);
                    for (int j = 0; j < i; j++)
                    {
                        list.Add(original[j]);
                    }
                    list.Add(init);
                }
            }
            if (list != null)
                return list;
            return original;
        }

        protected virtual Expression VisitLambda(LambdaExpression lambda)
        {
            Expression body = this.Visit(lambda.Body);
            if (body != lambda.Body)
            {
                return Expression.Lambda(lambda.Type, body, lambda.Parameters);
            }
            return lambda;
        }

        protected virtual NewExpression VisitNew(NewExpression nex)
        {
            IEnumerable<Expression> args = this.VisitExpressionList(nex.Arguments);
            if (args != nex.Arguments)
            {
                if (nex.Members != null)
                    return Expression.New(nex.Constructor, args, nex.Members);
                else
                    return Expression.New(nex.Constructor, args);
            }
            return nex;
        }

        protected virtual Expression VisitMemberInit(MemberInitExpression init)
        {
            NewExpression n = this.VisitNew(init.NewExpression);
            IEnumerable<MemberBinding> bindings = this.VisitBindingList(init.Bindings);
            if (n != init.NewExpression || bindings != init.Bindings)
            {
                return Expression.MemberInit(n, bindings);
            }
            return init;
        }

        protected virtual Expression VisitListInit(ListInitExpression init)
        {
            NewExpression n = this.VisitNew(init.NewExpression);
            IEnumerable<ElementInit> initializers = this.VisitElementInitializerList(init.Initializers);
            if (n != init.NewExpression || initializers != init.Initializers)
            {
                return Expression.ListInit(n, initializers);
            }
            return init;
        }

        protected virtual Expression VisitNewArray(NewArrayExpression na)
        {
            IEnumerable<Expression> exprs = this.VisitExpressionList(na.Expressions);
            if (exprs != na.Expressions)
            {
                if (na.NodeType == ExpressionType.NewArrayInit)
                {
                    return Expression.NewArrayInit(na.Type.GetElementType(), exprs);
                }
                else
                {
                    return Expression.NewArrayBounds(na.Type.GetElementType(), exprs);
                }
            }
            return na;
        }

        protected virtual Expression VisitInvocation(InvocationExpression iv)
        {
            IEnumerable<Expression> args = this.VisitExpressionList(iv.Arguments);
            Expression expr = this.Visit(iv.Expression);
            if (args != iv.Arguments || expr != iv.Expression)
            {
                return Expression.Invoke(expr, args);
            }
            return iv;
        }
    }
#endif
}
