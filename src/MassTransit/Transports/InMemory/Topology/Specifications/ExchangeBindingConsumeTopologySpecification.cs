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
namespace MassTransit.Transports.InMemory.Topology.Specifications
{
    using System.Collections.Generic;
    using Builders;
    using GreenPipes;


    /// <summary>
    /// Used to bind an exchange to the consuming queue's exchange
    /// </summary>
    public class ExchangeBindingConsumeTopologySpecification :
        IInMemoryConsumeTopologySpecification
    {
        readonly string _exchange;

        public ExchangeBindingConsumeTopologySpecification(string exchange)
        {
            _exchange = exchange;
        }

        public IEnumerable<ValidationResult> Validate()
        {
            yield break;
        }

        public void Apply(IInMemoryConsumeTopologyBuilder builder)
        {
            builder.ExchangeBind(_exchange, builder.Exchange);
        }
    }
}