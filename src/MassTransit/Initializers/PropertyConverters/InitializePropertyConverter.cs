namespace MassTransit.Initializers.PropertyConverters
{
    using System.Threading.Tasks;
    using Util;


    public class InitializePropertyConverter<TProperty, TInput> :
        IPropertyConverter<TProperty, TInput>
        where TProperty : class
        where TInput : class
    {
        readonly IMessageInitializer<TProperty> _initializer;

        public InitializePropertyConverter()
        {
            _initializer = MessageInitializerCache<TProperty>.GetInitializer(typeof(TInput));
        }

        Task<TProperty> IPropertyConverter<TProperty, TInput>.Convert<TMessage>(InitializeContext<TMessage> context, TInput input)
        {
            if (input == null)
                return TaskUtil.Default<TProperty>();

            InitializeContext<TProperty> messageContext = MessageFactoryCache<TProperty>.Factory.Create(context);

            Task<InitializeContext<TProperty>> initTask = _initializer.Initialize(messageContext, input);
            if (initTask.Status == TaskStatus.RanToCompletion)
                return Task.FromResult(initTask.Result.Message);

            async Task<TProperty> ConvertAsync()
            {
                InitializeContext<TProperty> result = await initTask.ConfigureAwait(false);

                return result.Message;
            }

            return ConvertAsync();
        }
    }


    public class InitializePropertyConverter<TProperty> :
        IPropertyConverter<TProperty, object>
        where TProperty : class
    {
        Task<TProperty> IPropertyConverter<TProperty, object>.Convert<TMessage>(InitializeContext<TMessage> context, object input)
        {
            if (input == null)
                return TaskUtil.Default<TProperty>();

            InitializeContext<TProperty> messageContext = MessageFactoryCache<TProperty>.Factory.Create(context);

            IMessageInitializer<TProperty> initializer = MessageInitializerCache<TProperty>.GetInitializer(input.GetType());

            Task<InitializeContext<TProperty>> initTask = initializer.Initialize(messageContext, input);
            if (initTask.Status == TaskStatus.RanToCompletion)
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
