namespace MassTransit.ServiceBus.Formatters
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	using log4net;

    public class MessageFinder
	{
        private static ILog _log = LogManager.GetLogger(typeof (MessageFinder));
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
							{
                                if (_log.IsDebugEnabled)
                                    _log.DebugFormat("Ignoring Message {0} it is either Abstract or an Interface", type);

                                continue;
							}

							if (type.ContainsGenericParameters)
							{
                                if(_log.IsDebugEnabled)
                                    _log.DebugFormat("Ignoring Message {0} it contains generic parameters", type);
                                
                                continue;
							}
								

							if (messageType.IsAssignableFrom(type))
							{
                                _messageTypes.Add(type);
                                if(_log.IsInfoEnabled)
                                    _log.InfoFormat("Found Message {0}", type);
							}
								
						}
					}
					catch (Exception ex)
					{
						// NOTE: if we have a problem, we just ignore that assembly
                        _log.ErrorFormat("Encountered a problem loading messages in assembly {0}. Exception was {1}", assembly.FullName, ex.Message);
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