#nullable enable
namespace MassTransit.JobService.Messages;

using System;
using Contracts.JobService;


public class GetJobStateRequest :
    GetJobState
{
    public Guid JobId { get; set; }
}
