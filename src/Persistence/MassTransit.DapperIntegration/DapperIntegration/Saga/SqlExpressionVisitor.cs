namespace MassTransit.DapperIntegration.Saga
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using Internals;


    public static class SqlExpressionVisitor
    {
        public static List<(string, object)> CreateFromExpression(Expression node)
        {
            switch (node.NodeType)
            {
                case ExpressionType.Lambda:
                    return LambdaVisit((LambdaExpression)node);
                case ExpressionType.AndAlso:
                    return AndAlsoVisit((BinaryExpression)node);
                case ExpressionType.Equal:
                    return EqualVisit((BinaryExpression)node);
                case ExpressionType.MemberAccess:
                    return MemberAccessVisit((MemberExpression)node);
                default:
                    throw new Exception("Node type not supported.");
            }
        }

        static List<(string, object)> LambdaVisit(LambdaExpression node)
        {
            return CreateFromExpression(node.Body);
        }

        static List<(string, object)> AndAlsoVisit(BinaryExpression node)
        {
            var result = new List<(string, object)>();
            List<(string, object)> leftResult = CreateFromExpression(node.Left);
            result.AddRange(leftResult);

            List<(string, object)> rightResult = CreateFromExpression(node.Right);
            result.AddRange(rightResult);

            return result;
        }

        static List<(string, object)> EqualVisit(BinaryExpression node)
        {
            var left = (MemberExpression)node.Left;

            var name = left.Member.Name;

            object value;
            if (node.Right is ConstantExpression right)
                value = right.Value;
            else
            {
                value = Expression.Lambda<Func<object>>(Expression.Convert(node.Right, typeof(object))).CompileFast().Invoke();
            }

            return new List<(string, object)> { (name, value) };
        }

        static List<(string, object)> MemberAccessVisit(MemberExpression node)
        {
            var name = node.Member.Name;
            object value;

            if (node.Type == typeof(bool))
                value = true; // No support for Not yet.
            else if (node.Type.IsValueType)
                value = Activator.CreateInstance(node.Type);
            else
                value = null;

            return new List<(string, object)> { (name, value) };
        }
    }
}
