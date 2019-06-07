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
namespace Automatonymous.CorrelationConfigurators
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using MassTransit;


    public class EventMissingInstanceConfigurator<TInstance, TData> :
        IMissingInstanceConfigurator<TInstance, TData>
        where TInstance : SagaStateMachineInstance
        where TData : class
    {
        public IPipe<ConsumeContext<TData>> Discard()
        {
            return Pipe.Empty<ConsumeContext<TData>>();
        }

        public IPipe<ConsumeContext<TData>> Fault()
        {
            return Pipe.Execute<ConsumeContext<TData>>(context =>
            {
                throw new SagaException("An existing saga instance was not found", typeof(TInstance), typeof(TData), context.CorrelationId ?? Guid.Empty);
            });
        }

        public IPipe<ConsumeContext<TData>> ExecuteAsync(Func<ConsumeContext<TData>, Task> action)
        {
            return Pipe.ExecuteAsync(action);
        }

        public IPipe<ConsumeContext<TData>> Execute(Action<ConsumeContext<TData>> action)
        {
            return Pipe.Execute(action);
        }
    }
}
