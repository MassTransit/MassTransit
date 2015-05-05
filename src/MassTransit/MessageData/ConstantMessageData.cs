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


    public class ConstantMessageData<T> :
        MessageData<T>
    {
        readonly Uri _address;
        readonly Task<T> _value;

        public ConstantMessageData(Uri address, T value)
        {
            _address = address;
            _value = Task.FromResult(value);
        }

        public Uri Address
        {
            get { return _address; }
        }

        public bool HasValue
        {
            get { return true; }
        }

        public Task<T> Value
        {
            get { return _value; }
        }
    }
}