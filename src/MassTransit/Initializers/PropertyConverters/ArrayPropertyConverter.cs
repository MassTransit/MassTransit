namespace MassTransit.Initializers.PropertyConverters
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Util;


    public class ArrayPropertyConverter<TElement> :
        IPropertyConverter<TElement[], IEnumerable<TElement>>
    {
        public async Task<TElement[]> Convert<TMessage>(InitializeContext<TMessage> context, IEnumerable<TElement> input)
            where TMessage : class
        {
            return input?.ToArray();
        }
    }


    public class ArrayPropertyConverter<TElement, TInputElement> :
        IPropertyConverter<TElement[], IEnumerable<TInputElement>>
    {
        readonly IPropertyConverter<TElement, TInputElement> _converter;

        public ArrayPropertyConverter(IPropertyConverter<TElement, TInputElement> converter)
        {
            _converter = converter;
        }

        public Task<TElement[]> Convert<TMessage>(InitializeContext<TMessage> context, IEnumerable<TInputElement> input)
            where TMessage : class
        {
            if (input == null)
                return TaskUtil.Default<TElement[]>();

            return Task.WhenAll(input.Select(x => _converter.Convert(context, x)));
        }
    }
}
