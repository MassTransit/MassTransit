namespace MassTransit.Transformation
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Initializers;
    using Initializers.Conventions;


    public class MessageTransformConvention<TMessage> :
        IInitializerConvention<TMessage, TMessage>,
        IInitializerConvention<TMessage>,
        IInitializerConvention
        where TMessage : class
    {
        readonly IDictionary<string, IPropertyInitializer<TMessage, TMessage>> _initializers;

        public MessageTransformConvention()
        {
            _initializers = new Dictionary<string, IPropertyInitializer<TMessage, TMessage>>(StringComparer.OrdinalIgnoreCase);
        }

        public int Count => _initializers.Count;

        public bool TryGetPropertyInitializer<T, TInput, TProperty>(PropertyInfo propertyInfo, out IPropertyInitializer<T, TInput> initializer)
            where T : class
            where TInput : class
        {
            if (this is IInitializerConvention<T, TInput> convention)
                return convention.TryGetPropertyInitializer<TProperty>(propertyInfo, out initializer);

            initializer = default;
            return false;
        }

        public bool TryGetHeaderInitializer<T, TInput, TProperty>(PropertyInfo propertyInfo, out IHeaderInitializer<T, TInput> initializer)
            where T : class
            where TInput : class
        {
            if (this is IInitializerConvention<T, TInput> convention)
                return convention.TryGetHeaderInitializer<TProperty>(propertyInfo, out initializer);

            initializer = default;
            return false;
        }

        public bool TryGetHeadersInitializer<T, TInput, TProperty>(PropertyInfo propertyInfo, out IHeaderInitializer<T, TInput> initializer)
            where T : class
            where TInput : class
        {
            if (this is IInitializerConvention<T, TInput> convention)
                return convention.TryGetHeaderInitializer<TProperty>(propertyInfo, out initializer);

            initializer = default;
            return false;
        }

        public bool TryGetPropertyInitializer<TProperty>(PropertyInfo propertyInfo, out IPropertyInitializer<TMessage, TMessage> initializer)
        {
            if (_initializers.TryGetValue(propertyInfo.Name, out initializer))
                return true;

            initializer = default;
            return false;
        }

        public bool TryGetHeaderInitializer<TProperty>(PropertyInfo propertyInfo, out IHeaderInitializer<TMessage, TMessage> initializer)
        {
            initializer = default;
            return false;
        }

        public bool TryGetHeadersInitializer<TProperty>(PropertyInfo propertyInfo, out IHeaderInitializer<TMessage, TMessage> initializer)
        {
            initializer = default;
            return false;
        }

        public bool TryGetPropertyInitializer<TInput, TProperty>(PropertyInfo propertyInfo,
            out IPropertyInitializer<TMessage, TInput> initializer)
            where TInput : class
        {
            if (this is IInitializerConvention<TMessage, TInput> convention)
                return convention.TryGetPropertyInitializer<TProperty>(propertyInfo, out initializer);

            initializer = default;
            return false;
        }

        public bool TryGetHeaderInitializer<TInput, TProperty>(PropertyInfo propertyInfo,
            out IHeaderInitializer<TMessage, TInput> initializer)
            where TInput : class
        {
            if (this is IInitializerConvention<TMessage, TInput> convention)
                return convention.TryGetHeaderInitializer<TProperty>(propertyInfo, out initializer);

            initializer = default;
            return false;
        }

        public bool TryGetHeadersInitializer<TInput, TProperty>(PropertyInfo propertyInfo,
            out IHeaderInitializer<TMessage, TInput> initializer)
            where TInput : class
        {
            if (this is IInitializerConvention<TMessage, TInput> convention)
                return convention.TryGetHeaderInitializer<TProperty>(propertyInfo, out initializer);

            initializer = default;
            return false;
        }

        public void Add(string propertyName, IPropertyInitializer<TMessage, TMessage> initializer)
        {
            _initializers.Add(propertyName, initializer);
        }
    }
}
