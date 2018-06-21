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
    using System.Threading.Tasks;
    using Transformation;


    /// <summary>
    /// Loads the message data when accessed via Value, using the specified repository and converter.
    /// </summary>
    /// <typeparam name="T">The message data property type</typeparam>
    public class LoadMessageData<T> :
        MessageData<T>
    {
        readonly Uri _address;
        readonly IMessageDataConverter<T> _converter;
        readonly IMessageDataRepository _repository;
        readonly TransformContext _transformContext;
        readonly Lazy<Task<T>> _value;

        public LoadMessageData(Uri address, IMessageDataRepository repository, IMessageDataConverter<T> converter, TransformContext transformContext)
        {
            _address = address;
            _repository = repository;
            _converter = converter;
            _transformContext = transformContext;

            _value = new Lazy<Task<T>>(GetValue);
        }

        public Uri Address => _address;

        public bool HasValue => true;

        public Task<T> Value => _value.Value;

        async Task<T> GetValue()
        {
            using (Stream valueStream = await _repository.Get(_address, _transformContext.CancellationToken).ConfigureAwait(false))
            {
                return await _converter.Convert(valueStream, _transformContext.CancellationToken).ConfigureAwait(false);
            }
        }
    }
}