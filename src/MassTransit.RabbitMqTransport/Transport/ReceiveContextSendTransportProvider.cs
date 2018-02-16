// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.RabbitMqTransport.Transport
{
    using Configuration;
    using EndpointSpecifications;
    using GreenPipes;
    using GreenPipes.Agents;


    public class ReceiveContextSendTransportProvider :
        SendTransportProvider
    {
        readonly ModelContext _modelContext;
        readonly ReceiveContext _receiveContext;

        public ReceiveContextSendTransportProvider(IRabbitMqBusConfiguration busConfiguration, ReceiveContext receiveContext)
            : base(busConfiguration)
        {
            _receiveContext = receiveContext;

            _modelContext = receiveContext.GetPayload<ModelContext>();
        }

        protected override IAgent<ModelContext> GetModelSource(IRabbitMqHostControl rabbitMqHostControl)
        {
            return new ReceiveModelSource(_modelContext, _receiveContext);
        }
    }
}