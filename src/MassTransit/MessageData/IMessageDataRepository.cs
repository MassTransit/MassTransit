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
namespace MassTransit.MessageData
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;


    /// <summary>
    /// Storage of large message data that can be stored and retrieved separate of the message body. 
    /// Implemented as a claim-check pattern when an identifier is stored in the message body which
    /// is used to retrieve the message data separately.
    /// </summary>
    public interface IMessageDataRepository
    {
        /// <summary>
        /// Returns a stream to read the message data for the specified address.
        /// </summary>
        /// <param name="address">The data address</param>
        /// <param name="cancellationToken">A cancellation token for the request</param>
        /// <returns></returns>
        Task<Stream> Get(Uri address, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Puts message data into the repository
        /// </summary>
        /// <param name="stream">The stream of data for the message</param>
        /// <param name="timeToLive"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<Uri> Put(Stream stream, TimeSpan? timeToLive = default(TimeSpan?), CancellationToken cancellationToken = default(CancellationToken));
    }
}