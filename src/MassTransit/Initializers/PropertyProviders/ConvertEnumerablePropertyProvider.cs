namespace MassTransit.Initializers.PropertyProviders
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;


    public class ConvertEnumerablePropertyProvider<TInput, TProperty, TElement, TInputElement> :
        IPropertyProvider<TInput, IEnumerable<TElement>>
        where TInput : class
        where TProperty : class, IEnumerable<TElement>
    {
        readonly IPropertyProvider<TInput, IEnumerable<TInputElement>> _provider;
        readonly IPropertyConverter<TElement, TInputElement> _converter;

        public ConvertEnumerablePropertyProvider(IPropertyProvider<TInput, IEnumerable<TInputElement>> provider,
            IPropertyConverter<TElement, TInputElement> converter)
        {
            _provider = provider;
            _converter = converter;
        }

        public async Task<IEnumerable<TElement>> GetProperty<T>(InitializeContext<T, TInput> context)
            where T : class
        {
            if (context.HasInput)
            {
                IEnumerable<TInputElement> propertyValue = await _provider.GetProperty(context).ConfigureAwait(false);
                if (propertyValue != null)
                {
                    var results = new List<TElement>();
                    foreach (var element in propertyValue)
                    {
                        var result = await _converter.Convert(context, element).ConfigureAwait(false);

                        results.Add(result);
                    }

                    return results;
                }
            }

            return Enumerable.Empty<TElement>();
        }
    }
}
