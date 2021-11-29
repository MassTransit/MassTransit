namespace MassTransit.Initializers.HeaderInitializers
{
    using System;
    using System.Threading.Tasks;


    /// <summary>
    /// Set a header to a constant value from the input
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="THeader">The header type</typeparam>
    public class SetHeaderInitializer<TMessage, TInput, THeader> :
        IHeaderInitializer<TMessage, TInput>
        where TMessage : class
        where TInput : class
    {
        readonly string _headerName;
        readonly IPropertyProvider<TInput, THeader> _provider;

        public SetHeaderInitializer(string headerName, IPropertyProvider<TInput, THeader> provider)
        {
            if (headerName == null)
                throw new ArgumentNullException(nameof(headerName));

            _headerName = headerName;
            _provider = provider;
        }

        public Task Apply(InitializeContext<TMessage, TInput> context, SendContext sendContext)
        {
            Task<THeader> propertyTask = _provider.GetProperty(context);
            if (propertyTask.IsCompleted)
            {
                sendContext.Headers.Set(_headerName, propertyTask.Result);
                return Task.CompletedTask;
            }

            async Task ApplyAsync()
            {
                var value = await propertyTask.ConfigureAwait(false);

                sendContext.Headers.Set(_headerName, value);
            }

            return ApplyAsync();
        }
    }
}
