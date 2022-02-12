namespace MassTransit.Internals
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;


    public static class ExpressionExtensions
    {
        /// <summary>
        /// Gets the name of the member specified
        /// </summary>
        /// <typeparam name="T">The type referenced</typeparam>
        /// <typeparam name="TMember">The type of the member referenced</typeparam>
        /// <param name="expression">The expression referencing the member</param>
        /// <returns>The name of the member referenced by the expression</returns>
        public static string GetMemberName<T, TMember>(this Expression<Func<T, TMember>> expression)
        {
            return expression.GetMemberExpression().Member.Name;
        }

        /// <summary>
        /// Gets the name of the member specified
        /// </summary>
        /// <typeparam name="T">The type referenced</typeparam>
        /// <param name="expression">The expression referencing the member</param>
        /// <returns>The name of the member referenced by the expression</returns>
        public static string GetMemberName<T>(this Expression<Action<T>> expression)
        {
            return expression.GetMemberExpression().Member.Name;
        }

        public static string GetMemberName<T>(this Expression<Func<T>> expression)
        {
            return expression.GetMemberExpression().Member.Name;
        }

        public static PropertyInfo? GetPropertyInfo<T, TMember>(this Expression<Func<T, TMember>> expression)
        {
            return expression.GetMemberExpression().Member as PropertyInfo;
        }

        public static PropertyInfo? GetPropertyInfo<T>(this Expression<Func<T>> expression)
        {
            return expression.GetMemberExpression().Member as PropertyInfo;
        }

        public static MemberInfo GetMemberInfo<T>(this Expression<Action<T>> expression)
        {
            return expression.GetMemberExpression().Member;
        }

        public static MemberExpression GetMemberExpression<T, TMember>(this Expression<Func<T, TMember>> expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            return GetMemberExpression(expression.Body);
        }

        public static MemberExpression GetMemberExpression<T>(this Expression<Action<T>> expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            return GetMemberExpression(expression.Body);
        }

        public static MemberExpression GetMemberExpression<T>(this Expression<Func<T>> expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            return GetMemberExpression(expression.Body);
        }

        public static MemberExpression GetMemberExpression<T1, T2>(this Expression<Action<T1, T2>> expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            return GetMemberExpression(expression.Body);
        }

        static MemberExpression GetMemberExpression(Expression body)
        {
            if (body == null)
                throw new ArgumentNullException(nameof(body));

            MemberExpression? memberExpression = null;
            if (body.NodeType == ExpressionType.Convert)
            {
                var unaryExpression = (UnaryExpression)body;
                memberExpression = unaryExpression.Operand as MemberExpression;
            }
            else if (body.NodeType == ExpressionType.MemberAccess)
                memberExpression = body as MemberExpression;

            if (memberExpression == null)
                throw new ArgumentException("Expression is not a member access");

            return memberExpression;
        }
    }
}
