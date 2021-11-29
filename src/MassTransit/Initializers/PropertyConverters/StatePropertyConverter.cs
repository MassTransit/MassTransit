namespace MassTransit.Initializers.PropertyConverters
{
    using System.Threading.Tasks;


    public class StatePropertyConverter<TInstance> :
        IPropertyConverter<string, State<TInstance>>
        where TInstance : class, SagaStateMachineInstance
    {
        public Task<string> Convert<T>(InitializeContext<T> context, State<TInstance> input)
            where T : class
        {
            return Task.FromResult(input?.Name);
        }
    }


    public class StatePropertyConverter<TResult, TInstance> :
        IPropertyConverter<TResult, State<TInstance>>
        where TInstance : class, SagaStateMachineInstance
    {
        readonly IPropertyConverter<TResult, string> _propertyConverter;

        public StatePropertyConverter(IPropertyConverter<TResult, string> propertyConverter)
        {
            _propertyConverter = propertyConverter;
        }

        public Task<TResult> Convert<T>(InitializeContext<T> context, State<TInstance> input)
            where T : class
        {
            if (input == default)
                return default;

            var name = input?.Name;

            return _propertyConverter.Convert(context, name);
        }
    }
}
