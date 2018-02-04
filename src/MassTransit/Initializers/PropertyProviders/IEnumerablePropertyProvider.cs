namespace MassTransit.Initializers.PropertyProviders
{
    using System.Collections.Generic;
    using System.Threading.Tasks;


    public interface IEnumerablePropertyProvider<in TInput, TElement>
        where TInput : class
    {
        Task<IEnumerable<TElement>> GetProperty<T>(InitializeContext<T, TInput> context)
            where T : class;
    }
}