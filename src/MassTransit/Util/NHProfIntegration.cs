namespace MassTransit.Util
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public class NHProfIntegration
    {
        private static readonly PropertyInfo currentContext;
        private static readonly Dictionary<Type, Func<object, string>> type2ToString = new Dictionary<Type, Func<object, string>>();


        static NHProfIntegration()
        {
            try
            {
                var profilerAsm = Assembly.Load("HibernatingRhinos.Profiler.Appender");
                if (profilerAsm == null)
                    return;
                var profilerIntegration = profilerAsm.GetType("HibernatingRhinos.Profiler.Appender.ProfilerIntegration");
                if (profilerIntegration == null)
                    return;
                currentContext = profilerIntegration.GetProperty("CurrentSessionContext");
            }
            catch
            {
                // we ignore if not found / failed
            }
        }

        public static Action Track(object currentMessageInformation)
        {
//            if (currentContext == null)
//                return null;
//            if (currentMessageInformation == null)
//                return null;
//
//            var str = string.Join(", ", currentMessageInformation.AllMessages.Select(o => ToString(o)).ToArray());
//            currentContext.SetValue(null, str, null);
//            return () => currentContext.SetValue(null, null, null);
            return null;
        }

        private static string ToString(object msg)
        {
            //Func<object, string> val = null;
            //var type = msg.GetType();
            //type2ToString.Read(reader => reader.TryGetValue(type, out val));
            //if (val != null)
            //    return val(msg);

            //type2ToString.Write(writer =>
            //{
            //    var overrideToString = type.GetMethod("ToString").DeclaringType != typeof(object);
            //    if (overrideToString)
            //        val = o => o.ToString();
            //    else
            //        val = o => type.Name;
            //    writer.Add(type, val);
            //});

            //return val(msg);
            return "";
        }


    }
}