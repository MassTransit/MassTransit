namespace MassTransit.Internals
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;


    public class ReadWriteProperty :
        ReadOnlyProperty
    {
        public readonly Action<object, object> SetProperty;

        public ReadWriteProperty(PropertyInfo property)
            : base(property)
        {
            SetProperty = GetSetMethod(Property);
        }

        public void Set(object instance, object value)
        {
            SetProperty(instance, value);
        }

        static Action<object, object> GetSetMethod(PropertyInfo property)
        {
            if (property.DeclaringType == null)
                throw new ArgumentException("DeclaringType is null", nameof(property));

            var instance = Expression.Parameter(typeof(object), "instance");
            var value = Expression.Parameter(typeof(object), "value");

            // value as T is slightly faster than (T)value, so if it's not a value type, use that
            var instanceCast = property.DeclaringType.GetTypeInfo().IsValueType
                ? Expression.Convert(instance, property.DeclaringType)
                : Expression.TypeAs(instance, property.DeclaringType);

            var valueCast = property.PropertyType.GetTypeInfo().IsValueType
                ? Expression.Convert(value, property.PropertyType)
                : Expression.TypeAs(value, property.PropertyType);

            var call = Expression.Call(instanceCast, property.SetMethod, valueCast);

            return Expression.Lambda<Action<object, object>>(call, instance, value).Compile();
        }
    }


    public class ReadWriteProperty<T> :
        ReadOnlyProperty<T>
    {
        public readonly Action<T, object> SetProperty;

        public ReadWriteProperty(Expression<Func<T, object>> propertyExpression)
            : this(propertyExpression.GetPropertyInfo())
        {
        }

        public ReadWriteProperty(Expression<Func<T, object>> propertyExpression, bool includeNonPublic)
            : this(propertyExpression.GetPropertyInfo())
        {
        }

        public ReadWriteProperty(PropertyInfo? property)
            : base(property)
        {
            if (property == null)
                throw new ArgumentNullException(nameof(property));

            SetProperty = GetSetMethod(Property);
        }

        public void Set(T instance, object value)
        {
            SetProperty(instance, value);
        }

        static Action<T, object> GetSetMethod(PropertyInfo property)
        {
            if (!property.CanWrite)
                return (_, __) => throw new InvalidOperationException("No setter available on " + property.Name);

            var instance = Expression.Parameter(typeof(T), "instance");
            var value = Expression.Parameter(typeof(object), "value");
            var valueCast = property.PropertyType.GetTypeInfo().IsValueType
                ? Expression.Convert(value, property.PropertyType)
                : Expression.TypeAs(value, property.PropertyType);
            var call = Expression.Call(instance, property.SetMethod, valueCast);

            return Expression.Lambda<Action<T, object>>(call, instance, value).Compile();
        }
    }


    public class ReadWriteProperty<T, TProperty> :
        ReadOnlyProperty<T, TProperty>
    {
        public readonly Action<T, TProperty> SetProperty;

        public ReadWriteProperty(Expression<Func<T, object>> propertyExpression)
            : this(propertyExpression.GetPropertyInfo())
        {
        }

        public ReadWriteProperty(Expression<Func<T, object>> propertyExpression, bool includeNonPublic)
            : this(propertyExpression.GetPropertyInfo())
        {
        }

        public ReadWriteProperty(PropertyInfo? property)
            : base(property)
        {
            if (property == null)
                throw new ArgumentNullException(nameof(property));

            SetProperty = GetSetMethod(Property);
        }

        public void Set(T instance, TProperty value)
        {
            SetProperty(instance, value);
        }

        static Action<T, TProperty> GetSetMethod(PropertyInfo property)
        {
            if (!property.CanWrite)
                return (_, __) => throw new InvalidOperationException("No setter available on " + property.Name);

            var instance = Expression.Parameter(typeof(T), "instance");
            var value = Expression.Parameter(typeof(TProperty), "value");
            var call = Expression.Call(instance, property.SetMethod, value);
            return Expression.Lambda<Action<T, TProperty>>(call, instance, value).Compile();
        }
    }
}
