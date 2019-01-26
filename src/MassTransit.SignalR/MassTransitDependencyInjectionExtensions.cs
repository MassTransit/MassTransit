// Copyright 2007-2019 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.SignalR
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.Extensions.DependencyInjection;


    public static class MassTransitDependencyInjectionExtensions
    {
        public static void AddMassTransitBackplane(this IServiceCollection serviceCollection, out IReadOnlyDictionary<Type, IReadOnlyList<Type>> hubConsumers,
            params Assembly[] hubAssemblies)
        {
            var dict = new Dictionary<Type, IReadOnlyList<Type>>();

            IEnumerable<Type> hubs = from a in hubAssemblies
                from t in a.GetTypes()
                where typeof(Hub).IsAssignableFrom(t)
                select t;

            Type[] consumers = (from t in typeof(MassTransitDependencyInjectionExtensions).Assembly.GetTypes()
                where typeof(IConsumer).IsAssignableFrom(t)
                select t).ToArray();

            foreach (var hub in hubs)
            {
                var consumerTypes = new List<Type>(consumers.Length);

                foreach (var consumer in consumers)
                {
                    var consumerType = consumer.MakeGenericType(hub);
                    consumerTypes.Add(consumerType);
                    serviceCollection.AddScoped(consumerType);
                }

                dict.Add(hub, consumerTypes);
            }

            hubConsumers = dict;

            serviceCollection.AddSingleton(typeof(HubLifetimeManager<>), typeof(MassTransitHubLifetimeManager<>));
        }
    }
}
