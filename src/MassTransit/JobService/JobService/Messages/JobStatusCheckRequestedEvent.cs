#nullable enable
namespace MassTransit.JobService.Messages;

using System;
using Contracts.JobService;


public class JobStatusCheckRequestedEvent :
    JobStatusCheckRequested
{
    public Guid AttemptId { get; set; }
}
