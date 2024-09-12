#nullable enable
namespace MassTransit.JobService.Messages;

using System;
using Contracts.JobService;


public class JobRetryDelayElapsedEvent :
    JobRetryDelayElapsed
{
    public Guid JobId { get; set; }
}
