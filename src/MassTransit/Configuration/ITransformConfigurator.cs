namespace MassTransit
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using Initializers;
    using Transformation;


    public interface ITransformConfigurator<TInput>
        where TInput : class
    {
        /// <summary>
        /// Specifies if the message should be replaced, meaning modified in-place, instead of creating a new message
        /// </summary>
        bool Replace { set; }

        /// <summary>
        /// Set the specified message property to the default value (ignoring the input value)
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="propertyExpression"></param>
        void Default<TProperty>(Expression<Func<TInput, TProperty>> propertyExpression);

        /// <summary>
        /// Set the specified property to a constant value
        /// </summary>
        /// <param name="propertyExpression"></param>
        /// <param name="value"></param>
        /// <typeparam name="TProperty"></typeparam>
        void Set<TProperty>(Expression<Func<TInput, TProperty>> propertyExpression, TProperty value);

        /// <summary>
        /// Set the property to the value, using the source context to create/select the value
        /// </summary>
        /// <typeparam name="TProperty">The property type</typeparam>
        /// <param name="propertyExpression">The property select expression</param>
        /// <param name="valueProvider">The method to return the property</param>
        void Set<TProperty>(Expression<Func<TInput, TProperty>> propertyExpression, Func<TransformPropertyContext<TProperty, TInput>, TProperty> valueProvider);

        /// <summary>
        /// Set the property to the value, using the property provider specified
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="property"></param>
        /// <param name="propertyProvider"></param>
        void Set<TProperty>(PropertyInfo property, IPropertyProvider<TInput, TProperty> propertyProvider);

        /// <summary>
        /// Transform the property, but leave it unchanged on the input
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="property"></param>
        /// <param name="propertyProvider"></param>
        void Transform<TProperty>(PropertyInfo property, IPropertyProvider<TInput, TProperty> propertyProvider);
    }
}
