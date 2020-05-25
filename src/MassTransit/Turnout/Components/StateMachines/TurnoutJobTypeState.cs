namespace MassTransit.Turnout.Components.StateMachines
{
    using System;
    using System.Collections.Generic;
    using Automatonymous;


    /// <summary>
    /// Every job type has one entry in this state machine
    /// </summary>
    public class TurnoutJobTypeState :
        SagaStateMachineInstance
    {
        public TurnoutJobTypeState()
        {
            ConcurrentJobLimit = 1;
            ActiveJobs = new List<ActiveJob>();
        }

        public int CurrentState { get; set; }

        public int ActiveJobCount { get; set; }

        /// <summary>
        /// The current limit, which may be customized
        /// </summary>
        public int ConcurrentJobLimit { get; set; }

        public int? OverrideJobLimit { get; set; }
        public DateTime? OverrideLimitExpiration { get; set; }

        public List<ActiveJob> ActiveJobs { get; set; }

        public Guid CorrelationId { get; set; }
    }
}
