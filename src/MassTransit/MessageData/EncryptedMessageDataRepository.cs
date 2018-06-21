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
    using System.Collections.Specialized;
    using System.IO;
    using System.Security.Cryptography;
    using System.Threading;
    using System.Threading.Tasks;
    using Internals.Extensions;
    using Serialization;


    public class EncryptedMessageDataRepository :
        IMessageDataRepository
    {
        readonly IMessageDataRepository _repository;
        readonly ICryptoStreamProvider _streamProvider;

        /// <summary>
        /// Provides encrypted stream support to ensure that message data is encrypted at rest.
        /// </summary>
        /// <param name="repository">The original message data repository where message data is stored.</param>
        /// <param name="streamProvider">The encrypted stream provider</param>
        public EncryptedMessageDataRepository(IMessageDataRepository repository, ICryptoStreamProvider streamProvider)
        {
            _repository = repository;
            _streamProvider = streamProvider;
        }

        public async Task<Stream> Get(Uri address, CancellationToken cancellationToken = new CancellationToken())
        {
            var stream = await _repository.Get(address, cancellationToken).ConfigureAwait(false);

            string keyId;
            address.TryGetValueFromQueryString("keyId", out keyId);

            return _streamProvider.GetDecryptStream(stream, keyId, CryptoStreamMode.Read);
        }

        public async Task<Uri> Put(Stream stream, TimeSpan? timeToLive = null, CancellationToken cancellationToken = new CancellationToken())
        {
            string keyId = null;
            using (Stream cryptoStream = _streamProvider.GetEncryptStream(stream, keyId, CryptoStreamMode.Read))
            {
                var address = await _repository.Put(cryptoStream, timeToLive, cancellationToken).ConfigureAwait(false);

                var addressBuilder = new UriBuilder(address);

                NameValueCollection parameters = new NameValueCollection();
                if (!string.IsNullOrWhiteSpace(addressBuilder.Query))
                {
                    var query = addressBuilder.Query;

                    if (query.Contains("?"))
                    {
                        query = query.Substring(query.IndexOf('?') + 1);
                    }

                    foreach (string parameter in query.Split('&'))
                    {
                        if (string.IsNullOrWhiteSpace(parameter))
                            continue;

                        string[] pair = parameter.Split('=');

                        parameters.Add(pair[0], pair.Length == 2 ? pair[1] : "");
                    }
                }

                parameters["keyId"] = "";
                addressBuilder.Query = parameters.ToString();

                return addressBuilder.Uri;
            }
        }
    }
}