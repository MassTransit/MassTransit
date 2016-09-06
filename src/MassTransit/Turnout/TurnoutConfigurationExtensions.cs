// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Threading.Tasks;
    using Turnout;
    using Turnout.Configuration;


    public static class TurnoutConfigurationExtensions
    {
        /// <summary>
        /// Configures a Turnout on the receive endpoint, which executes a long-running job and supervises the job until it
        /// completes.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator">The receive endpoint configurator</param>
        /// <param name="busFactoryConfigurator">The bus factory configuration to use a separate endpoint for the control traffic</param>
        /// <param name="configure"></param>
        public static void Turnout<T>(this IReceiveEndpointConfigurator configurator, IInMemoryBusFactoryConfigurator busFactoryConfigurator,
            Action<ITurnoutHostConfigurator<T>> configure)
            where T : class
        {
            var temporaryQueueName = busFactoryConfigurator.GetTemporaryQueueName($"turnout-");

            busFactoryConfigurator.ReceiveEndpoint(temporaryQueueName, turnoutEndpointConfigurator =>
            {
                configurator.ConfigureTurnoutEndpoints(busFactoryConfigurator, turnoutEndpointConfigurator, configure);
            });
        }

        public static void ConfigureTurnoutEndpoints<T>(this IReceiveEndpointConfigurator endpointConfigurator, IBusFactoryConfigurator busFactoryConfigurator,
            IReceiveEndpointConfigurator turnoutEndpointConfigurator, Action<ITurnoutHostConfigurator<T>> configure)
            where T : class
        {
            var specification = new TurnoutHostSpecification<T>(turnoutEndpointConfigurator);

            configure(specification);

            specification.ControlAddress = turnoutEndpointConfigurator.InputAddress;

            busFactoryConfigurator.AddBusFactorySpecification(specification);

            var jobRoster = specification.JobRoster;
            var superviseInterval = specification.SuperviseInterval;

            turnoutEndpointConfigurator.Consumer(() => new SuperviseJobConsumer(jobRoster, superviseInterval));
            turnoutEndpointConfigurator.Consumer(() => new CancelJobConsumer(jobRoster));

            var controller = specification.Controller;
            IJobFactory<T> jobFactory = specification.JobFactory;

            endpointConfigurator.Consumer(() => new CreateJobConsumer<T>(controller, jobFactory));
        }

        /// <summary>
        /// Sets the job factory to the specified delegate
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="configurator">The turnout configurator</param>
        /// <param name="jobFactory">A function that returns a Task for the job</param>
        public static void SetJobFactory<T>(this ITurnoutHostConfigurator<T> configurator, Func<JobContext<T>, Task> jobFactory)
            where T : class
        {
            configurator.JobFactory = new DelegateJobFactory<T>(jobFactory);
        }
    }
}