namespace MassTransit.Transformation.PropertyProviders
{
    using System.Threading.Tasks;
    using Contexts;
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
            if (input == null || !context.TryGetPayload(out TransformContext<TMessage> transformContext) || !transformContext.HasInput)
                return TaskUtil.Default<TProperty>();

            var propertyTransformContext = new PropertyTransformContext<TMessage, TProperty>(transformContext, input);

            InitializeContext<TProperty> messageContext = _initializer.Create(propertyTransformContext);

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
