namespace MassTransit.Middleware
{
    using System;
    using System.Linq;
    using Internals;


    public class ConsumeContextConverterFactory :
        IPipeContextConverterFactory<ConsumeContext>
    {
        public IPipeContextConverter<ConsumeContext, TOutput> GetConverter<TOutput>()
            where TOutput : class, PipeContext
        {
            var innerType = typeof(TOutput).GetClosingArguments(typeof(ConsumeContext<>)).Single();

            return (IPipeContextConverter<ConsumeContext, TOutput>)Activator.CreateInstance(typeof(Converter<>).MakeGenericType(innerType));
        }


        class Converter<T> :
            IPipeContextConverter<ConsumeContext, ConsumeContext<T>>
            where T : class
        {
            public bool TryConvert(ConsumeContext input, out ConsumeContext<T> output)
            {
                return input.TryGetMessage(out output);
            }
        }
    }
}
