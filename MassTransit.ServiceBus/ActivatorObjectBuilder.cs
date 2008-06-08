/// Copyright 2007-2008 The Apache Software Foundation.
/// 
/// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
/// this file except in compliance with the License. You may obtain a copy of the 
/// License at 
/// 
///   http://www.apache.org/licenses/LICENSE-2.0 
/// 
/// Unless required by applicable law or agreed to in writing, software distributed 
/// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
/// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
/// specific language governing permissions and limitations under the License.
namespace MassTransit.ServiceBus
{
	using System;
	using System.Collections;

    public class ActivatorObjectBuilder : IObjectBuilder
	{
		public object Build(Type objectType)
		{
			return Activator.CreateInstance(objectType);
		}

		public T Build<T>() where T : class
		{
			return Build(typeof(T)) as T;
		}


        public T Build<T>(Type component) where T : class
        {
            return Build(component) as T;
        }

        public T Build<T>(IDictionary arguments)
        {
            ArrayList args = new ArrayList();
            foreach (DictionaryEntry entry in arguments)
            {
                args.Add(entry.Value);
            }
            return (T)Activator.CreateInstance(typeof(T), args.ToArray());
        }

        public void Release<T>(T obj)
		{
			IDisposable disposal = obj as IDisposable;
			if (disposal != null)
				disposal.Dispose();
		}
	}
}