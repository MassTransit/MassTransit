namespace MassTransit.Initializers.PropertyConverters
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Util;


    public class ListPropertyConverter<TElement> :
        IPropertyConverter<List<TElement>, IEnumerable<TElement>>,
        IPropertyConverter<IList<TElement>, IEnumerable<TElement>>,
        IPropertyConverter<IReadOnlyList<TElement>, IEnumerable<TElement>>,
        IPropertyConverter<IEnumerable<TElement>, IEnumerable<TElement>>
    {
        Task<IEnumerable<TElement>> IPropertyConverter<IEnumerable<TElement>, IEnumerable<TElement>>.Convert<T>(InitializeContext<T> context,
            IEnumerable<TElement> input)
        {
            switch (input)
            {
                case null:
                    return TaskUtil.Default<IEnumerable<TElement>>();
                default:
                    return Task.FromResult(input);
            }
        }

        Task<IList<TElement>> IPropertyConverter<IList<TElement>, IEnumerable<TElement>>.Convert<T>(InitializeContext<T> context, IEnumerable<TElement> input)
        {
            switch (input)
            {
                case null:
                    return TaskUtil.Default<IList<TElement>>();
                case IList<TElement> list:
                    return Task.FromResult(list);
                default:
                    return Task.FromResult<IList<TElement>>(input.ToList());
            }
        }

        Task<IReadOnlyList<TElement>> IPropertyConverter<IReadOnlyList<TElement>, IEnumerable<TElement>>.Convert<T>(InitializeContext<T> context,
            IEnumerable<TElement> input)
        {
            switch (input)
            {
                case null:
                    return TaskUtil.Default<IReadOnlyList<TElement>>();
                case IReadOnlyList<TElement> list:
                    return Task.FromResult(list);
                default:
                    return Task.FromResult<IReadOnlyList<TElement>>(input.ToList());
            }
        }

        public Task<List<TElement>> Convert<TMessage>(InitializeContext<TMessage> context, IEnumerable<TElement> input)
            where TMessage : class
        {
            switch (input)
            {
                case null:
                    return TaskUtil.Default<List<TElement>>();
                case List<TElement> list:
                    return Task.FromResult(list);
                default:
                    return Task.FromResult(input.ToList());
            }
        }
    }


    public class ListPropertyConverter<TElement, TInputElement> :
        IPropertyConverter<List<TElement>, IEnumerable<TInputElement>>,
        IPropertyConverter<IList<TElement>, IEnumerable<TInputElement>>,
        IPropertyConverter<IReadOnlyList<TElement>, IEnumerable<TInputElement>>,
        IPropertyConverter<IEnumerable<TElement>, IEnumerable<TInputElement>>
    {
        readonly IPropertyConverter<TElement, TInputElement> _converter;

        public ListPropertyConverter(IPropertyConverter<TElement, TInputElement> converter)
        {
            _converter = converter;
        }

        Task<IEnumerable<TElement>> IPropertyConverter<IEnumerable<TElement>, IEnumerable<TInputElement>>.Convert<T>(InitializeContext<T> context,
            IEnumerable<TInputElement> elements)
        {
            Task<List<TElement>> resultTask = ConvertSync(context, elements);
            if (resultTask.IsCompleted)
                return Task.FromResult<IEnumerable<TElement>>(resultTask.Result);

            async Task<IEnumerable<TElement>> ConvertAsync()
            {
                return await resultTask.ConfigureAwait(false);
            }

            return ConvertAsync();
        }

        Task<IList<TElement>> IPropertyConverter<IList<TElement>, IEnumerable<TInputElement>>.Convert<T>(InitializeContext<T> context,
            IEnumerable<TInputElement> elements)
        {
            Task<List<TElement>> resultTask = ConvertSync(context, elements);
            if (resultTask.Status == TaskStatus.RanToCompletion)
                return Task.FromResult<IList<TElement>>(resultTask.Result);

            async Task<IList<TElement>> ConvertAsync()
            {
                return await resultTask.ConfigureAwait(false);
            }

            return ConvertAsync();
        }

        Task<IReadOnlyList<TElement>> IPropertyConverter<IReadOnlyList<TElement>, IEnumerable<TInputElement>>.Convert<T>(InitializeContext<T> context,
            IEnumerable<TInputElement> elements)
        {
            Task<List<TElement>> resultTask = ConvertSync(context, elements);
            if (resultTask.Status == TaskStatus.RanToCompletion)
                return Task.FromResult<IReadOnlyList<TElement>>(resultTask.Result);

            async Task<IReadOnlyList<TElement>> ConvertAsync()
            {
                return await resultTask.ConfigureAwait(false);
            }

            return ConvertAsync();
        }

        Task<List<TElement>> IPropertyConverter<List<TElement>, IEnumerable<TInputElement>>.Convert<T>(InitializeContext<T> context,
            IEnumerable<TInputElement> elements)
        {
            return ConvertSync(context, elements);
        }

        Task<List<TElement>> ConvertSync<TMessage>(InitializeContext<TMessage> context, IEnumerable<TInputElement> input)
            where TMessage : class
        {
            if (input == null)
                return TaskUtil.Default<List<TElement>>();

            var capacity = 0;
            if (input is ICollection<TElement> collection)
            {
                capacity = collection.Count;
                if (capacity == 0)
                    return Task.FromResult(new List<TElement>());
            }

            var results = new List<TElement>(capacity);
            IEnumerator<TInputElement> enumerator = input.GetEnumerator();
            var disposeEnumerator = true;
            try
            {
                async Task<List<TElement>> ConvertAsync(IEnumerator<TInputElement> asyncEnumerator, Task<TElement> elementTask)
                {
                    try
                    {
                        var element = await elementTask.ConfigureAwait(false);

                        results.Add(element);

                        while (asyncEnumerator.MoveNext())
                        {
                            var current = asyncEnumerator.Current;

                            elementTask = _converter.Convert(context, current);
                            if (elementTask.Status == TaskStatus.RanToCompletion)
                                results.Add(elementTask.Result);
                            else
                            {
                                element = await elementTask.ConfigureAwait(false);

                                results.Add(element);
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
                    var current = enumerator.Current;

                    Task<TElement> elementTask = _converter.Convert(context, current);
                    if (elementTask.Status == TaskStatus.RanToCompletion)
                        results.Add(elementTask.Result);
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
}
