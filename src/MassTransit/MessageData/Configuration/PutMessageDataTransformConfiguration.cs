namespace MassTransit.MessageData.Configuration
{
    using System;
    using System.Reflection;
    using Initializers.PropertyProviders;
    using PropertyProviders;


    public class PutMessageDataTransformConfiguration<TInput, TValue> :
        IMessageDataTransformConfiguration<TInput>
        where TInput : class
    {
        readonly PropertyInfo _property;
        readonly IMessageDataRepository _repository;

        public PutMessageDataTransformConfiguration(IMessageDataRepository repository, PropertyInfo property)
        {
            if (repository == null)
                throw new ArgumentNullException(nameof(repository));

            _property = property;
            _repository = repository;
        }

        public void Apply(ITransformConfigurator<TInput> configurator)
        {
            if (!MessageTypeCache<TInput>.IsValidMessageType)
                return;

            var inputPropertyProvider = new InputPropertyProvider<TInput, MessageData<TValue>>(_property);

            var provider = new PutMessageDataPropertyProvider<TInput, TValue>(inputPropertyProvider, _repository);

            configurator.Set(_property, provider);
        }
    }
}
