// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Topology.Observers
{
    using GreenPipes.Util;
    using Topologies;


    public class MessageTopologyConfigurationObservable :
        Connectable<IMessageTopologyConfigurationObserver>,
        IMessageTopologyConfigurationObserver
    {
        public void MessageTopologyCreated<T>(IMessageTopologyConfigurator<T> configuration)
            where T : class
        {
            All(observer =>
            {
                observer.MessageTopologyCreated(configuration);

                return true;
            });
        }

        public void MessagePropertyTopologyCreated<TMessage, T>(IMessagePropertyTopologyConfigurator<TMessage, T> configuration)
            where TMessage : class
        {
            All(observer =>
            {
                observer.MessagePropertyTopologyCreated(configuration);

                return true;
            });
        }
    }
}
