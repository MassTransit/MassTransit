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
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;


    public class ReadStringMessageData :
        MessageData<string>
    {
        readonly Uri _address;
        readonly CancellationToken _cancellationToken;
        readonly bool _hasValue;
        readonly IMessageDataRepository _storage;
        readonly Lazy<Task<string>> _value;

        public ReadStringMessageData(Uri address, IMessageDataRepository storage, CancellationToken cancellationToken)
        {
            _address = address;
            _storage = storage;
            _cancellationToken = cancellationToken;
            _hasValue = true;
            _value = new Lazy<Task<string>>(GetValue);
        }

        public ReadStringMessageData(CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
            _hasValue = false;
        }

        public Uri Address
        {
            get { return _address; }
        }

        public bool HasValue
        {
            get { return _hasValue; }
        }

        public Task<string> Value
        {
            get
            {
                if (_hasValue == false)
                    throw new BigPropertyException("The property {0} has no value.");

                return _value.Value;
            }
        }

        async Task<string> GetValue()
        {
            Stream valueStream = await _storage.Get(_address, _cancellationToken);
            try
            {
                using (var ms = new MemoryStream())
                {
                    await valueStream.CopyToAsync(ms, 16384, _cancellationToken);

                    return Encoding.UTF8.GetString(ms.ToArray());
                }
            }
            finally
            {
                valueStream.Dispose();
            }
        }
    }
}