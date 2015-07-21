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
    using System.Threading.Tasks;


    public class DeserializedMessageData<T> :
        MessageData<T>
    {
        readonly Uri _address;
        readonly bool _hasValue;

        public DeserializedMessageData(Uri address)
        {
            _address = address;
            _hasValue = true;
        }

        public Uri Address => _address;

        public bool HasValue => _hasValue;

        public Task<T> Value
        {
            get
            {
                if (_hasValue == false)
                    throw new MessageDataException("The message data has no value");

                throw new MessageDataException("The message data was not loaded: " + _address);
            }
        }
    }
}