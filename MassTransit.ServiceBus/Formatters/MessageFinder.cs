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
                    if (DoesAssemblyReferenceMassTransit(assembly))
                    {
                        Type[] types = assembly.GetTypes();

                        foreach (Type type in types)
                        {
                            if (type.IsAbstract || type.IsInterface)
                                continue;

                            if (messageType.IsAssignableFrom(type))
                                _messageTypes.Add(type);
                        }
                    }
                }
            }
        }

        public static bool DoesAssemblyReferenceMassTransit(Assembly assembly)
        {
            bool result = false;
            AssemblyName[] names = assembly.GetReferencedAssemblies();

            foreach (AssemblyName name in names)
            {
                if(name.Name.Contains("MassTransit"))
                {
                    result = true;
                    break;
                }
            }

            return result;
        }
        public static List<Type> AllMessageTypes()
        {
            Initialize();
            return _messageTypes;
        }
    }
}