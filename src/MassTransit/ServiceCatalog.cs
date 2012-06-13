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
namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Magnum.Caching;
    using Magnum.Extensions;
    using Util;

    [DebuggerDisplay("{DebuggerDisplay()}")]
    public class ServiceCatalog
    {
        readonly Cache<int, IList<IBusService>> _services;

        public ServiceCatalog()
        {
            _services = new ConcurrentCache<int, IList<IBusService>>(x => new List<IBusService>());
        }

        public IEnumerable<IBusService> Services
        {
            get { return _services.GetAllKeys().OrderBy(x => x).SelectMany(x => _services[x]); }
        }

        public int NumberOfServices
        {
            get { return _services.Count; }
        }

        public void Add(BusServiceLayer layer, IBusService service)
        {
            _services[(int)layer].Add(service);
        }

        public IBusService Get(Type type)
        {
            return _services.SelectMany(x => x).First(type.IsInstanceOfType);
        }

        public bool TryGet(Type type, out IBusService result)
        {
            result = _services.SelectMany(x => x).FirstOrDefault(type.IsInstanceOfType);
            return result != null;
        }

        [UsedImplicitly]
        string DebuggerDisplay()
        {
            return "Hosting '{0}' Services".FormatWith(NumberOfServices);
        }
    }
}