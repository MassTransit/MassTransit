namespace MassTransit.Persistence.Integration.Saga
{
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq.Expressions;
    using System.Reflection;
    using Internals;


    public static class SqlExpressionVisitor
    {
        public static List<SqlPredicate> CreateFromExpression(Expression node, List<SqlPropertyMapping>? mappings = null)
        {
            switch (node.NodeType)
            {
                case ExpressionType.Lambda:
                    return LambdaVisit((LambdaExpression)node, mappings);
                case ExpressionType.AndAlso:
                    return AndAlsoVisit((BinaryExpression)node, mappings);
                case ExpressionType.Not:
                    return NegatedVisit((UnaryExpression)node, mappings);
                case ExpressionType.NotEqual:
                    return ComparisonVisit((BinaryExpression)node, "<>", mappings);
                case ExpressionType.Equal:
                    return ComparisonVisit((BinaryExpression)node, "=", mappings);
                case ExpressionType.LessThan:
                    return ComparisonVisit((BinaryExpression)node, "<", mappings);
                case ExpressionType.LessThanOrEqual:
                    return ComparisonVisit((BinaryExpression)node, "<=", mappings);
                case ExpressionType.GreaterThan:
                    return ComparisonVisit((BinaryExpression)node, ">", mappings);
                case ExpressionType.GreaterThanOrEqual:
                    return ComparisonVisit((BinaryExpression)node, ">=", mappings);
                case ExpressionType.MemberAccess:
                    return MemberAccessVisit((MemberExpression)node, mappings);
                default:
                    throw new Exception("Node type not supported.");
            }
        }

        static List<SqlPredicate> LambdaVisit(LambdaExpression node, List<SqlPropertyMapping>? mappings)
        {
            return CreateFromExpression(node.Body, mappings);
        }

        static List<SqlPredicate> AndAlsoVisit(BinaryExpression node, List<SqlPropertyMapping>? mappings)
        {
            var result = new List<SqlPredicate>();

            result.AddRange(CreateFromExpression(node.Left, mappings));
            result.AddRange(CreateFromExpression(node.Right, mappings));

            return result;
        }

        static List<SqlPredicate> ComparisonVisit(BinaryExpression node, string op, List<SqlPropertyMapping>? mappings)
        {
            var left = (MemberExpression)node.Left;
            var name = MemberName(left.Member, mappings);

            object? value;

            if (node.Right is ConstantExpression right)
                value = right.Value;
            else
                value = Expression.Lambda<Func<object>>(Expression.Convert(node.Right, typeof(object))).CompileFast().Invoke();

            return [new SqlPredicate(name, value, op)];
        }

        static List<SqlPredicate> NegatedVisit(UnaryExpression node, List<SqlPropertyMapping>? mappings)
        {
            var property = (MemberExpression)node.Operand;
            var name = MemberName(property.Member, mappings);

            if (node.Type != typeof(bool))
                throw new InvalidOperationException("Negation is only supported for boolean properties");

            return [new SqlPredicate(name, false)];
        }

        static List<SqlPredicate> MemberAccessVisit(MemberExpression node, List<SqlPropertyMapping>? mappings)
        {
            var name = MemberName(node.Member, mappings);
            object? value;

            if (node.Type == typeof(bool))
                value = true;
            else if (node.Type.IsValueType)
                value = Activator.CreateInstance(node.Type);
            else
                value = null;

            return [new SqlPredicate(name, value)];
        }

        static string MemberName(MemberInfo member, List<SqlPropertyMapping>? mappings)
        {
            var specificName = member.GetCustomAttribute<ColumnAttribute>()?.Name;
            if (specificName is not null)
                return specificName;

            var prefixMatch = mappings?.Where(m => !m.Exact).FirstOrDefault(m =>
                m.Property.Type == member.DeclaringType
            );

            if (prefixMatch is not null)
                return string.Concat(prefixMatch.Name, member.Name);

            if (member is PropertyInfo prop)
            {
                var exactMatch = mappings?.Where(m => m.Exact).FirstOrDefault(m =>
                    m.Property.Type == prop.PropertyType &&
                    m.Property.Member.DeclaringType == prop.DeclaringType &&
                    m.Property.Member.Name == prop.Name
                );

                if (exactMatch is not null)
                    return exactMatch.Name!;
            }

            return member.Name;
        }
    }


    public class SqlPredicate
    {
        public SqlPredicate(string name, object? value, string @operator = "=")
        {
            Name = name;
            Operator = @operator;
            Value = value;
        }

        public string Name { get; set; }
        public string Operator { get; set; }
        public object? Value { get; set; }
    }


    public class SqlPropertyMapping
    {
        public MemberExpression Property { get; set; } = null!;
        public string? Name { get; set; }
        public bool Exact { get; set; }
    }
}
