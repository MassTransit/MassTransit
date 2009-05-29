// Copyright 2007-2008 The Apache Software Foundation.
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
namespace MassTransit.StructureMapIntegration
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using StructureMap;
	using StructureMap.Pipeline;

	public class StructureMapObjectBuilder :
		IObjectBuilder
	{
		private readonly IContainer _container;

		public StructureMapObjectBuilder()
			: this(ObjectFactory.GetInstance<IContainer>())
		{
		}

		public StructureMapObjectBuilder(IContainer container)
		{
			_container = container;
		}

		public T GetInstance<T>(IDictionary arguments)
		{
			IDictionary<string, object> bob = new Dictionary<string, object>();
			foreach (DictionaryEntry entry in arguments)
			{
				bob.Add(entry.Key.ToString(), entry.Value);
			}
			var args = new ExplicitArguments(bob);
			return _container.GetInstance<T>(args); //how to handle component
		}

		public void Release<T>(T obj)
		{
			//structure map doesn't have this concept.
		}

		public object GetService(Type serviceType)
		{
			return _container.GetInstance(serviceType);
		}

		public object GetInstance(Type serviceType)
		{
			return _container.GetInstance(serviceType);
		}

		public object GetInstance(Type serviceType, string key)
		{
			return _container.GetInstance(serviceType, key);
		}

		public IEnumerable<object> GetAllInstances(Type serviceType)
		{
			var result = new ArrayList(_container.GetAllInstances(serviceType));
			return result.ToArray();
		}

		public TService GetInstance<TService>()
		{
			return _container.GetInstance<TService>();
		}

		public TService GetInstance<TService>(string key)
		{
			return _container.GetInstance<TService>(key);
		}

		public IEnumerable<TService> GetAllInstances<TService>()
		{
			return _container.GetAllInstances<TService>();
		}
	}
}