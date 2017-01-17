// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.PipeConfigurators
{
    using System.Collections.Generic;
    using GreenPipes;
    using Pipeline.Filters;


    /// <summary>
    /// Adds a message handler to the consuming pipe builder
    /// </summary>
    /// <typeparam name="T">The message type</typeparam>
    public class HandlerPipeSpecification<T> :
        IPipeSpecification<ConsumeContext<T>>
        where T : class
    {
        readonly MessageHandler<T> _handler;

        public HandlerPipeSpecification(MessageHandler<T> handler)
        {
            _handler = handler;
        }

        void IPipeSpecification<ConsumeContext<T>>.Apply(IPipeBuilder<ConsumeContext<T>> builder)
        {
            builder.AddFilter(new HandlerMessageFilter<T>(_handler));
        }

        IEnumerable<ValidationResult> ISpecification.Validate()
        {
            if (_handler == null)
                yield return this.Failure("Handler", "must not be null");
        }
    }
}