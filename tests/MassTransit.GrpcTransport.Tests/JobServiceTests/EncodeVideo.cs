namespace MassTransit.GrpcTransport.Tests.JobServiceTests
{
    using System;


    public interface EncodeVideo
    {
        Guid VideoId { get; }
        string Path { get; }
        int Duration { get; }
    }
}
