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
namespace Automatonymous.Schedules
{
    using System;
    using System.Linq.Expressions;
    using GreenPipes.Internals.Reflection;
    using MassTransit.Internals.Extensions;


    public class StateMachineSchedule<TInstance, TMessage> :
        Schedule<TInstance, TMessage>
        where TInstance : class, SagaStateMachineInstance
        where TMessage : class
    {
        readonly string _name;
        readonly ScheduleSettings<TInstance, TMessage> _settings;
        readonly ReadWriteProperty<TInstance, Guid?> _tokenIdProperty;

        public StateMachineSchedule(string name, Expression<Func<TInstance, Guid?>> tokenIdExpression, ScheduleSettings<TInstance, TMessage> settings)
        {
            _name = name;
            _settings = settings;

            _tokenIdProperty = new ReadWriteProperty<TInstance, Guid?>(tokenIdExpression.GetPropertyInfo());
        }

        string Schedule<TInstance>.Name => _name;
        public TimeSpan Delay => _settings.Delay;
        public Event<TMessage> Received { get; set; }
        public Event<TMessage> AnyReceived { get; set; }

        public Guid? GetTokenId(TInstance instance)
        {
            return _tokenIdProperty.Get(instance);
        }

        public void SetTokenId(TInstance instance, Guid? tokenId)
        {
            _tokenIdProperty.Set(instance, tokenId);
        }
    }
}