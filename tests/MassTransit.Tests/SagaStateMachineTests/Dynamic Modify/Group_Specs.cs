namespace MassTransit.Tests.SagaStateMachineTests.Dynamic_Modify
{
    using System;
    using NUnit.Framework;


    // NOTE: This test was pulled from the non-dynamic set; Seems incomplete with the commented out code below.
    [TestFixture(Category = "Dynamic Modify")]
    public class Declaring_groups_in_a_state_machine
    {
        [Test]
        public void Should_allow_parallel_execution_of_events()
        {
        }

        [Test]
        public void Should_have_captured_initial_data()
        {
            Assert.AreEqual("Audi", _instance.VehicleMake);
            Assert.AreEqual("A6", _instance.VehicleModel);
        }

        State BeingServiced;
        Event<Vehicle> VehicleArrived;

        StateMachine<PitStopInstance> _machine;
        PitStopInstance _instance;

        [OneTimeSetUp]
        public void Setup()
        {
            _instance = new PitStopInstance();
            _machine = MassTransitStateMachine<PitStopInstance>
                .New(builder => builder
                    .State("BeingServiced", out BeingServiced)
                    .Event("VehicleArrived", out VehicleArrived)
                    .InstanceState(b => b.OverallState)
                    .During(builder.Initial)
                        .When(VehicleArrived, b => b
                            .Then(context =>
                            {
                                context.Instance.VehicleMake = context.Data.Make;
                                context.Instance.VehicleModel = context.Data.Model;
                            })
                            .TransitionTo(BeingServiced))
                );

            var vehicle = new Vehicle
            {
                Make = "Audi",
                Model = "A6",
            };

            _machine.RaiseEvent(_instance, VehicleArrived, vehicle).Wait();
        }


        class PitStopInstance :
SagaStateMachineInstance
        {
            public Guid CorrelationId { get; set; }
            public State OverallState { get; private set; }
            public State FuelState { get; private set; }
            public State OilState { get; private set; }

            public string VehicleMake { get; set; }
            public string VehicleModel { get; set; }

            public decimal FuelGallons { get; set; }
            public decimal FuelPricePerGallon { get; set; }
            public decimal FuelCost { get; set; }

            public decimal OilQuarts { get; set; }
            public decimal OilPricePerQuart { get; set; }
            public decimal OilCost { get; set; }
        }

        // NOTE: Left in place due to the incompleteness of this test.
//        class PitStop :
//            MassTransitStateMachine<PitStopInstance>
//        {
//            public PitStop()
//            {
//                InstanceState(x => x.OverallState);

//                During(Initial,
//                    When(VehicleArrived)
//                        .Then(context =>
//                        {
//                            context.Instance.VehicleMake = context.Data.Make;
//                            context.Instance.VehicleModel = context.Data.Model;
//                        })
//                        .TransitionTo(BeingServiced)
////                        .RunParallel(p =>
////                            {
////                                p.Start<FillTank>(x => x.BeginFilling);
////                                p.Start<CheckOil>(x => x.BeginChecking);
////                            }))
//                    );
//            }

//            public State BeingServiced { get; private set; }

//            public Event<Vehicle> VehicleArrived { get; private set; }
//        }


        class FillTank :
            MassTransitStateMachine<PitStopInstance>
        {
            public FillTank()
            {
                InstanceState(x => x.FuelState);

                Initially(
                    When(Initial.Enter)
                        .TransitionTo(Filling));

                During(Filling,
                    When(Full)
                        .Then(context =>
                        {
                            context.Instance.FuelGallons = context.Data.Gallons;
                            context.Instance.FuelPricePerGallon = context.Data.PricePerGallon;
                            context.Instance.FuelCost = context.Data.Gallons * context.Data.PricePerGallon;
                        })
                        .Finalize());
            }

            public State Filling { get; private set; }

            public Event<FuelDispensed> Full { get; private set; }
        }


        class CheckOil :
            MassTransitStateMachine<PitStopInstance>
        {
            public CheckOil()
            {
                InstanceState(x => x.OilState);

                Initially(
                    When(Initial.Enter)
                        .TransitionTo(AddingOil));

                During(AddingOil,
                    When(Done)
                        .Then(context =>
                        {
                            context.Instance.OilQuarts = context.Data.Quarts;
                            context.Instance.OilPricePerQuart = context.Data.PricePerQuart;
                            context.Instance.OilCost = Math.Ceiling(context.Data.Quarts) * context.Data.PricePerQuart;
                        })
                        .Finalize());
            }

            public State AddingOil { get; private set; }

            public Event<OilAdded> Done { get; private set; }
        }


        class FuelDispensed
        {
            public decimal Gallons { get; set; }
            public decimal PricePerGallon { get; set; }
        }


        class OilAdded
        {
            public decimal Quarts { get; set; }
            public decimal PricePerQuart { get; set; }
        }


        class Vehicle
        {
            public string Make { get; set; }
            public string Model { get; set; }
        }
    }
}
