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
namespace MassTransit.Transports.RabbitMq.Pipeline
{
    using System.Threading;
    using System.Threading.Tasks;
    using MassTransit.Pipeline;


    public interface IRabbitMqConnector
    {
        /// <summary>
        /// Connects to RabbitMQ, invoking the pipe when the connection is established
        /// </summary>
        /// <param name="pipe">The connection context pipe</param>
        /// <param name="cancellationToken">The token that will be cancelled when the connection should no longer be used</param>
        /// <returns></returns>
        Task Connect(IPipe<ConnectionContext> pipe, CancellationToken cancellationToken);
    }
}