namespace MassTransit.Transformation.TransformConfigurators
{
    using System.Reflection;
    using MessageData;


    public class LoadMessageDataTransformConfiguration<TInput, TValue> :
        IMessageDataTransformConfiguration<TInput>
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
            var provider = new LoadMessageDataPropertyProvider<TInput, TValue>(_repository, _property);

            configurator.Replace(_property, provider);
        }
    }
}