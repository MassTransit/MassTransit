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
namespace MassTransit.DocumentDbIntegration.Tests
{
    using System;
    using Automatonymous;
    using DocumentDbIntegration;
    using Newtonsoft.Json;


    /// <summary>
    ///     Why to exit the door to go shopping
    /// </summary>
    public class GirlfriendYelling
    {
        public Guid CorrelationId { get; set; }
    }


    public class GotHitByACar
    {
        public Guid CorrelationId { get; set; }
    }


    public class SodOff
    {
        public Guid CorrelationId { get; set; }
    }


    public class ShoppingChore : IVersionedSaga,
        SagaStateMachineInstance
    {
        protected ShoppingChore()
        {
        }

        public ShoppingChore(Guid correlationId)
        {
            CorrelationId = correlationId;
        }

        public string CurrentState { get; set; }
        public int Everything { get; set; }
        public bool Screwed { get; set; }

        [JsonProperty("id")]
        public Guid CorrelationId { get; set; }

        [JsonProperty("_etag")]
        public string ETag { get; set; }
    }


    public class SuperShopper :
        MassTransitStateMachine<ShoppingChore>
    {
        public SuperShopper()
        {
            InstanceState(x => x.CurrentState);

            Event(() => ExitFrontDoor, x => x.CorrelateById(context => context.Message.CorrelationId));
            Event(() => GotHitByCar, x => x.CorrelateById(context => context.Message.CorrelationId));
            Event(() => JustSodOff, x => x.CorrelateById(context => context.Message.CorrelationId));

            CompositeEvent(() => EndOfTheWorld, x => x.Everything, CompositeEventOptions.IncludeInitial, ExitFrontDoor, GotHitByCar);

            Initially(
                When(ExitFrontDoor)
                    .Then(context => Console.Write("Leaving!"))
                    .TransitionTo(OnTheWayToTheStore));

            During(OnTheWayToTheStore,
                When(GotHitByCar)
                    .Then(context => Console.WriteLine("Ouch!!"))
                    .TransitionTo(Dead));

            DuringAny(
                When(EndOfTheWorld)
                    .Then(context => Console.WriteLine("Screwed!!"))
                    .Then(context => context.Instance.Screwed = true));

            DuringAny(
                When(JustSodOff)
                    .Finalize());

            SetCompletedWhenFinalized();
        }

        public Event<GirlfriendYelling> ExitFrontDoor { get; private set; }
        public Event<GotHitByACar> GotHitByCar { get; private set; }
        public Event<SodOff> JustSodOff { get; private set; }
        public Event EndOfTheWorld { get; private set; }
        public State OnTheWayToTheStore { get; private set; }
        public State Dead { get; private set; }
    }
}
