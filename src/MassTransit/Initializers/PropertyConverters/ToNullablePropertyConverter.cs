namespace MassTransit.Initializers.PropertyConverters
{
    using System.Threading.Tasks;


    public class ToNullablePropertyConverter<TResult> :
        IPropertyConverter<TResult?, TResult>
        where TResult : struct
    {
        public Task<TResult?> Convert<T>(InitializeContext<T> context, TResult input)
            where T : class
        {
            return Task.FromResult<TResult?>(input);
        }
    }


    public class ToNullablePropertyConverter<TResult, TInput> :
        IPropertyConverter<TResult?, TInput>
        where TResult : struct
    {
        readonly IPropertyConverter<TResult, TInput> _converter;

        public ToNullablePropertyConverter(IPropertyConverter<TResult, TInput> converter)
        {
            _converter = converter;
        }

        public Task<TResult?> Convert<T>(InitializeContext<T> context, TInput input)
            where T : class
        {
            Task<TResult> resultTask = _converter.Convert(context, input);
            if (resultTask.Status == TaskStatus.RanToCompletion)
                return Task.FromResult<TResult?>(resultTask.Result);

            async Task<TResult?> ConvertAsync()
            {
                var result = await resultTask.ConfigureAwait(false);

                return result;
            }

            return ConvertAsync();
        }
    }
}
