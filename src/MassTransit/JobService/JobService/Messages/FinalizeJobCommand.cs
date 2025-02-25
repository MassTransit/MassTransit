#nullable enable
namespace MassTransit.JobService.Messages;

using System;
using Contracts.JobService;


public class FinalizeJobCommand :
    FinalizeJob
{
    public Guid JobId { get; set; }
}
