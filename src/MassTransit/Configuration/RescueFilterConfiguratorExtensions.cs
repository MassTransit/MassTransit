// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Linq;
    using PipeConfigurators;
    using Pipeline;
    using Pipeline.Filters;

    public static class RescueFilterConfiguratorExtensions
    {
        /// <summary>
        /// Rescue exceptions via the alternate pipe
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="rescuePipe"></param>
        /// <param name="exceptionTypes"></param>
        public static void Rescue<T>(this IPipeConfigurator<T> configurator, IPipe<T> rescuePipe, params Type[] exceptionTypes)
            where T : class, PipeContext
        {
            if (configurator == null)
                throw new ArgumentNullException("configurator");

            RescueExceptionFilter exceptionFilter = exception => exceptionTypes.Any(x => x.IsInstanceOfType(exception));

            var rescueConfigurator = new RescuePipeBuilderConfigurator<T>(rescuePipe, exceptionFilter);

            configurator.AddPipeBuilderConfigurator(rescueConfigurator);
        }
    }
}