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
namespace MassTransit.SagaConfigurators
{
    using GreenPipes.Util;
    using Saga;


    public class SagaConfigurationObservable :
        Connectable<ISagaConfigurationObserver>,
        ISagaConfigurationObserver
    {
        public void SagaConfigured<TSaga>(ISagaConfigurator<TSaga> configurator) 
            where TSaga : class, ISaga
        {
            All(observer =>
            {
                observer.SagaConfigured(configurator);

                return true;
            });
        }

        public void SagaMessageConfigured<TSaga, TMessage>(ISagaMessageConfigurator<TSaga, TMessage> configurator) 
            where TSaga : class, ISaga
            where TMessage : class
        {
            All(observer =>
            {
                observer.SagaMessageConfigured(configurator);

                return true;
            });
        }
    }
}