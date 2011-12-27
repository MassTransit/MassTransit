// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using Magnum.Extensions;
	using Util;

    [DebuggerDisplay("{DebuggerDisplay()}")]
	public class ServiceCatalog
	{
		readonly IDictionary<int, IList<IBusService>> _services;

		public ServiceCatalog()
		{
			_services = new Dictionary<int, IList<IBusService>>();
		}

		public IEnumerable<IBusService> Services
		{
			get { return _services.OrderBy(x => x.Key).SelectMany(x => x.Value); }
		}

        public int NumberOfServices
        {
            get { return _services.Count; }
        }

        public void Add(BusServiceLayer layer, IBusService service)
		{
			lock (_services)
			{
				IList<IBusService> serviceList;
				if (!_services.TryGetValue((int) layer, out serviceList))
				{
					serviceList = new List<IBusService>();
					_services.Add((int) layer, serviceList);
				}

				serviceList.Add(service);
			}
		}

        [UsedImplicitly]
        string DebuggerDisplay()
        {
            return "Hosting '{0}' Services".FormatWith(NumberOfServices);
        }
	}
}