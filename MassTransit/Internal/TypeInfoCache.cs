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
namespace MassTransit.Internal
{
	using System;
	using System.Collections.Generic;
	using Magnum.Common.Threading;

	public interface ITypeInfoCache : IDisposable
	{
		ITypeInfo Resolve<TComponent>();
		ITypeInfo Resolve(Type componentType);
		IPublicationTypeInfo GetPublicationTypeInfo<TComponent>() where TComponent : class;
		IPublicationTypeInfo GetPublicationTypeInfo(Type type);
	}

	public class TypeInfoCache : ITypeInfoCache
	{
		private static readonly IList<ActionEntry> _consumerActions = new List<ActionEntry>();

		private static readonly Type _correlatedMessageType = typeof (CorrelatedBy<>);
		private readonly ReaderWriterLockedDictionary<Type, TypeInfo> _types = new ReaderWriterLockedDictionary<Type, TypeInfo>();

		static TypeInfoCache()
		{
			_consumerActions.Add(new ActionEntry(_correlatedMessageType, (i, c, t) => i.SetPublicationType(c, t)));
		}

		public void Dispose()
		{
			foreach (ITypeInfo info in _types.Values)
			{
				info.Dispose();
			}
			_types.Clear();
		}

		public ITypeInfo Resolve<TComponent>()
		{
			return Resolve(typeof (TComponent));
		}

		public ITypeInfo Resolve(Type componentType)
		{
			return _types.Retrieve(componentType, () =>
				{
					TypeInfo info = new TypeInfo(componentType);

					List<Type> usedMessageTypes = new List<Type>();

					foreach (Type interfaceType in componentType.GetInterfaces())
					{
						if (!interfaceType.IsGenericType) continue;

						Type genericType = interfaceType.GetGenericTypeDefinition();
						Type[] types = interfaceType.GetGenericArguments();

						if (usedMessageTypes.Contains(types[0])) continue;

						foreach (ActionEntry entry in _consumerActions)
						{
							if (entry.GenericType != genericType) continue;

							usedMessageTypes.Add(types[0]);

							entry.AddAction(info, componentType, types);
							break;
						}
					}

					return info;
				});
		}

		public IPublicationTypeInfo GetPublicationTypeInfo<TComponent>() where TComponent : class
		{
			ITypeInfo typeInfo = Resolve<TComponent>();

			return typeInfo.GetPublicationTypeInfo();
		}

		public IPublicationTypeInfo GetPublicationTypeInfo(Type type)
		{
			ITypeInfo typeInfo = Resolve(type);

			return typeInfo.GetPublicationTypeInfo();
		}

		public class ActionEntry
		{
			private readonly Action<TypeInfo, Type, Type[]> _addAction;
			private readonly Type _genericType;

			public ActionEntry(Type genericType, Action<TypeInfo, Type, Type[]> addAction)
			{
				_genericType = genericType;
				_addAction = addAction;
			}

			public Type GenericType
			{
				get { return _genericType; }
			}

			public Action<TypeInfo, Type, Type[]> AddAction
			{
				get { return _addAction; }
			}
		}
	}

	public enum SubscriptionMode
	{
		All,
		Selected,
		Correlated,
	}
}