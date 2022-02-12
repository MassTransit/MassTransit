namespace MassTransit.Internals
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;


    public class ReadOnlyProperty
    {
        public readonly Func<object, object> GetProperty;

        public ReadOnlyProperty(PropertyInfo property)
        {
            Property = property;
            GetProperty = GetGetMethod(Property);
        }

        public PropertyInfo Property { get; private set; }

        public object Get(object instance)
        {
            return GetProperty(instance);
        }

        static Func<object, object> GetGetMethod(PropertyInfo property)
        {
            if (property.DeclaringType == null)
                throw new ArgumentException("DeclaringType is null", nameof(property));

            var instance = Expression.Parameter(typeof(object), "instance");
            var instanceCast = property.DeclaringType.GetTypeInfo().IsValueType
                ? Expression.Convert(instance, property.DeclaringType)
                : Expression.TypeAs(instance, property.DeclaringType);

            var call = Expression.Call(instanceCast, property.GetMethod);
            var typeAs = Expression.TypeAs(call, typeof(object));

            return Expression.Lambda<Func<object, object>>(typeAs, instance).Compile();
        }
    }


    public class ReadOnlyProperty<T>
    {
        public readonly Func<T, object> GetProperty;

        public ReadOnlyProperty(Expression<Func<T, object>> propertyExpression)
            : this(propertyExpression.GetPropertyInfo())
        {
        }

        public ReadOnlyProperty(PropertyInfo? property)
        {
            if (property == null)
                throw new ArgumentNullException(nameof(property));

            Property = property;
            GetProperty = GetGetMethod(Property);
        }

        public PropertyInfo Property { get; private set; }

        public object Get(T instance)
        {
            return GetProperty(instance);
        }

        static Func<T, object> GetGetMethod(PropertyInfo property)
        {
            var instance = Expression.Parameter(typeof(T), "instance");
            var call = Expression.Call(instance, property.GetMethod);
            var typeAs = Expression.TypeAs(call, typeof(object));
            return Expression.Lambda<Func<T, object>>(typeAs, instance).Compile();
        }
    }


    public class ReadOnlyProperty<T, TProperty>
    {
        public readonly Func<T, TProperty> GetProperty;

        public ReadOnlyProperty(Expression<Func<T, object>> propertyExpression)
            : this(propertyExpression.GetPropertyInfo())
        {
        }

        public ReadOnlyProperty(PropertyInfo? property)
        {
            if (property == null)
                throw new ArgumentNullException(nameof(property));

            Property = property;
            GetProperty = GetGetMethod(Property);
        }

        public PropertyInfo Property { get; private set; }

        public TProperty Get(T instance)
        {
            return GetProperty(instance);
        }

        static Func<T, TProperty> GetGetMethod(PropertyInfo property)
        {
            var instance = Expression.Parameter(typeof(T), "instance");
            var call = Expression.Call(instance, property.GetMethod);

            return Expression.Lambda<Func<T, TProperty>>(call, instance).Compile();
        }
    }
}
