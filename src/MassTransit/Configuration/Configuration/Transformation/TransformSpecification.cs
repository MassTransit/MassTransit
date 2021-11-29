namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading.Tasks;
    using Initializers;
    using Initializers.Conventions;
    using Initializers.Factories;
    using Initializers.PropertyInitializers;
    using Initializers.PropertyProviders;
    using Internals;
    using Transformation;


    public abstract class TransformSpecification<TMessage> :
        ITransformConfigurator<TMessage>
        where TMessage : class
    {
        readonly MessageTransformConvention<TMessage> _convention;

        protected TransformSpecification()
        {
            _convention = new MessageTransformConvention<TMessage>();
        }

        public int Count => _convention.Count;

        public bool Replace { get; set; }

        public void Default<TProperty>(Expression<Func<TMessage, TProperty>> propertyExpression)
        {
            Set(propertyExpression, (TProperty)default);
        }

        public void Set<TProperty>(Expression<Func<TMessage, TProperty>> propertyExpression, TProperty value)
        {
            var propertyInfo = propertyExpression.GetPropertyInfo();

            var valueProvider = new ConstantPropertyProvider<TMessage, TProperty>(value);

            var initializer = new ProviderPropertyInitializer<TMessage, TMessage, TProperty>(valueProvider, propertyInfo);

            _convention.Add(propertyInfo.Name, initializer);
        }

        public void Set<TProperty>(Expression<Func<TMessage, TProperty>> propertyExpression,
            Func<TransformPropertyContext<TProperty, TMessage>, TProperty> valueProvider)
        {
            var propertyInfo = propertyExpression.GetPropertyInfo();

            var inputValueProvider = new InputPropertyProvider<TMessage, TProperty>(propertyInfo);

            Task<TProperty> PropertyProvider(TransformPropertyContext<TProperty, TMessage> context)
            {
                return Task.FromResult(valueProvider(context));
            }

            var propertyProvider = new DelegatePropertyProvider<TMessage, TProperty>(inputValueProvider, PropertyProvider);

            var initializer = new ProviderPropertyInitializer<TMessage, TMessage, TProperty>(propertyProvider, propertyInfo);

            _convention.Add(propertyInfo.Name, initializer);
        }

        public void Set<TProperty>(PropertyInfo propertyInfo, IPropertyProvider<TMessage, TProperty> propertyProvider)
        {
            var initializer = new ProviderPropertyInitializer<TMessage, TMessage, TProperty>(propertyProvider, propertyInfo);

            _convention.Add(propertyInfo.Name, initializer);
        }

        public void Transform<TProperty>(PropertyInfo propertyInfo, IPropertyProvider<TMessage, TProperty> propertyProvider)
        {
            var initializer = new TransformPropertyInitializer<TMessage, TMessage, TProperty>(propertyProvider, propertyInfo);

            _convention.Add(propertyInfo.Name, initializer);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            yield break;
        }

        protected IMessageInitializer<TMessage> Build()
        {
            IMessageFactory<TMessage> messageFactory = null;
            IEnumerable<IInitializerConvention> conventions = Enumerable.Repeat<IInitializerConvention>(_convention, 1);
            if (Replace)
                messageFactory = new ReplaceMessageFactory<TMessage>();
            else
                conventions = conventions.Concat(MessageInitializer.Conventions);


            var initializerFactory = new MessageInitializerFactory<TMessage, TMessage>(messageFactory, conventions.ToArray());

            IMessageInitializer<TMessage> messageInitializer = initializerFactory.CreateMessageInitializer();

            return messageInitializer;
        }
    }
}
