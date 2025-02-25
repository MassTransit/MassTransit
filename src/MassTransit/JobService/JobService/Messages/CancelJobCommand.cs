#nullable enable
namespace MassTransit.JobService.Messages;

using System;
using Contracts.JobService;


public class CancelJobCommand :
    CancelJob
{
    public Guid JobId { get; set; }
    public string? Reason { get; set; }
}
