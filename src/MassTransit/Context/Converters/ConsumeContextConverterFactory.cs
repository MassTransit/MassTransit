namespace MassTransit.Context.Converters
{
    using System;
    using System.Linq;
    using GreenPipes.Filters;
    using Internals.Extensions;


    public class ConsumeContextConverterFactory :
        IPipeContextConverterFactory<ConsumeContext>
    {
        IPipeContextConverter<ConsumeContext, TOutput> IPipeContextConverterFactory<ConsumeContext>.GetConverter<TOutput>()
        {
            var innerType = typeof(TOutput).GetClosingArguments(typeof(ConsumeContext<>)).Single();

            return (IPipeContextConverter<ConsumeContext, TOutput>)Activator.CreateInstance(typeof(Converter<>).MakeGenericType(innerType));
        }


        class Converter<T> :
            IPipeContextConverter<ConsumeContext, ConsumeContext<T>>
            where T : class
        {
            bool IPipeContextConverter<ConsumeContext, ConsumeContext<T>>.TryConvert(ConsumeContext input, out ConsumeContext<T> output)
            {
                return input.TryGetMessage(out output);
            }
        }
    }
}
