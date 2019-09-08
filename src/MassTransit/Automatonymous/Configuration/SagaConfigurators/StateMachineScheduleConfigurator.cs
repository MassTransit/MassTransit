// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace Automatonymous.SagaConfigurators
{
    using System;


    public class StateMachineScheduleConfigurator<TInstance, TMessage> :
        IScheduleConfigurator<TInstance, TMessage>,
        ScheduleSettings<TInstance, TMessage>
        where TInstance : class, SagaStateMachineInstance
        where TMessage : class
    {
        Action<IEventCorrelationConfigurator<TInstance, TMessage>> _received;

        public StateMachineScheduleConfigurator()
        {
            Delay = TimeSpan.FromSeconds(30);
        }

        public ScheduleSettings<TInstance, TMessage> Settings => this;

        public TimeSpan Delay { get; set; }

        Action<IEventCorrelationConfigurator<TInstance, TMessage>> IScheduleConfigurator<TInstance, TMessage>.Received
        {
            set { _received = value; }
        }

        Action<IEventCorrelationConfigurator<TInstance, TMessage>> ScheduleSettings<TInstance, TMessage>.Received
        {
            get { return _received; }
        }
    }
}