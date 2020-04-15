namespace MassTransit.WindsorIntegration.ScopeProviders
{
    using System;
    using Castle.MicroKernel;
    using Context;
    using GreenPipes;
    using Scoping;
    using Scoping.SendContexts;


    public class WindsorSendScopeProvider :
        ISendScopeProvider
    {
        readonly IKernel _kernel;

        public WindsorSendScopeProvider(IKernel kernel)
        {
            _kernel = kernel;
        }

        public void Probe(ProbeContext context)
        {
            context.Add("provider", "windsor");
        }

        public ISendScopeContext<T> GetScope<T>(SendContext<T> context)
            where T : class
        {
            if (context.TryGetPayload<IKernel>(out _))
                return new ExistingSendScopeContext<T>(context);

            var scope = _kernel.CreateNewMessageScope(context);
            try
            {
                var sendContext = new SendContextScope<T>(context, _kernel);

                return new CreatedSendScopeContext<IDisposable, T>(scope, sendContext);
            }
            catch
            {
                scope?.Dispose();
                throw;
            }
        }
    }
}
