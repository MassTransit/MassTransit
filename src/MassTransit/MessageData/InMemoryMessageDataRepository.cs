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
    using System.Collections.Concurrent;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;


    public class InMemoryMessageDataRepository :
        IMessageDataRepository
    {
        readonly ConcurrentDictionary<Uri, byte[]> _values;

        public InMemoryMessageDataRepository()
        {
            _values = new ConcurrentDictionary<Uri, byte[]>();
        }

        async Task<Stream> IMessageDataRepository.Get(Uri address, CancellationToken cancellationToken)
        {
            if (address == null)
                throw new ArgumentNullException("address");

            byte[] value;
            if (_values.TryGetValue(address, out value))
                return new MemoryStream(value, false);

            throw new MessageDataNotFoundException(address);
        }

        async Task<Uri> IMessageDataRepository.Put(Stream stream, CancellationToken cancellationToken)
        {
            Uri address = new InMemoryMessageDataId().Uri;

            using (var ms = new MemoryStream())
            {
                await stream.CopyToAsync(ms);

                _values.TryAdd(address, ms.ToArray());
            }

            return address;
        }
    }
}