namespace MassTransit.ServiceBus.Formatters
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;

	public class MessageFinder
	{
		private static readonly List<Type> _messageTypes = new List<Type>();

		public static void Initialize()
		{
			lock (_messageTypes)
			{
				_messageTypes.Clear();

				Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
				Type messageType = typeof (IMessage);

				foreach (Assembly assembly in assemblies)
				{
					try
					{
						Type[] types = assembly.GetTypes();

						foreach (Type type in types)
						{
							if (type.IsAbstract || type.IsInterface)
								continue;

							if (type.ContainsGenericParameters)
								continue;

							if (messageType.IsAssignableFrom(type))
								_messageTypes.Add(type);
						}
					}
					catch (Exception ex)
					{
						// NOTE if we have a problem, we just ignore that assembly
					}
				}
			}
		}

		public static List<Type> AllMessageTypes()
		{
			if (_messageTypes.Count == 0)
				Initialize();

			return _messageTypes;
		}
	}
}