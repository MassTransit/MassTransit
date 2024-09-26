#nullable enable
namespace MassTransit.JobService.Messages;

using System;
using Contracts.JobService;


public class SetJobProgressCommand :
    SetJobProgress
{
    public Guid JobId { get; set; }
    public Guid AttemptId { get; set; }
    public long SequenceNumber { get; set; }
    public long Value { get; set; }
    public long? Limit { get; set; }
}
