// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.EndpointConfigurators
{
    using Pipeline;


    public class ReceiveEndpointBuilder :
        IReceiveEndpointBuilder
    {
        readonly IConsumePipe _inputPipe;

        public ReceiveEndpointBuilder(IConsumePipe inputPipe)
        {
            _inputPipe = inputPipe;
        }

        ConnectHandle IConsumeFilterConnector.Connect<T>(IPipe<ConsumeContext<T>> pipe)
        {
            return _inputPipe.Connect(pipe);
        }

        ConnectHandle IMessageObserverConnector.Connect<T>(IMessageObserver<T> observer)
        {
            return _inputPipe.Connect(observer);
        }

        IConsumePipe IReceiveEndpointBuilder.InputPipe
        {
            get { return _inputPipe; }
        }
    }
}