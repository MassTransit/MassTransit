namespace MassTransit.Transformation.TransformConfigurators
{
    using System;
    using System.Reflection;
    using Initializers.PropertyProviders;
    using MessageData;
    using MessageData.PropertyProviders;


    public class GetMessageDataTransformConfiguration<TInput, TValue> :
        IMessageDataTransformConfiguration<TInput>
        where TInput : class
    {
        readonly PropertyInfo _property;
        readonly IMessageDataRepository _repository;

        public GetMessageDataTransformConfiguration(IMessageDataRepository repository, PropertyInfo property)
        {
            if (repository == null)
                throw new ArgumentNullException(nameof(repository));

            _property = property;
            _repository = repository;
        }

        public void Apply(ITransformConfigurator<TInput> configurator)
        {
            var inputPropertyProvider = new InputPropertyProvider<TInput, MessageData<TValue>>(_property);

            var provider = new GetMessageDataPropertyProvider<TInput, TValue>(inputPropertyProvider, _repository);

            configurator.Set(_property, provider);
        }
    }
}
