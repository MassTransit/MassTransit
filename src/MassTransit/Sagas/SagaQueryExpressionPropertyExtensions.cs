#nullable enable
namespace MassTransit;

using System;
using System.Linq.Expressions;
using Saga;


public static class SagaQueryExpressionPropertyExtensions
{
    /// <summary>
    /// Analyzes the saga query and determines if it contains a binary expression that queries by a property value
    /// </summary>
    /// <param name="query"></param>
    /// <param name="value"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static bool TryGetPropertyValue<T>(this ISagaQuery<T> query, out object? value)
        where T : class, ISaga
    {
        Expression<Func<T, bool>> expression = query.FilterExpression;
        if (expression == null)
            throw new ArgumentException("The query is not a lambda expression", nameof(query));

        if (expression.Body is BinaryExpression
            {
                Right: MemberExpression
                {
                    Member.Name: "Value", Expression: ConstantExpression { Value: IPropertyExpressionPropertyValue propertyValue }
                }
            })
        {
            value = propertyValue.GetValue();
            return true;
        }

        value = null;
        return false;
    }

    /// <summary>
    /// Analyzes the saga query and determines if it contains a binary expression that queries by a property value
    /// </summary>
    /// <param name="query"></param>
    /// <param name="value"></param>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TProperty"></typeparam>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static bool TryGetPropertyValue<T, TProperty>(this ISagaQuery<T> query, out TProperty? value)
        where T : class, ISaga
    {
        Expression<Func<T, bool>> expression = query.FilterExpression;
        if (expression == null)
            throw new ArgumentException("The query is not a lambda expression", nameof(query));

        if (expression.Body is BinaryExpression
            {
                Right: MemberExpression
                {
                    Member.Name: "Value", Expression: ConstantExpression { Value: PropertyExpressionPropertyValue<TProperty> propertyValue }
                }
            })
        {
            value = propertyValue.Value;
            return true;
        }

        value = default;
        return false;
    }
}
