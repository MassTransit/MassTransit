// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Context
{
    using System;
    using Magnum.Caching;

    public class EndpointObjectSenderCache
    {
        static EndpointObjectSenderCache _instance;

        readonly Cache<Type, EndpointObjectSender> _typeCache =
            new GenericTypeCache<EndpointObjectSender>(typeof(EndpointObjectSenderImpl<>));

        public static EndpointObjectSenderCache Instance
        {
            get { return _instance ?? (_instance = new EndpointObjectSenderCache()); }
        }

        public EndpointObjectSender this[Type type]
        {
            get { return _typeCache[type]; }
        }
    }
}