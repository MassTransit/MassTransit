namespace MassTransit.DependencyInjection
{
    using System;


    public class TypedScopedConsumeContextProvider :
        ScopedConsumeContextProvider
    {
        readonly IScopedConsumeContextProvider _global;

        public TypedScopedConsumeContextProvider(IScopedConsumeContextProvider global)
        {
            _global = global;
        }

        public override IDisposable PushContext(ConsumeContext context)
        {
            return new CombinedDisposable(_global.PushContext(context), base.PushContext(context));
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
