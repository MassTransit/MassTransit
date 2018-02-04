namespace MassTransit.Initializers.PropertyProviders
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;


    public class EnumerablePropertyProvider<TInput, TProperty, TElement> :
        IPropertyProvider<TInput, IEnumerable<TElement>>
        where TInput : class
        where TProperty : class, IEnumerable<TElement>
    {
        readonly IPropertyProvider<TInput, TProperty> _provider;

        public EnumerablePropertyProvider(IPropertyProvider<TInput, TProperty> provider)
        {
            _provider = provider;
        }

        public async Task<IEnumerable<TElement>> GetProperty<T>(InitializeContext<T, TInput> context)
            where T : class
        {
            if (context.HasInput)
            {
                var propertyValue = await _provider.GetProperty<T>(context).ConfigureAwait(false);
                if (propertyValue != default)
                {
                    return propertyValue;
                }
            }

            return Enumerable.Empty<TElement>();
        }
    }
}
