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
namespace Automatonymous.Activities
{
    using Binders;


    public class StateMachineActivitySelector<TInstance> :
        IStateMachineActivitySelector<TInstance>
        where TInstance : class, SagaStateMachineInstance
    {
        readonly EventActivityBinder<TInstance> _binder;

        public StateMachineActivitySelector(EventActivityBinder<TInstance> binder)
        {
            _binder = binder;
        }

        EventActivityBinder<TInstance> IStateMachineActivitySelector<TInstance>.OfType<TActivity>()
        {
            var activity = new ContainerFactoryActivity<TInstance, TActivity>();

            return _binder.Add(activity);
        }
    }


    public class StateMachineActivitySelector<TInstance, TData> :
        IStateMachineActivitySelector<TInstance, TData>
        where TInstance : class, SagaStateMachineInstance
        where TData : class
    {
        readonly EventActivityBinder<TInstance, TData> _binder;

        public StateMachineActivitySelector(EventActivityBinder<TInstance, TData> binder)
        {
            _binder = binder;
        }

        EventActivityBinder<TInstance, TData> IStateMachineActivitySelector<TInstance, TData>.OfType<TActivity>()
        {
            var activity = new ContainerFactoryActivity<TInstance, TData, TActivity>();

            return _binder.Add(activity);
        }

        EventActivityBinder<TInstance, TData> IStateMachineActivitySelector<TInstance, TData>.OfInstanceType<TActivity>()
        {
            var activity = new ContainerFactoryActivity<TInstance, TActivity>();

            return _binder.Add(activity);
        }
    }
}