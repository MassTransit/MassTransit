namespace MassTransit.Initializers.PropertyConverters
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;


    public class DictionaryPropertyConverter<TKey, TElement> :
        IPropertyConverter<Dictionary<TKey, TElement>, IEnumerable<KeyValuePair<TKey, TElement>>>,
        IPropertyConverter<IDictionary<TKey, TElement>, IEnumerable<KeyValuePair<TKey, TElement>>>,
        IPropertyConverter<IEnumerable<KeyValuePair<TKey, TElement>>, IEnumerable<KeyValuePair<TKey, TElement>>>
    {
        public Task<Dictionary<TKey, TElement>> Convert<TMessage>(InitializeContext<TMessage> context,
            IEnumerable<KeyValuePair<TKey, TElement>> input)
            where TMessage : class
        {
            return Task.FromResult(input?.ToDictionary(x => x.Key, x => x.Value));
        }

        Task<IDictionary<TKey, TElement>> IPropertyConverter<IDictionary<TKey, TElement>, IEnumerable<KeyValuePair<TKey, TElement>>>
            .Convert<TMessage>(InitializeContext<TMessage> context, IEnumerable<KeyValuePair<TKey, TElement>> input)
        {
            return Task.FromResult<IDictionary<TKey, TElement>>(input?.ToDictionary(x => x.Key, x => x.Value));
        }

        Task<IEnumerable<KeyValuePair<TKey, TElement>>>
            IPropertyConverter<IEnumerable<KeyValuePair<TKey, TElement>>, IEnumerable<KeyValuePair<TKey, TElement>>>.Convert<TMessage>(
                InitializeContext<TMessage> context, IEnumerable<KeyValuePair<TKey, TElement>> input)
        {
            return Task.FromResult<IEnumerable<KeyValuePair<TKey, TElement>>>(input?.ToDictionary(x => x.Key, x => x.Value));
        }
    }


    public class DictionaryPropertyConverter<TKey, TElement, TInputElement> :
        IPropertyConverter<Dictionary<TKey, TElement>, IEnumerable<KeyValuePair<TKey, TInputElement>>>,
        IPropertyConverter<IDictionary<TKey, TElement>, IEnumerable<KeyValuePair<TKey, TInputElement>>>,
        IPropertyConverter<IEnumerable<KeyValuePair<TKey, TElement>>, IEnumerable<KeyValuePair<TKey, TInputElement>>>
    {
        readonly IPropertyConverter<TElement, TInputElement> _converter;

        public DictionaryPropertyConverter(IPropertyConverter<TElement, TInputElement> converter)
        {
            _converter = converter;
        }

        public async Task<Dictionary<TKey, TElement>> Convert<TMessage>(InitializeContext<TMessage> context,
            IEnumerable<KeyValuePair<TKey, TInputElement>> input)
            where TMessage : class
        {
            if (input == null)
                return default;

            (TKey Key, TElement Value)[] elements = await Task
                .WhenAll(input.Select(async x => (x.Key, await _converter.Convert(context, x.Value).ConfigureAwait(false))))
                .ConfigureAwait(false);

            return elements.ToDictionary(x => x.Key, x => x.Value);
        }

        async Task<IDictionary<TKey, TElement>> IPropertyConverter<IDictionary<TKey, TElement>, IEnumerable<KeyValuePair<TKey, TInputElement>>>.
            Convert<TMessage>(InitializeContext<TMessage> context, IEnumerable<KeyValuePair<TKey, TInputElement>> input)
        {
            return await Convert(context, input).ConfigureAwait(false);
        }

        async Task<IEnumerable<KeyValuePair<TKey, TElement>>>
            IPropertyConverter<IEnumerable<KeyValuePair<TKey, TElement>>, IEnumerable<KeyValuePair<TKey, TInputElement>>>.Convert<TMessage>(
                InitializeContext<TMessage> context, IEnumerable<KeyValuePair<TKey, TInputElement>> input)
        {
            return await Convert(context, input).ConfigureAwait(false);
        }
    }


    public class DictionaryKeyPropertyConverter<TKey, TInputKey, TElement> :
        IPropertyConverter<Dictionary<TKey, TElement>, IEnumerable<KeyValuePair<TInputKey, TElement>>>,
        IPropertyConverter<IDictionary<TKey, TElement>, IEnumerable<KeyValuePair<TInputKey, TElement>>>,
        IPropertyConverter<IEnumerable<KeyValuePair<TKey, TElement>>, IEnumerable<KeyValuePair<TInputKey, TElement>>>
    {
        readonly IPropertyConverter<TKey, TInputKey> _converter;

        public DictionaryKeyPropertyConverter(IPropertyConverter<TKey, TInputKey> converter)
        {
            _converter = converter;
        }

        public async Task<Dictionary<TKey, TElement>> Convert<TMessage>(InitializeContext<TMessage> context,
            IEnumerable<KeyValuePair<TInputKey, TElement>> input)
            where TMessage : class
        {
            if (input == null)
                return default;

            (TKey Key, TElement Value)[] elements = await Task
                .WhenAll(input.Select(async x => (await _converter.Convert(context, x.Key).ConfigureAwait(false), x.Value)))
                .ConfigureAwait(false);

            return elements.ToDictionary(x => x.Key, x => x.Value);
        }

        async Task<IDictionary<TKey, TElement>> IPropertyConverter<IDictionary<TKey, TElement>, IEnumerable<KeyValuePair<TInputKey, TElement>>>.
            Convert<TMessage>(InitializeContext<TMessage> context, IEnumerable<KeyValuePair<TInputKey, TElement>> input)
        {
            return await Convert(context, input).ConfigureAwait(false);
        }

        async Task<IEnumerable<KeyValuePair<TKey, TElement>>>
            IPropertyConverter<IEnumerable<KeyValuePair<TKey, TElement>>, IEnumerable<KeyValuePair<TInputKey, TElement>>>.Convert<TMessage>(
                InitializeContext<TMessage> context, IEnumerable<KeyValuePair<TInputKey, TElement>> input)
        {
            return await Convert(context, input).ConfigureAwait(false);
        }
    }


    public class DictionaryPropertyConverter<TKey, TElement, TInputKey, TInputElement> :
        IPropertyConverter<Dictionary<TKey, TElement>, IEnumerable<KeyValuePair<TInputKey, TInputElement>>>,
        IPropertyConverter<IDictionary<TKey, TElement>, IEnumerable<KeyValuePair<TInputKey, TInputElement>>>,
        IPropertyConverter<IEnumerable<KeyValuePair<TKey, TElement>>, IEnumerable<KeyValuePair<TInputKey, TInputElement>>>
    {
        readonly IPropertyConverter<TElement, TInputElement> _elementConverter;
        readonly IPropertyConverter<TKey, TInputKey> _keyConverter;

        public DictionaryPropertyConverter(IPropertyConverter<TKey, TInputKey> keyConverter,
            IPropertyConverter<TElement, TInputElement> elementConverter)
        {
            _elementConverter = elementConverter;
            _keyConverter = keyConverter;
        }

        public async Task<Dictionary<TKey, TElement>> Convert<TMessage>(InitializeContext<TMessage> context,
            IEnumerable<KeyValuePair<TInputKey, TInputElement>> input)
            where TMessage : class
        {
            if (input == null)
                return default;

            (TKey Key, TElement Value)[] elements = await Task
                .WhenAll(input.Select(async x => (await _keyConverter.Convert(context, x.Key).ConfigureAwait(false),
                    await _elementConverter.Convert(context, x.Value).ConfigureAwait(false))))
                .ConfigureAwait(false);

            return elements.ToDictionary(x => x.Key, x => x.Value);
        }

        async Task<IDictionary<TKey, TElement>> IPropertyConverter<IDictionary<TKey, TElement>, IEnumerable<KeyValuePair<TInputKey, TInputElement>>>
            .Convert<TMessage>(InitializeContext<TMessage> context, IEnumerable<KeyValuePair<TInputKey, TInputElement>> input)
        {
            return await Convert(context, input).ConfigureAwait(false);
        }

        async Task<IEnumerable<KeyValuePair<TKey, TElement>>> IPropertyConverter<IEnumerable<KeyValuePair<TKey, TElement>>,
            IEnumerable<KeyValuePair<TInputKey, TInputElement>>>.Convert<TMessage>(InitializeContext<TMessage> context,
            IEnumerable<KeyValuePair<TInputKey, TInputElement>> input)
        {
            return await Convert(context, input).ConfigureAwait(false);
        }
    }
}
