namespace MassTransit.Initializers.PropertyConverters
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Util;


    public class ArrayPropertyConverter<TElement> :
        IPropertyConverter<TElement[], IEnumerable<TElement>>
    {
        public Task<TElement[]> Convert<TMessage>(InitializeContext<TMessage> context, IEnumerable<TElement> input)
            where TMessage : class
        {
            switch (input)
            {
                case null:
                    return TaskUtil.Default<TElement[]>();
                case TElement[] array:
                    return Task.FromResult(array);
                default:
                    return Task.FromResult(input.ToArray());
            }
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
            var resultTask = ConvertSync(context, input);
            if (resultTask.IsCompleted)
                return Task.FromResult(resultTask.Result);

            async Task<TElement[]> ConvertAsync()
            {
                return await resultTask.ConfigureAwait(false);
            }

            return ConvertAsync();
        }

        Task<TElement[]> ConvertSync<TMessage>(InitializeContext<TMessage> context, IEnumerable<TInputElement> input)
            where TMessage : class
        {
            if (input == null)
                return TaskUtil.Default<TElement[]>();

            int capacity = 0;
            if (input is ICollection<TElement> collection)
            {
                capacity = collection.Count;
                if (capacity == 0)
                    return Task.FromResult(_emptyArray);
            }

            List<TElement> results = new List<TElement>(capacity);
            IEnumerator<TInputElement> enumerator = input.GetEnumerator();
            bool disposeEnumerator = true;
            try
            {
                async Task<TElement[]> ConvertAsync(IEnumerator<TInputElement> asyncEnumerator, Task<TElement> elementTask)
                {
                    try
                    {
                        var element = await elementTask.ConfigureAwait(false);

                        results.Add(element);

                        while (asyncEnumerator.MoveNext())
                        {
                            var current = asyncEnumerator.Current;

                            elementTask = _converter.Convert(context, current);
                            if (elementTask.IsCompleted)
                                results.Add(elementTask.Result);
                            else
                            {
                                element = await elementTask.ConfigureAwait(false);

                                results.Add(element);
                            }
                        }

                        return results.ToArray();
                    }
                    finally
                    {
                        asyncEnumerator.Dispose();
                    }
                }

                while (enumerator.MoveNext())
                {
                    var current = enumerator.Current;

                    var elementTask = _converter.Convert(context, current);
                    if (elementTask.IsCompleted)
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

            return Task.FromResult(results.ToArray());
        }

        static readonly TElement[] _emptyArray = new TElement[0];
    }
}
