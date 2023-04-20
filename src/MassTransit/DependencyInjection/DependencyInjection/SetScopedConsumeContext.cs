namespace MassTransit.DependencyInjection
{
    using System;
    using Microsoft.Extensions.DependencyInjection;


    public class SetScopedConsumeContext :
        ISetScopedConsumeContext
    {
        readonly Func<IServiceProvider, IScopedConsumeContextProvider> _setterProvider;

        public SetScopedConsumeContext(Func<IServiceProvider, IScopedConsumeContextProvider> setterProvider)
        {
            _setterProvider = setterProvider;
        }

        public IDisposable PushContext(IServiceScope scope, ConsumeContext context)
        {
            return _setterProvider(scope.ServiceProvider).PushContext(context);
        }
    }
}
