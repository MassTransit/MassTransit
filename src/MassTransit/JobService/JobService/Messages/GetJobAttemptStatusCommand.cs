#nullable enable
namespace MassTransit.JobService.Messages;

using System;
using Contracts.JobService;


public class GetJobAttemptStatusCommand :
    GetJobAttemptStatus
{
    public Guid JobId { get; set; }
    public Guid AttemptId { get; set; }
}
