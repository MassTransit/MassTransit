namespace MassTransit.GrpcTransport.Configuration
{
    using Topology;


    /// <summary>
    /// A unique builder context should be created for each specification, so that the items added
    /// by it can be combined together into a group - so that if a subsequent specification yanks
    /// something that conflicts, the system can yank the group or warn that it's impacted.
    /// </summary>
    public interface IGrpcConsumeTopologyBuilder :
        IGrpcTopologyBuilder
    {
        string Exchange { get; set; }
        string Queue { get; set; }
    }
}