#nullable enable
namespace MassTransit.JobService.Messages;

using System;
using Contracts.JobService;


public class JobSlotReleasedEvent :
    JobSlotReleased
{
    public Guid JobTypeId { get; set; }
    public Guid JobId { get; set; }
    public JobSlotDisposition Disposition { get; set; }
}
