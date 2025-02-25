#nullable enable
namespace MassTransit.JobService.Messages;

using System;
using Contracts.JobService;


public class RetryJobCommand :
    RetryJob
{
    public Guid JobId { get; set; }
}
