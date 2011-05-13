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
namespace MassTransit.Monitoring
{
	using System.Diagnostics;

	public class RuntimePerformanceCounter
	{
		readonly PerformanceCounterType _counterType;
		readonly string _help;
		readonly string _name;

		public RuntimePerformanceCounter(string name, string help, PerformanceCounterType counterType)
		{
			_name = name;
			_help = help;
			_counterType = counterType;
		}

		public string Name
		{
			get { return _name; }
		}

		public void AddCounterToCollection(CounterCreationDataCollection counterData)
		{
			var counterCreationData = new CounterCreationData(
				_name,
				_help,
				_counterType);

			counterData.Add(counterCreationData);
		}

		public static implicit operator CounterCreationData(RuntimePerformanceCounter counter)
		{
			return new CounterCreationData(counter._name, counter._help, counter._counterType);
		}
	}
}