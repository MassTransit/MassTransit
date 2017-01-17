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
namespace MassTransit.SendPipeSpecifications
{
    using GreenPipes;
    using PipeBuilders;


    public interface IMessageSendPipeSpecification<TMessage> :
        IMessageSendPipeConfigurator<TMessage>,
        ISpecificationPipeSpecification<SendContext<TMessage>>
        where TMessage : class
    {
        void AddParentMessageSpecification(ISpecificationPipeSpecification<SendContext<TMessage>> parentSpecification);

        /// <summary>
        /// Build the pipe for the specification
        /// </summary>
        /// <returns></returns>
        IPipe<SendContext<TMessage>> BuildMessagePipe();
    }


    public interface IMessageSendPipeSpecification :
        IPipeConfigurator<SendContext>,
        ISpecification
    {
        IMessageSendPipeSpecification<T> GetMessageSpecification<T>()
            where T : class;

        /// <summary>
        /// Connect the pipe for the specfication to the specified connector
        /// </summary>
        /// <param name="connector"></param>
        ConnectHandle Connect(IPipeConnector connector);
    }
}