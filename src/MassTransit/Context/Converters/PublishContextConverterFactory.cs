namespace MassTransit.Context.Converters
{
    using System;
    using System.Linq;
    using GreenPipes.Filters;
    using Internals.Extensions;


    public class PublishContextConverterFactory :
        IPipeContextConverterFactory<PublishContext>
    {
        IPipeContextConverter<PublishContext, TOutput> IPipeContextConverterFactory<PublishContext>.GetConverter<TOutput>()
        {
            var innerType = typeof(TOutput).GetClosingArguments(typeof(PublishContext<>)).Single();

            return (IPipeContextConverter<PublishContext, TOutput>)Activator.CreateInstance(typeof(Converter<>).MakeGenericType(innerType));
        }


        class Converter<T> :
            IPipeContextConverter<PublishContext, PublishContext<T>>
            where T : class
        {
            bool IPipeContextConverter<PublishContext, PublishContext<T>>.TryConvert(PublishContext input, out PublishContext<T> output)
            {
                output = input as PublishContext<T>;

                return output != null;
            }
        }
    }
}
