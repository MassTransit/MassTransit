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
namespace MassTransit.Registration
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using Courier;
    using Internals.Extensions;
    using Util;


    public static class ActivityRegistrationCache
    {
        static CachedRegistration GetOrAdd(Type activityType)
        {
            if (!activityType.HasInterface(typeof(Activity<,>)))
                throw new ArgumentException($"The type is not an activity: {TypeMetadataCache.GetShortName(activityType)}", nameof(activityType));

            var argumentLogTypes = activityType.GetClosingArguments(typeof(Activity<,>)).ToArray();
            var genericType = typeof(CachedRegistration<,,>).MakeGenericType(activityType, argumentLogTypes[0], argumentLogTypes[1]);

            return Cached.Instance.GetOrAdd(activityType, _ => (CachedRegistration)Activator.CreateInstance(genericType));
        }

        public static void Register(Type activityType, IContainerRegistrar registrar)
        {
            GetOrAdd(activityType).Register(registrar);
        }

        public static IActivityRegistration CreateRegistration(Type activityType, Type activityDefinitionType, IContainerRegistrar registrar)
        {
            return GetOrAdd(activityType).CreateRegistration(activityDefinitionType, registrar);
        }


        static class Cached
        {
            internal static readonly ConcurrentDictionary<Type, CachedRegistration> Instance = new ConcurrentDictionary<Type, CachedRegistration>();
        }


        interface CachedRegistration
        {
            void Register(IContainerRegistrar registrar);
            IActivityRegistration CreateRegistration(Type activityDefinitionType, IContainerRegistrar registrar);
        }


        class CachedRegistration<TActivity, TArguments, TLog> :
            CachedRegistration
            where TActivity : class, Activity<TArguments, TLog>
            where TArguments : class
            where TLog : class
        {
            public void Register(IContainerRegistrar registrar)
            {
                registrar.RegisterExecuteActivity<TActivity, TArguments>();
                registrar.RegisterCompensateActivity<TActivity, TLog>();
            }

            public IActivityRegistration CreateRegistration(Type activityDefinitionType, IContainerRegistrar registrar)
            {
                Register(registrar);

                if (activityDefinitionType != null)
                    ActivityDefinitionRegistrationCache.Register(activityDefinitionType, registrar);

                return new ActivityRegistration<TActivity, TArguments, TLog>();
            }
        }
    }
}
