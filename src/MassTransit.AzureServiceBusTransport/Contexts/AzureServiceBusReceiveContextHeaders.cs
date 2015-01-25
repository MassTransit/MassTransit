// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.AzureServiceBusTransport.Contexts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Context;


    public class AzureServiceBusContextHeaderProvider :
        IContextHeaderProvider
    {
        readonly AzureServiceBusReceiveContext _context;

        public AzureServiceBusContextHeaderProvider(AzureServiceBusReceiveContext context)
        {
            _context = context;
        }

        public IEnumerable<Tuple<string, object>> Headers
        {
            get { return _context.Properties.Select(x => Tuple.Create<string, object>(x.Key, x.Value)); }
        }

        public bool TryGetHeader(string key, out object value)
        {
            if (_context.Properties == null)
            {
                value = null;
                return false;
            }

            if (_context.Properties.TryGetValue(key, out value))
                return true;

            return false;
        }
    }
}