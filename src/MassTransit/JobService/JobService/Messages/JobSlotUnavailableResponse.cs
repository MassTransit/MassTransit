#nullable enable
namespace MassTransit.JobService.Messages;

using System;
using Contracts.JobService;


public class JobSlotUnavailableResponse :
    JobSlotUnavailable
{
    public Guid JobId { get; set; }
}
