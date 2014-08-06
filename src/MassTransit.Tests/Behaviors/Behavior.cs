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
namespace MassTransit.Tests.Behaviors
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Pipeline;


    public interface Behavior
    {
    }


    public interface Behavior<in TState> :
        Behavior
    {
    }


    public interface BehaviorHandler<in TState> :
        IFilter<ConsumeContext>
    {
    }


    public interface BehaviorHandler<in TState, in TMessage>
        where TMessage : class
    {
        Task<Behavior> Handle(BehaviorContext<TState, TMessage> context);
    }


    public interface IBehaviorFactory<out TState>
        where TState : class
    {
        Task Send<TMessage>(ConsumeContext<TMessage> context, IPipe<BehaviorContext<TState, TMessage>> next)
            where TMessage : class;
    }


    public interface BehaviorContext<out TState, out T> :
        ConsumeContext<T>
        where T : class
    {
        TState State { get; }

        ConsumeContext<T> Pop();
    }


    class MemberState
    {
        public string Name { get; set; }
        public string Buddy { get; set; }
    }


    class AddBuddy
    {
        public string Name { get; set; }
    }

    class Welcome
    {
        public string Name { get; set; }
    }


    class InitialMemberBehavior :
        Behavior,
        BehaviorHandler<MemberState, Welcome>
    {
        public async Task<Behavior> Handle(BehaviorContext<MemberState, Welcome> context)
        {
            context.State.Name = context.Message.Name;

            return new NewMemberBehavior();
        }
    }


    class NewMemberBehavior : 
        Behavior,
        BehaviorHandler<MemberState, AddBuddy>
    {
        async Task<Behavior> BehaviorHandler<MemberState, AddBuddy>.Handle(BehaviorContext<MemberState, AddBuddy> context)
        {
            context.State.Buddy = context.Message.Name;

            return this;
        }
    }
}