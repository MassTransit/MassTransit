namespace MassTransit.Initializers.PropertyConverters
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Util;


    public class DictionaryPropertyConverter<TKey, TElement> :
        IPropertyConverter<Dictionary<TKey, TElement>, IEnumerable<KeyValuePair<TKey, TElement>>>,
        IPropertyConverter<IDictionary<TKey, TElement>, IEnumerable<KeyValuePair<TKey, TElement>>>,
        IPropertyConverter<IReadOnlyDictionary<TKey, TElement>, IEnumerable<KeyValuePair<TKey, TElement>>>,
        IPropertyConverter<IEnumerable<KeyValuePair<TKey, TElement>>, IEnumerable<KeyValuePair<TKey, TElement>>>
    {
        public Task<Dictionary<TKey, TElement>> Convert<TMessage>(InitializeContext<TMessage> context,
            IEnumerable<KeyValuePair<TKey, TElement>> input)
            where TMessage : class
        {
            switch (input)
            {
                case null:
                    return TaskUtil.Default<Dictionary<TKey, TElement>>();
                case Dictionary<TKey, TElement> dictionary:
                    return Task.FromResult(dictionary);
                default:
                    return Task.FromResult(input.ToDictionary(x => x.Key, x => x.Value));
            }
        }

        Task<IDictionary<TKey, TElement>> IPropertyConverter<IDictionary<TKey, TElement>, IEnumerable<KeyValuePair<TKey, TElement>>>
            .Convert<TMessage>(InitializeContext<TMessage> context, IEnumerable<KeyValuePair<TKey, TElement>> input)
        {
            switch (input)
            {
                case null:
                    return TaskUtil.Default<IDictionary<TKey, TElement>>();
                case IDictionary<TKey, TElement> dictionary:
                    return Task.FromResult(dictionary);
                default:
                    return Task.FromResult<IDictionary<TKey, TElement>>(input.ToDictionary(x => x.Key, x => x.Value));
            }
        }

        Task<IEnumerable<KeyValuePair<TKey, TElement>>>
            IPropertyConverter<IEnumerable<KeyValuePair<TKey, TElement>>, IEnumerable<KeyValuePair<TKey, TElement>>>.Convert<TMessage>(
                InitializeContext<TMessage> context, IEnumerable<KeyValuePair<TKey, TElement>> input)
        {
            switch (input)
            {
                case null:
                    return TaskUtil.Default<IEnumerable<KeyValuePair<TKey, TElement>>>();
                default:
                    return Task.FromResult(input);
            }
        }

        Task<IReadOnlyDictionary<TKey, TElement>> IPropertyConverter<IReadOnlyDictionary<TKey, TElement>, IEnumerable<KeyValuePair<TKey, TElement>>>
            .Convert<TMessage>(InitializeContext<TMessage> context, IEnumerable<KeyValuePair<TKey, TElement>> input)
        {
            switch (input)
            {
                case null:
                    return TaskUtil.Default<IReadOnlyDictionary<TKey, TElement>>();
                case IReadOnlyDictionary<TKey, TElement> dictionary:
                    return Task.FromResult(dictionary);
                default:
                    return Task.FromResult<IReadOnlyDictionary<TKey, TElement>>(input.ToDictionary(x => x.Key, x => x.Value));
            }
        }
    }


    public class DictionaryPropertyConverter<TKey, TElement, TInputElement> :
        IPropertyConverter<Dictionary<TKey, TElement>, IEnumerable<KeyValuePair<TKey, TInputElement>>>,
        IPropertyConverter<IDictionary<TKey, TElement>, IEnumerable<KeyValuePair<TKey, TInputElement>>>,
        IPropertyConverter<IReadOnlyDictionary<TKey, TElement>, IEnumerable<KeyValuePair<TKey, TInputElement>>>,
        IPropertyConverter<IEnumerable<KeyValuePair<TKey, TElement>>, IEnumerable<KeyValuePair<TKey, TInputElement>>>
    {
        readonly IPropertyConverter<TElement, TInputElement> _converter;

        public DictionaryPropertyConverter(IPropertyConverter<TElement, TInputElement> converter)
        {
            _converter = converter;
        }

        public Task<Dictionary<TKey, TElement>> Convert<TMessage>(InitializeContext<TMessage> context,
            IEnumerable<KeyValuePair<TKey, TInputElement>> input)
            where TMessage : class
        {
            return ConvertSync(context, input);
        }

        Task<IDictionary<TKey, TElement>> IPropertyConverter<IDictionary<TKey, TElement>, IEnumerable<KeyValuePair<TKey, TInputElement>>>.
            Convert<TMessage>(InitializeContext<TMessage> context, IEnumerable<KeyValuePair<TKey, TInputElement>> input)
        {
            Task<Dictionary<TKey, TElement>> resultTask = ConvertSync(context, input);
            if (resultTask.Status == TaskStatus.RanToCompletion)
                return Task.FromResult<IDictionary<TKey, TElement>>(resultTask.Result);

            async Task<IDictionary<TKey, TElement>> ConvertAsync()
            {
                return await resultTask.ConfigureAwait(false);
            }

            return ConvertAsync();
        }

        Task<IEnumerable<KeyValuePair<TKey, TElement>>>
            IPropertyConverter<IEnumerable<KeyValuePair<TKey, TElement>>, IEnumerable<KeyValuePair<TKey, TInputElement>>>.Convert<TMessage>(
                InitializeContext<TMessage> context, IEnumerable<KeyValuePair<TKey, TInputElement>> input)
        {
            Task<Dictionary<TKey, TElement>> resultTask = ConvertSync(context, input);
            if (resultTask.Status == TaskStatus.RanToCompletion)
                return Task.FromResult<IEnumerable<KeyValuePair<TKey, TElement>>>(resultTask.Result);

            async Task<IEnumerable<KeyValuePair<TKey, TElement>>> ConvertAsync()
            {
                return await resultTask.ConfigureAwait(false);
            }

            return ConvertAsync();
        }

        Task<IReadOnlyDictionary<TKey, TElement>> IPropertyConverter<IReadOnlyDictionary<TKey, TElement>, IEnumerable<KeyValuePair<TKey, TInputElement>>>.
            Convert<TMessage>(InitializeContext<TMessage> context, IEnumerable<KeyValuePair<TKey, TInputElement>> input)
        {
            Task<Dictionary<TKey, TElement>> resultTask = ConvertSync(context, input);
            if (resultTask.Status == TaskStatus.RanToCompletion)
                return Task.FromResult<IReadOnlyDictionary<TKey, TElement>>(resultTask.Result);

            async Task<IReadOnlyDictionary<TKey, TElement>> ConvertAsync()
            {
                return await resultTask.ConfigureAwait(false);
            }

            return ConvertAsync();
        }

        Task<Dictionary<TKey, TElement>> ConvertSync<TMessage>(InitializeContext<TMessage> context,
            IEnumerable<KeyValuePair<TKey, TInputElement>> input)
            where TMessage : class
        {
            if (input == null)
                return TaskUtil.Default<Dictionary<TKey, TElement>>();

            var capacity = 0;
            if (input is ICollection<TElement> collection)
            {
                capacity = collection.Count;
                if (capacity == 0)
                    return Task.FromResult(new Dictionary<TKey, TElement>());
            }

            var results = new Dictionary<TKey, TElement>(capacity);
            IEnumerator<KeyValuePair<TKey, TInputElement>> enumerator = input.GetEnumerator();
            var disposeEnumerator = true;
            try
            {
                async Task<Dictionary<TKey, TElement>> ConvertAsync(IEnumerator<KeyValuePair<TKey, TInputElement>> asyncEnumerator, Task<TElement> elementTask)
                {
                    try
                    {
                        var element = await elementTask.ConfigureAwait(false);

                        results.Add(asyncEnumerator.Current.Key, element);

                        while (asyncEnumerator.MoveNext())
                        {
                            KeyValuePair<TKey, TInputElement> current = asyncEnumerator.Current;

                            elementTask = _converter.Convert(context, current.Value);
                            if (elementTask.Status == TaskStatus.RanToCompletion)
                                results.Add(current.Key, elementTask.Result);
                            else
                            {
                                element = await elementTask.ConfigureAwait(false);

                                results.Add(asyncEnumerator.Current.Key, element);
                            }
                        }

                        return results;
                    }
                    finally
                    {
                        asyncEnumerator.Dispose();
                    }
                }

                while (enumerator.MoveNext())
                {
                    KeyValuePair<TKey, TInputElement> current = enumerator.Current;

                    Task<TElement> elementTask = _converter.Convert(context, current.Value);
                    if (elementTask.Status == TaskStatus.RanToCompletion)
                        results.Add(current.Key, elementTask.Result);
                    else
                    {
                        disposeEnumerator = false;
                        return ConvertAsync(enumerator, elementTask);
                    }
                }
            }
            finally
            {
                if (disposeEnumerator)
                    enumerator.Dispose();
            }

            return Task.FromResult(results);
        }
    }


    public class DictionaryKeyPropertyConverter<TKey, TInputKey, TElement> :
        IPropertyConverter<Dictionary<TKey, TElement>, IEnumerable<KeyValuePair<TInputKey, TElement>>>,
        IPropertyConverter<IDictionary<TKey, TElement>, IEnumerable<KeyValuePair<TInputKey, TElement>>>,
        IPropertyConverter<IReadOnlyDictionary<TKey, TElement>, IEnumerable<KeyValuePair<TInputKey, TElement>>>,
        IPropertyConverter<IEnumerable<KeyValuePair<TKey, TElement>>, IEnumerable<KeyValuePair<TInputKey, TElement>>>
    {
        readonly IPropertyConverter<TKey, TInputKey> _converter;

        public DictionaryKeyPropertyConverter(IPropertyConverter<TKey, TInputKey> converter)
        {
            _converter = converter;
        }

        public Task<Dictionary<TKey, TElement>> Convert<TMessage>(InitializeContext<TMessage> context,
            IEnumerable<KeyValuePair<TInputKey, TElement>> input)
            where TMessage : class
        {
            return ConvertSync(context, input);
        }

        Task<IDictionary<TKey, TElement>> IPropertyConverter<IDictionary<TKey, TElement>, IEnumerable<KeyValuePair<TInputKey, TElement>>>.
            Convert<TMessage>(InitializeContext<TMessage> context, IEnumerable<KeyValuePair<TInputKey, TElement>> input)
        {
            Task<Dictionary<TKey, TElement>> resultTask = ConvertSync(context, input);
            if (resultTask.IsCompleted)
                return Task.FromResult<IDictionary<TKey, TElement>>(resultTask.Result);

            async Task<IDictionary<TKey, TElement>> ConvertAsync()
            {
                return await resultTask.ConfigureAwait(false);
            }

            return ConvertAsync();
        }

        Task<IEnumerable<KeyValuePair<TKey, TElement>>>
            IPropertyConverter<IEnumerable<KeyValuePair<TKey, TElement>>, IEnumerable<KeyValuePair<TInputKey, TElement>>>.Convert<TMessage>(
                InitializeContext<TMessage> context, IEnumerable<KeyValuePair<TInputKey, TElement>> input)
        {
            Task<Dictionary<TKey, TElement>> resultTask = ConvertSync(context, input);
            if (resultTask.Status == TaskStatus.RanToCompletion)
                return Task.FromResult<IEnumerable<KeyValuePair<TKey, TElement>>>(resultTask.Result);

            async Task<IEnumerable<KeyValuePair<TKey, TElement>>> ConvertAsync()
            {
                return await resultTask.ConfigureAwait(false);
            }

            return ConvertAsync();
        }

        Task<IReadOnlyDictionary<TKey, TElement>> IPropertyConverter<IReadOnlyDictionary<TKey, TElement>, IEnumerable<KeyValuePair<TInputKey, TElement>>>.
            Convert<TMessage>(InitializeContext<TMessage> context, IEnumerable<KeyValuePair<TInputKey, TElement>> input)
        {
            Task<Dictionary<TKey, TElement>> resultTask = ConvertSync(context, input);
            if (resultTask.Status == TaskStatus.RanToCompletion)
                return Task.FromResult<IReadOnlyDictionary<TKey, TElement>>(resultTask.Result);

            async Task<IReadOnlyDictionary<TKey, TElement>> ConvertAsync()
            {
                return await resultTask.ConfigureAwait(false);
            }

            return ConvertAsync();
        }

        Task<Dictionary<TKey, TElement>> ConvertSync<TMessage>(InitializeContext<TMessage> context,
            IEnumerable<KeyValuePair<TInputKey, TElement>> input)
            where TMessage : class
        {
            if (input == null)
                return TaskUtil.Default<Dictionary<TKey, TElement>>();

            var capacity = 0;
            if (input is ICollection<TElement> collection)
            {
                capacity = collection.Count;
                if (capacity == 0)
                    return Task.FromResult(new Dictionary<TKey, TElement>());
            }

            var results = new Dictionary<TKey, TElement>(capacity);
            IEnumerator<KeyValuePair<TInputKey, TElement>> enumerator = input.GetEnumerator();
            var disposeEnumerator = true;
            try
            {
                async Task<Dictionary<TKey, TElement>> ConvertAsync(IEnumerator<KeyValuePair<TInputKey, TElement>> asyncEnumerator, Task<TKey> keyTask)
                {
                    try
                    {
                        var key = await keyTask.ConfigureAwait(false);

                        results.Add(key, asyncEnumerator.Current.Value);

                        while (asyncEnumerator.MoveNext())
                        {
                            KeyValuePair<TInputKey, TElement> current = asyncEnumerator.Current;

                            keyTask = _converter.Convert(context, current.Key);
                            if (keyTask.Status == TaskStatus.RanToCompletion)
                                results.Add(keyTask.Result, current.Value);
                            else
                            {
                                key = await keyTask.ConfigureAwait(false);

                                results.Add(key, asyncEnumerator.Current.Value);
                            }
                        }

                        return results;
                    }
                    finally
                    {
                        asyncEnumerator.Dispose();
                    }
                }

                while (enumerator.MoveNext())
                {
                    KeyValuePair<TInputKey, TElement> current = enumerator.Current;

                    Task<TKey> keyTask = _converter.Convert(context, current.Key);
                    if (keyTask.Status == TaskStatus.RanToCompletion)
                        results.Add(keyTask.Result, current.Value);
                    else
                    {
                        disposeEnumerator = false;
                        return ConvertAsync(enumerator, keyTask);
                    }
                }
            }
            finally
            {
                if (disposeEnumerator)
                    enumerator.Dispose();
            }

            return Task.FromResult(results);
        }
    }


    public class DictionaryPropertyConverter<TKey, TElement, TInputKey, TInputElement> :
        IPropertyConverter<Dictionary<TKey, TElement>, IEnumerable<KeyValuePair<TInputKey, TInputElement>>>,
        IPropertyConverter<IDictionary<TKey, TElement>, IEnumerable<KeyValuePair<TInputKey, TInputElement>>>,
        IPropertyConverter<IReadOnlyDictionary<TKey, TElement>, IEnumerable<KeyValuePair<TInputKey, TInputElement>>>,
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

        public Task<Dictionary<TKey, TElement>> Convert<TMessage>(InitializeContext<TMessage> context,
            IEnumerable<KeyValuePair<TInputKey, TInputElement>> input)
            where TMessage : class
        {
            return ConvertSync(context, input);
        }

        Task<IDictionary<TKey, TElement>> IPropertyConverter<IDictionary<TKey, TElement>, IEnumerable<KeyValuePair<TInputKey, TInputElement>>>
            .Convert<TMessage>(InitializeContext<TMessage> context, IEnumerable<KeyValuePair<TInputKey, TInputElement>> input)
        {
            Task<Dictionary<TKey, TElement>> resultTask = ConvertSync(context, input);
            if (resultTask.Status == TaskStatus.RanToCompletion)
                return Task.FromResult<IDictionary<TKey, TElement>>(resultTask.Result);

            async Task<IDictionary<TKey, TElement>> ConvertAsync()
            {
                return await resultTask.ConfigureAwait(false);
            }

            return ConvertAsync();
        }

        Task<IEnumerable<KeyValuePair<TKey, TElement>>> IPropertyConverter<IEnumerable<KeyValuePair<TKey, TElement>>,
            IEnumerable<KeyValuePair<TInputKey, TInputElement>>>.Convert<TMessage>(InitializeContext<TMessage> context,
            IEnumerable<KeyValuePair<TInputKey, TInputElement>> input)
        {
            Task<Dictionary<TKey, TElement>> resultTask = ConvertSync(context, input);
            if (resultTask.Status == TaskStatus.RanToCompletion)
                return Task.FromResult<IEnumerable<KeyValuePair<TKey, TElement>>>(resultTask.Result);

            async Task<IEnumerable<KeyValuePair<TKey, TElement>>> ConvertAsync()
            {
                return await resultTask.ConfigureAwait(false);
            }

            return ConvertAsync();
        }

        Task<IReadOnlyDictionary<TKey, TElement>>
            IPropertyConverter<IReadOnlyDictionary<TKey, TElement>, IEnumerable<KeyValuePair<TInputKey, TInputElement>>>.
            Convert<TMessage>(InitializeContext<TMessage> context, IEnumerable<KeyValuePair<TInputKey, TInputElement>> input)
        {
            Task<Dictionary<TKey, TElement>> resultTask = ConvertSync(context, input);
            if (resultTask.Status == TaskStatus.RanToCompletion)
                return Task.FromResult<IReadOnlyDictionary<TKey, TElement>>(resultTask.Result);

            async Task<IReadOnlyDictionary<TKey, TElement>> ConvertAsync()
            {
                return await resultTask.ConfigureAwait(false);
            }

            return ConvertAsync();
        }

        Task<Dictionary<TKey, TElement>> ConvertSync<TMessage>(InitializeContext<TMessage> context,
            IEnumerable<KeyValuePair<TInputKey, TInputElement>> input)
            where TMessage : class
        {
            if (input == null)
                return TaskUtil.Default<Dictionary<TKey, TElement>>();

            var capacity = 0;
            if (input is ICollection<TElement> collection)
            {
                capacity = collection.Count;
                if (capacity == 0)
                    return Task.FromResult(new Dictionary<TKey, TElement>());
            }

            var results = new Dictionary<TKey, TElement>(capacity);
            IEnumerator<KeyValuePair<TInputKey, TInputElement>> enumerator = input.GetEnumerator();
            var disposeEnumerator = true;
            try
            {
                async Task<Dictionary<TKey, TElement>> ConvertAsync(IEnumerator<KeyValuePair<TInputKey, TInputElement>> asyncEnumerator, Task<TKey> keyTask,
                    Task<TElement> elementTask)
                {
                    try
                    {
                        var key = keyTask.Status == TaskStatus.RanToCompletion ? keyTask.Result : await keyTask.ConfigureAwait(false);
                        var element = elementTask.Status == TaskStatus.RanToCompletion ? elementTask.Result : await elementTask.ConfigureAwait(false);

                        results.Add(key, element);

                        while (asyncEnumerator.MoveNext())
                        {
                            KeyValuePair<TInputKey, TInputElement> current = asyncEnumerator.Current;

                            keyTask = _keyConverter.Convert(context, current.Key);
                            elementTask = _elementConverter.Convert(context, current.Value);

                            key = keyTask.IsCompleted ? keyTask.Result : await keyTask.ConfigureAwait(false);
                            element = elementTask.IsCompleted ? elementTask.Result : await elementTask.ConfigureAwait(false);

                            results.Add(key, element);
                        }

                        return results;
                    }
                    finally
                    {
                        asyncEnumerator.Dispose();
                    }
                }

                while (enumerator.MoveNext())
                {
                    KeyValuePair<TInputKey, TInputElement> current = enumerator.Current;

                    Task<TKey> keyTask = _keyConverter.Convert(context, current.Key);
                    Task<TElement> elementTask = _elementConverter.Convert(context, current.Value);
                    if (keyTask.Status == TaskStatus.RanToCompletion && elementTask.Status == TaskStatus.RanToCompletion)
                        results.Add(keyTask.Result, elementTask.Result);
                    else
                    {
                        disposeEnumerator = false;
                        return ConvertAsync(enumerator, keyTask, elementTask);
                    }
                }
            }
            finally
            {
                if (disposeEnumerator)
                    enumerator.Dispose();
            }

            return Task.FromResult(results);
        }
    }
}
