namespace MassTransit.Transformation.TransformConfigurators
{
    using System;
    using System.Reflection;
    using Initializers.PropertyProviders;
    using MessageData;
    using MessageData.PropertyProviders;
    using Metadata;


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
            if (!TypeMetadataCache<TInput>.IsValidMessageType)
                return;

            var inputPropertyProvider = new InputPropertyProvider<TInput, MessageData<TValue>>(_property);

            var provider = new PutMessageDataPropertyProvider<TInput, TValue>(inputPropertyProvider, _repository);

            configurator.Set(_property, provider);
        }
    }
}
