namespace MassTransit.Tests.StateMachine
{
    using NUnit.Framework;
    using Rhino.Mocks;

    [TestFixture]
    public class Transition_Specs
    {
        [Test]
        public void The_name_of_the_state_should_match_the_name_of_the_variable()
        {
        	Assert.AreEqual("Idle", MoreFluentState.Idle.Name);

            


        }
    }

    public class MoreFluentState : StateMachineBase<MoreFluentState>
    {
        static MoreFluentState()
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

        public static State<MoreFluentState> Idle { get; set; }
        public static State<MoreFluentState> Active
        {
            get;
            set;
        }

        public StateEvent<MoreFluentState> CommandReceived { get; set; }
        
    
    
    }
}