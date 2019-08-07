namespace MassTransit.Initializers.PropertyConverters
{
    using System;
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
        public Task<List<TElement>> Convert<TMessage>(InitializeContext<TMessage> context, IEnumerable<TElement> input)
            where TMessage : class
        {
            return Task.FromResult(input?.ToList());
        }

        Task<IList<TElement>> IPropertyConverter<IList<TElement>, IEnumerable<TElement>>.Convert<T>(InitializeContext<T> context, IEnumerable<TElement> input)
        {
            return Task.FromResult<IList<TElement>>(input?.ToList());
        }

        Task<IEnumerable<TElement>> IPropertyConverter<IEnumerable<TElement>, IEnumerable<TElement>>.Convert<T>(InitializeContext<T> context, IEnumerable<TElement> input)
        {
            return Task.FromResult<IEnumerable<TElement>>(input?.ToList());
        }

        Task<IReadOnlyList<TElement>> IPropertyConverter<IReadOnlyList<TElement>, IEnumerable<TElement>>.Convert<T>(InitializeContext<T> context, IEnumerable<TElement> input)
        {
            return Task.FromResult<IReadOnlyList<TElement>>(input?.ToList());
        }
    }


    public class ListPropertyConverter<TElement, TInputElement> :
        IPropertyConverter<List<TElement>, IEnumerable<TInputElement>>,
        IPropertyConverter<IList<TElement>, IEnumerable<TInputElement>>,
        IPropertyConverter<IReadOnlyList<TElement>, IEnumerable<TInputElement>>,
        IPropertyConverter<IEnumerable<TElement>, IEnumerable<TInputElement>>
    {
        readonly IPropertyConverter<TElement, TInputElement> _propertyConverter;

        public ListPropertyConverter(IPropertyConverter<TElement, TInputElement> converter)
        {
            _propertyConverter = converter;
        }

        Task<List<TElement>> IPropertyConverter<List<TElement>, IEnumerable<TInputElement>>.Convert<T>(InitializeContext<T> context, IEnumerable<TInputElement> elements)
        {
            return Convert(context, elements, x => x);
        }

        Task<IList<TElement>> IPropertyConverter<IList<TElement>, IEnumerable<TInputElement>>.Convert<T>(InitializeContext<T> context, IEnumerable<TInputElement> elements)
        {
            return Convert<IList<TElement>, T>(context, elements, x => x);
        }

        Task<IEnumerable<TElement>> IPropertyConverter<IEnumerable<TElement>, IEnumerable<TInputElement>>.Convert<T>(InitializeContext<T> context,
            IEnumerable<TInputElement> elements)
        {
            return Convert<IEnumerable<TElement>, T>(context, elements, x => x);
        }

        Task<IReadOnlyList<TElement>> IPropertyConverter<IReadOnlyList<TElement>, IEnumerable<TInputElement>>.Convert<T>(InitializeContext<T> context,
            IEnumerable<TInputElement> elements)
        {
            return Convert<IReadOnlyList<TElement>, T>(context, elements, x => x);
        }

        Task<TResult> Convert<TResult, T>(InitializeContext<T> context, IEnumerable<TInputElement> elements, Func<List<TElement>, TResult> selector)
            where T : class
        {
            if (elements == null)
                return TaskUtil.Default<TResult>();

            return ConvertElementsAsync(context, elements, selector);
        }

        async Task<TResult> ConvertElementsAsync<TResult, T>(InitializeContext<T> context, IEnumerable<TInputElement> inputElements,
            Func<List<TElement>, TResult> selector)
            where T : class
        {
            if (inputElements == null)
                return default;

            List<TElement> elements = new List<TElement>();
            foreach(var inputElement in inputElements)
            {
                var element = await _propertyConverter.Convert(context, inputElement).ConfigureAwait(false);

                elements.Add(element);
            }

            return selector(elements);
        }
    }
}
