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
namespace MassTransit
{
    using System;
    using ConsumeConfigurators;
    using Courier;
    using Registration;
    using Saga;


    public interface IRegistrationConfigurator
    {
        void AddConsumer<T>(Action<IConsumerConfigurator<T>> configure = null)
            where T : class, IConsumer;

        void AddConsumer(Type consumerType);

        void AddSaga<T>(Action<ISagaConfigurator<T>> configure = null)
            where T : class, ISaga;

        void AddSaga<T>(SagaRegistrationFactory<T> factory, Action<ISagaConfigurator<T>> configure = null)
            where T : class, ISaga;

        void AddSaga(Type sagaType);

        void AddExecuteActivity<TActivity, TArguments>(Action<IExecuteActivityConfigurator<TActivity, TArguments>> configure = null)
            where TActivity : class, ExecuteActivity<TArguments>
            where TArguments : class;

        void AddExecuteActivity(Type activityType);

        void AddActivity<TActivity, TArguments, TLog>(Action<IExecuteActivityConfigurator<TActivity, TArguments>> configureExecute = null,
            Action<ICompensateActivityConfigurator<TActivity, TLog>> configureCompensate = null)
            where TActivity : class, Activity<TArguments, TLog>
            where TLog : class
            where TArguments : class;

        void AddActivity(Type activityType);
    }
}
