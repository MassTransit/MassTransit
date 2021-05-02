namespace MassTransit.JobService.Components.StateMachines
{
    using System;


    public class JobTypeInstance
    {
        public DateTime? Updated { get; set; }
        public DateTime? Used { get; set; }
    }
}
