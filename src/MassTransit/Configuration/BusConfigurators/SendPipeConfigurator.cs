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
namespace MassTransit.BusConfigurators
{
    using System.Collections.Generic;
    using Configurators;
    using PipeConfigurators;
    using Pipeline;


    public class SendPipeConfigurator :
        ISendPipeConfigurator,
        ISendPipeFactory
    {
        readonly SendPipeSpecificationList _sendPipeSpecification;

        public SendPipeConfigurator()
        {
            _sendPipeSpecification = new SendPipeSpecificationList();
        }

        void IPipeConfigurator<SendContext>.AddPipeSpecification(IPipeSpecification<SendContext> specification)
        {
            _sendPipeSpecification.Add(specification);
        }

        void ISendPipeConfigurator.AddPipeSpecification<T>(IPipeSpecification<SendContext<T>> specification)
        {
            _sendPipeSpecification.Add(specification);
        }

        public ISendPipeSpecification Specification => _sendPipeSpecification;

        public ISendPipe CreateSendPipe(params ISendPipeSpecification[] specifications)
        {
            var builder = new SendPipeBuilder();

            _sendPipeSpecification.Apply(builder);

            for (int i = 0; i < specifications.Length; i++)
                specifications[i].Apply(builder);

            return builder.Build();
        }

        public IEnumerable<ValidationResult> Validate()
        {
            return _sendPipeSpecification.Validate();
        }
    }
}