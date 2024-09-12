#nullable enable
namespace MassTransit.JobService.Messages;

using System;
using Contracts.JobService;


public class AllocateJobSlotCommand :
    AllocateJobSlot
{
    public Guid JobTypeId { get; set; }
    public TimeSpan JobTimeout { get; set; }
    public Guid JobId { get; set; }
}
