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


    public class SetScopedConsumeContext<TBus> :
        ISetScopedConsumeContext
        where TBus : class, IBus
    {
        readonly Func<IServiceProvider, IScopedConsumeContextProvider> _setterProvider;

        public SetScopedConsumeContext(Func<IServiceProvider, IScopedConsumeContextProvider> setterProvider)
        {
            _setterProvider = setterProvider;
        }

        public IDisposable PushContext(IServiceScope scope, ConsumeContext context)
        {
            var provider = scope.ServiceProvider;

            var busContextProvider = provider.GetRequiredService<IScopedBusContextProvider>();

            return new CombinedDisposable(
                _setterProvider(provider).PushContext(context),
                busContextProvider.PushContext(provider.GetRequiredService<IScopedBusContextProvider<TBus>>().Context));
        }


        class CombinedDisposable :
            IDisposable
        {
            readonly IDisposable[] _disposables;

            public CombinedDisposable(params IDisposable[] disposables)
            {
                _disposables = disposables;
            }

            public void Dispose()
            {
                for (var i = 0; i < _disposables.Length; i++)
                    _disposables[i].Dispose();
            }
        }
    }
}
