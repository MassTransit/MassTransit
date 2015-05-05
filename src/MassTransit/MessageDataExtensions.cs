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
namespace MassTransit
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using MessageData;


    public static class MessageDataExtensions
    {
        public static async Task<MessageData<string>> PutString(this IMessageDataRepository repository, string value,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (repository == null)
                throw new ArgumentNullException("repository");
            if (value == null)
                return new EmptyMessageData<string>();

            byte[] bytes = Encoding.UTF8.GetBytes(value);
            using (var ms = new MemoryStream(bytes, false))
            {
                Uri address = await repository.Put(ms, default(TimeSpan?), cancellationToken);

                return new ConstantMessageData<string>(address, value);
            }
        }

        public static async Task<MessageData<string>> PutString(this IMessageDataRepository repository, string value, TimeSpan timeToLive,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (repository == null)
                throw new ArgumentNullException("repository");
            if (value == null)
                return new EmptyMessageData<string>();

            byte[] bytes = Encoding.UTF8.GetBytes(value);
            using (var ms = new MemoryStream(bytes, false))
            {
                Uri address = await repository.Put(ms, timeToLive, cancellationToken);

                return new ConstantMessageData<string>(address, value);
            }
        }

        public static async Task<MessageData<string>> GetString(this IMessageDataRepository repository, Uri address,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            using (var ms = new MemoryStream())
            {
                Stream stream = await repository.Get(address, cancellationToken);
                await stream.CopyToAsync(ms);

                return new ConstantMessageData<string>(address, Encoding.UTF8.GetString(ms.ToArray()));
            }
        }
    }
}