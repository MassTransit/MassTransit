namespace MassTransit.Context.Converters
{
    using System;
    using System.Linq;
    using GreenPipes.Filters;
    using Internals.Extensions;


    public class SendContextConverterFactory :
        IPipeContextConverterFactory<SendContext>
    {
        IPipeContextConverter<SendContext, TOutput> IPipeContextConverterFactory<SendContext>.GetConverter<TOutput>()
        {
            var innerType = typeof(TOutput).GetClosingArguments(typeof(SendContext<>)).Single();

            return (IPipeContextConverter<SendContext, TOutput>)Activator.CreateInstance(typeof(Converter<>).MakeGenericType(innerType));
        }


        class Converter<T> :
            IPipeContextConverter<SendContext, SendContext<T>>
            where T : class
        {
            bool IPipeContextConverter<SendContext, SendContext<T>>.TryConvert(SendContext input, out SendContext<T> output)
            {
                output = input as SendContext<T>;

                return output != null;
            }
        }
    }
}
