namespace MassTransit.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MassTransit.Saga;
    using NUnit.Framework;
    using Magnum.TestFramework;
    using Magnum.Extensions;
    using TestFramework;

    [TestFixture]
    public class ReflectionExploration
    {

        [Test]
        public void Bill()
        {
            
            var bob = typeof (Bob);
            var x = bob.GetConsumerTypesFiltered();

            x.Alls.Count.ShouldEqual(0, "all");
            x.Selecteds.Count.ShouldEqual(1, "selecteds");
            x.Correlateds.Count.ShouldEqual(0, "correlated");
            x.Orchestrates.Count.ShouldEqual(0, "orch");
            x.Initiates.Count.ShouldEqual(0, "init");
            x.Observers.Count.ShouldEqual(0, "obsrv");
        }

        [Test]
        public void Orchestrates()
        {
            var squid = typeof (Squid);
            var x = squid.GetConsumerTypesFiltered();


            x.Alls.Count.ShouldEqual(0, "all");
            x.Selecteds.Count.ShouldEqual(0, "selecteds");
            x.Correlateds.Count.ShouldEqual(0, "correlated");
            x.Orchestrates.Count.ShouldEqual(1, "orch");
            x.Initiates.Count.ShouldEqual(1, "init");
            x.Observers.Count.ShouldEqual(0, "obsrv");
        }


        class Bob : Consumes<object>.Selected
        {
            public bool Accept(object message)
            {
                return true;
            }

            public void Consume(object message)
            {
                
            }
        }

        class Squid :
            InitiatedBy<X>,
            Orchestrates<Y>
        {
            public void Consume(Y message)
            {
                throw new ATestException();
            }

            public void Consume(X message)
            {
                throw new ATestException();
            }
        }

        class X : CorrelatedBy<Guid>
        {
            public Guid CorrelationId
            {
                get { return Guid.Empty; }
            }
        }
        class Y : CorrelatedBy<Guid>
        {
            public Guid CorrelationId
            {
                get { return Guid.Empty; }
            }
        }
    }

    public static class TypeExtensions
    {
        public static IEnumerable<Type> GetClosedImplementationsOf(this Type type, Type openTypeToSearchFor)
        {
            if (!openTypeToSearchFor.IsGenericType) yield break;

            
            foreach (var i in type.GetInterfaces())
            {
                if (!i.IsGenericType) continue;

                var g = i.GetGenericTypeDefinition();
                
                if(g.Equals(openTypeToSearchFor))
                {
                    yield return i;
                }
            }
        }

        public static SubscriptionInterfaces GetConsumerTypesFiltered(this Type type)
        {

            
            //get all types that are IConsumer
            var consumerTypes = from i in type.GetInterfaces()
                                where i.Implements(typeof (IConsumer))
                                where !i.Equals(typeof(IConsumer))
                                select i;

            //now that we have the IConsumers we need to slot them into buckets
            var observers = from ob in consumerTypes
                            where ob.Implements(typeof (Observes<,>))
                            select ob;


            var initiates = from i in consumerTypes
                            where i.Implements(typeof (InitiatedBy<>))
                            select i;

            var orchestrates = from i in consumerTypes
                               where i.Implements(typeof (Orchestrates<>))
                               where NotInitiator(i, initiates)
                               select i;

            var selected = from i in consumerTypes
                           where i.ImplementsGeneric(typeof (Consumes<>.Selected))
                           select i;

            var correlated = from i in consumerTypes
                             where i.ImplementsGeneric(typeof (Consumes<>.For<>))
                             select i;

            var alls = from i in consumerTypes
                       where i.ImplementsGeneric(typeof (Consumes<>.All))
                       where !(correlated.Contains(i) || selected.Contains(i))
                       where NotASelectedCorrelatedMessageType(i, selected, correlated, orchestrates, initiates,observers)
                       select i;

            return new SubscriptionInterfaces()
                {
                    Alls = alls.ToList(),
                    Correlateds = correlated.ToList(),
                    Selecteds = selected.ToList(),
                    Orchestrates = orchestrates.ToList(),
                    Initiates = initiates.ToList(),
                    Observers = observers.ToList(),
                };
        }

        static bool NotInitiator(Type type, IEnumerable<Type> initiates)
        {
            var im = from i in initiates
                     select i.GetGenericArguments()[0];

            var m = type.GetGenericArguments()[0];

            return !im.Contains(m);
        }

        static bool NotASelectedCorrelatedMessageType(Type t, params IEnumerable<Type>[] selected)
        {
            var compiled = from s in selected
                           from ta in s
                           select ta.GetGenericArguments()[0];


            var m = t.GetGenericArguments()[0];

            return !compiled.Contains(m);
        }
    }

    public class SubscriptionInterfaces
    {
        public List<Type> Selecteds;
        public List<Type> Alls;
        public List<Type> Correlateds;
        public List<Type> Orchestrates;
        public List<Type> Initiates;
        public List<Type> Observers;
    }
}