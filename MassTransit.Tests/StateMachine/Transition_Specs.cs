namespace MassTransit.Tests.StateMachine
{
    using NUnit.Framework;
    using Rhino.Mocks;

    [TestFixture]
    public class Transition_Specs
    {
        [Test]
        public void FIRST_TEST_NAME()
        {

            


        }
    }

    public class MoreFluentState : StateMachineBase<MoreFluentState>
    {
        public MoreFluentState()
        {
            Define(() => Idle).AsInitial();
            Define(() => Active);

/*
            Idle
                .When(idle =>
                    {
                        {
                            Entering = () => true;

                            Leaving = () => false;
                        }
                    });
        */}

        public State<MoreFluentState> Idle { get; set; }
        public State<MoreFluentState> Active
        {
            get;
            set;
        }

        public StateEvent<MoreFluentState> CommandReceived { get; set; }
        
    
    
    }
}