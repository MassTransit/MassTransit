namespace MassTransit.Initializers.PropertyConverters
{
    using System.Threading.Tasks;


    public class ToObjectPropertyConverter<TInput> :
        IPropertyConverter<object, TInput>
    {
        public Task<object> Convert<T>(InitializeContext<T> context, TInput input)
            where T : class
        {
            return Task.FromResult<object>(input);
        }
    }
}