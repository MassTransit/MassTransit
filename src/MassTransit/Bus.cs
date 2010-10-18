// Copyright 2007-2010 The Apache Software Foundation.
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
namespace MassTransit
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Configuration;

    [Obsolete("Don't use yet")]
    public static class Bus
    {
        public static void Initialize(Action<IEndpointFactoryConfigurator> cfg, Action<IServiceBusConfigurator> action)
        {
            cfg = c =>
            {
                //add what we found in the dir
                cfg(c);
            };

            //endpoints
            Endpoints = EndpointFactoryConfigurator.New(cfg);

            //get the factory rocking
            Instance = ServiceBusConfigurator.New(action);
        }

        public static IServiceBus Instance { get; private set; }
        public static IEndpointFactory Endpoints { get; private set; }

        private static void FindTransports()
        {
            string assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var files = Directory.GetFiles(assemblyPath);
            var transportFiles = files.Where(f=> f.StartsWith("MassTransit.Transport."));

        }
    }

    //Use this so that we can track down where ever we are using factory stuff?
    //also this should make more sense that Func<Type, object> ?
    //
    public delegate object BuilderFunc(Type typeToBuild);

}