using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LightInject
{
    /// <summary>
    /// Represents a set of extension methods that adds search and replace capabilities to expression trees.   
    /// </summary>
    internal static class ExpressionExtensions
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
}
