namespace MassTransit.Initializers.PropertyConverters
{
    using System.Threading.Tasks;
    using Factories;


    public class InitializePropertyConverter<TProperty, TInput> :
        IPropertyConverter<TProperty, TInput>
        where TProperty : class
        where TInput : class
    {
        async Task<TProperty> IPropertyConverter<TProperty, TInput>.Convert<TMessage>(InitializeContext<TMessage> context, TInput input)
        {
            if (input == null)
                return null;

            InitializeContext<TProperty> messageContext = MessageFactoryCache<TProperty>.Factory.Create(context);

            IMessageInitializer<TProperty> initializer = typeof(TInput) == typeof(object)
                ? MessageInitializerCache<TProperty>.GetInitializer(input.GetType())
                : MessageInitializerCache<TProperty>.GetInitializer(typeof(TInput));

            InitializeContext<TProperty> result = await initializer.Initialize(messageContext, input).ConfigureAwait(false);

            return result.Message;
        }
    }
}
