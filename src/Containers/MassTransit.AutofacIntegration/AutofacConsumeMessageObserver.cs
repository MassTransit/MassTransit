// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.AutofacIntegration
{
    using System;
    using System.Threading.Tasks;
    using Autofac;
    using GreenPipes;


    public class AutofacConsumeMessageObserver<T> :
        IConsumeMessageObserver<T>
        where T : class
    {
        readonly ILifetimeScope _lifetimeScope;

        public AutofacConsumeMessageObserver()
        {
        }

        public AutofacConsumeMessageObserver(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;
        }

        Task IConsumeMessageObserver<T>.PreConsume(ConsumeContext<T> context)
        {
            var scope = GetLifetimeScope(context);

            var observer = scope.Resolve<IConsumeMessageObserver<T>>();

            return observer.PreConsume(context);
        }

        Task IConsumeMessageObserver<T>.PostConsume(ConsumeContext<T> context)
        {
            var scope = GetLifetimeScope(context);

            var observer = scope.Resolve<IConsumeMessageObserver<T>>();

            return observer.PostConsume(context);
        }

        Task IConsumeMessageObserver<T>.ConsumeFault(ConsumeContext<T> context, Exception exception)
        {
            var scope = GetLifetimeScope(context);

            var observer = scope.Resolve<IConsumeMessageObserver<T>>();

            return observer.ConsumeFault(context, exception);
        }

        ILifetimeScope GetLifetimeScope(PipeContext context)
        {
            if (context.TryGetPayload<ILifetimeScope>(out var payload))
                return payload;

            return _lifetimeScope ?? throw new MassTransitException("The lifetime scope was not in the payload, and a default lifetime scope was not specified.");
        }
    }
}