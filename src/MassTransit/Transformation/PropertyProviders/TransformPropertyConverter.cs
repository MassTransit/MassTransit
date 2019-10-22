namespace MassTransit.Transformation.PropertyProviders
{
    using System.Threading.Tasks;
    using Initializers;
    using Util;


    public class TransformPropertyConverter<TProperty> :
        IPropertyConverter<TProperty, TProperty>
        where TProperty : class
    {
        readonly IMessageInitializer<TProperty> _initializer;

        public TransformPropertyConverter(IMessageInitializer<TProperty> initializer)
        {
            _initializer = initializer;
        }

        Task<TProperty> IPropertyConverter<TProperty, TProperty>.Convert<TMessage>(InitializeContext<TMessage> context, TProperty input)
        {
            if (input == null)
                return TaskUtil.Default<TProperty>();

            InitializeContext<TProperty> messageContext = MessageFactoryCache<TProperty>.Factory.Create(context);

            Task<InitializeContext<TProperty>> initTask = _initializer.Initialize(messageContext, input);
            if (initTask.IsCompleted)
                return Task.FromResult(initTask.Result.Message);

            async Task<TProperty> ConvertAsync()
            {
                InitializeContext<TProperty> result = await initTask.ConfigureAwait(false);

                return result.Message;
            }

            return ConvertAsync();
        }
    }
}
