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
namespace MassTransit.Builders
{
    using Configuration;
    using GreenPipes;


    public abstract class ReceiveEndpointBuilder
    {
        readonly IReceiveEndpointConfiguration _configuration;

        protected ReceiveEndpointBuilder(IReceiveEndpointConfiguration configuration)
        {
            _configuration = configuration;
        }

        public virtual ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe)
            where T : class
        {
            IPipe<ConsumeContext<T>> messagePipe = _configuration.Consume.Specification.GetMessageSpecification<T>().BuildMessagePipe(pipe);

            return _configuration.ConsumePipe.ConnectConsumePipe(messagePipe);
        }

        public ConnectHandle ConnectConsumeMessageObserver<T>(IConsumeMessageObserver<T> observer)
            where T : class
        {
            return _configuration.ConsumePipe.ConnectConsumeMessageObserver(observer);
        }

        public ConnectHandle ConnectReceiveEndpointObserver(IReceiveEndpointObserver observer)
        {
            return _configuration.EndpointObservers.Connect(observer);
        }
    }
}