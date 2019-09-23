namespace MassTransit.Transformation.TransformConfigurators
{
    using System.Reflection;
    using Initializers.PropertyProviders;
    using MessageData;


    public class LoadMessageDataTransformConfiguration<TInput, TValue> :
        IMessageDataTransformConfiguration<TInput>
        where TInput : class
    {
        readonly PropertyInfo _property;
        readonly IMessageDataRepository _repository;

        public LoadMessageDataTransformConfiguration(IMessageDataRepository repository, PropertyInfo property)
        {
            _property = property;
            _repository = repository;
        }

        public void Apply(ITransformConfigurator<TInput> configurator)
        {
            var inputPropertyProvider = new InputPropertyProvider<TInput, MessageData<TValue>>(_property);

            var provider = new LoadMessageDataPropertyProvider<TInput, TValue>(inputPropertyProvider, _repository);

            configurator.Set(_property, provider);
        }
    }
}
