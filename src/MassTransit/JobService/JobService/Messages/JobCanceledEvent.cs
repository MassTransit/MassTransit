#nullable enable
namespace MassTransit.JobService.Messages;

using System;
using Contracts.JobService;


public class JobCanceledEvent :
    JobCanceled
{
    public Guid JobId { get; set; }
    public DateTime Timestamp { get; set; }
}
