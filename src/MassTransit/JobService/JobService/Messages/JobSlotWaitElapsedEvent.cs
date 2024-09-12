#nullable enable
namespace MassTransit.JobService.Messages;

using System;
using Contracts.JobService;


public class JobSlotWaitElapsedEvent :
    JobSlotWaitElapsed
{
    public Guid JobId { get; set; }
}
