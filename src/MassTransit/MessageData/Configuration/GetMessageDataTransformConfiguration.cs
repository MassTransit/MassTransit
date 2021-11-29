namespace MassTransit.MessageData.Configuration
{
    using System;
    using System.Reflection;
    using Initializers.PropertyProviders;
    using PropertyProviders;


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
