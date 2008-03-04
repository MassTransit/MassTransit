namespace MassTransit.ServiceBus.Formatters
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;

    public class MessageFinder
    {
        private static List<Type> _results = new List<Type>();

        public static List<Type> FindAll()
        {
            
            string loc = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            string[] files = Directory.GetFiles(loc, "*.dll");

            foreach (string file in files)
            {
                Assembly asm;
                if (!file.Contains("NHibernate")) //TODO: Hack!
                {
                    asm = Assembly.LoadFile(file);
                    if (ReferencesMassTransit(asm))
                    {
                        Type[] t = asm.GetTypes();

                        foreach (Type tm in t)
                        {
                            if (IsMessage(tm))
                            {
                                _results.Add(tm);
                            }
                        }
                    }
                }
            }

            return _results;
        }

        public static bool IsMessage(Type t)
        {
            bool result = false;

            if(!t.IsInterface && !t.IsGenericType && t.GetInterface(typeof(IMessage).FullName) != null)
            {
                result = true;
            }

            return result;
        }

        public static bool ReferencesMassTransit(Assembly assembly)
        {
            bool result = false;
            AssemblyName[] names = assembly.GetReferencedAssemblies();

            foreach (AssemblyName name in names)
            {
                if(name.Name.Equals("MassTransit.ServiceBus"))
                {
                    result = true;
                    break;
                }
            }

            return result;
        }
    }
}