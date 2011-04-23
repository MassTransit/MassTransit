namespace MassTransit.StructureMapIntegration
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using Configuration;
    using Magnum.Extensions;
    using StructureMap;
    using StructureMap.Query;

    public static class StructureMapBusExtensions
    {
        public static void LoadConsumersFromContainer(this BusConfiguration cfg, IContainer container)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
                
            
             var i = container.Model.AllInstances;
            var c = new List<InstanceRef>();

            foreach (var instanceRef in i)
            {
                var pt = instanceRef.PluginType;
                if(pt.Implements(typeof(IConsumer)))
                    c.Add(instanceRef);
            }

            //do something with the instance ref

            stopwatch.Stop();
            //loading took ---
        }
    }
}