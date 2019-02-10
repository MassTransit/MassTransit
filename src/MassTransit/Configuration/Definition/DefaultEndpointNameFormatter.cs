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
namespace MassTransit.Definition
{
    using System;
    using Courier;
    using Saga;


    /// <summary>
    /// The default endpoint name formatter, which simply trims the words Consumer, Activity, and Saga
    /// from the type name. If you need something more readable, consider the <see cref="SnakeCaseEndpointNameFormatter"/>
    /// or the <see cref="KebabCaseEndpointNameFormatter"/>.
    /// </summary>
    public class DefaultEndpointNameFormatter :
        IEndpointNameFormatter
    {
        public string Consumer<T>()
            where T : class, IConsumer
        {
            return GetConsumerName(typeof(T).Name);
        }

        public string Saga<T>()
            where T : class, ISaga
        {
            return GetSagaName(typeof(T).Name);
        }

        public string ExecuteActivity<T, TArguments>()
            where T : class, ExecuteActivity<TArguments>
            where TArguments : class
        {
            var activityName = GetActivityName(typeof(T).Name);

            return $"{activityName}_execute";
        }

        public string CompensateActivity<T, TLog>()
            where T : class, CompensateActivity<TLog>
            where TLog : class
        {
            var activityName = GetActivityName(typeof(T).Name);

            return $"{activityName}_compensate";
        }

        string GetConsumerName(string typeName)
        {
            const string consumer = "Consumer";

            string consumerName = typeName;
            if (consumerName.EndsWith(consumer, StringComparison.InvariantCultureIgnoreCase))
                consumerName = consumerName.Substring(0, consumerName.Length - consumer.Length);

            return SanitizeName(consumerName);
        }

        string GetSagaName(string typeName)
        {
            const string saga = "Saga";

            string sagaName = typeName;
            if (sagaName.EndsWith(saga, StringComparison.InvariantCultureIgnoreCase))
                sagaName = sagaName.Substring(0, sagaName.Length - saga.Length);

            return SanitizeName(sagaName);
        }

        string GetActivityName(string typeName)
        {
            const string activity = "Activity";

            string activityName = typeName;
            if (activityName.EndsWith(activity, StringComparison.InvariantCultureIgnoreCase))
                activityName = activityName.Substring(0, activityName.Length - activity.Length);

            return SanitizeName(activityName);
        }

        protected virtual string SanitizeName(string name)
        {
            return name;
        }
    }
}
