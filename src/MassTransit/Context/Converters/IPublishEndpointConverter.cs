// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Context.Converters
{
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;


    /// <summary>
    /// Calls the generic version of the IPublishEndpoint.Send method with the object's type
    /// </summary>
    public interface IPublishEndpointConverter
    {
        Task Publish(IPublishEndpoint endpoint, object message, CancellationToken cancellationToken = default(CancellationToken));

        Task Publish(IPublishEndpoint endpoint, object message, IPipe<PublishContext> pipe,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}