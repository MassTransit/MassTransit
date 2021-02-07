namespace MassTransit.TestComponents.ForkJoint.Futures
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Contracts;
    using MassTransit.Futures;


    public class OrderFuture :
        Future<SubmitOrder, OrderCompleted, OrderFaulted>
    {
        public OrderFuture()
        {
            ConfigureCommand(x => x.CorrelateById(context => context.Message.OrderId));

            SendRequests<Burger, OrderBurger>(x => x.Burgers, x =>
                {
                    x.UsingRequestInitializer(context => MapOrderBurger(context));
                    x.TrackPendingRequest(message => message.OrderLineId);
                })
                .OnResponseReceived<BurgerCompleted>(x => x.CompletePendingRequest(message => message.OrderLineId));

            SendRequests<Fry, OrderFry>(x => x.Fries, x =>
                {
                    x.UsingRequestInitializer(context => MapOrderFry(context));
                    x.TrackPendingRequest(message => message.OrderLineId);
                })
                .OnResponseReceived<FryCompleted>(x => x.CompletePendingRequest(message => message.OrderLineId));

            SendRequests<Shake, OrderShake>(x => x.Shakes, x =>
                {
                    x.UsingRequestInitializer(context => MapOrderShake(context));
                    x.TrackPendingRequest(message => message.OrderLineId);
                })
                .OnResponseReceived<ShakeCompleted>(x => x.CompletePendingRequest(message => message.OrderLineId));

            SendRequests<FryShake, OrderFryShake>(x => x.FryShakes, x =>
                {
                    x.UsingRequestInitializer(context => MapOrderFryShake(context));
                    x.TrackPendingRequest(message => message.OrderLineId);
                })
                .OnResponseReceived<FryShakeCompleted>(x => x.CompletePendingRequest(message => message.OrderLineId));


            WhenAllCompleted(r => r.SetCompletedUsingInitializer(context => new
            {
                LinesCompleted = context.Instance.Results.Select(x => x.Value.ToObject<OrderLineCompleted>()).ToDictionary(x => x.OrderLineId),
            }));

            WhenAnyFaulted(f => f.SetFaultedUsingInitializer(context => MapOrderFaulted(context)));
        }

        static object MapOrderFryShake(FutureConsumeContext<FryShake> context)
        {
            return new
            {
                OrderId = context.Instance.CorrelationId,
                OrderLineId = context.Message.FryShakeId,
                context.Message.Size,
                context.Message.Flavor
            };
        }

        static object MapOrderShake(FutureConsumeContext<Shake> context)
        {
            return new
            {
                OrderId = context.Instance.CorrelationId,
                OrderLineId = context.Message.ShakeId,
                context.Message.Size,
                context.Message.Flavor
            };
        }

        static object MapOrderFry(FutureConsumeContext<Fry> context)
        {
            return new
            {
                OrderId = context.Instance.CorrelationId,
                OrderLineId = context.Message.FryId,
                context.Message.Size,
            };
        }

        static object MapOrderBurger(FutureConsumeContext<Burger> context)
        {
            return new
            {
                OrderId = context.Instance.CorrelationId,
                OrderLineId = context.Message.BurgerId,
                Burger = context.Message
            };
        }

        static object MapOrderFaulted(FutureConsumeContext context)
        {
            Dictionary<Guid, Fault> faults = context.Instance.Faults.ToDictionary(x => x.Key, x => x.Value.ToObject<Fault>());

            return new
            {
                LinesCompleted = context.Instance.Results.ToDictionary(x => x.Key, x => x.Value.ToObject<OrderLineCompleted>()),
                LinesFaulted = faults,
                Exceptions = faults.SelectMany(x => x.Value.Exceptions).ToArray()
            };
        }
    }
}
