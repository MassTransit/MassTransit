#nullable enable
namespace MassTransit.JobService.Messages;

using System;
using System.Collections.Generic;
using Contracts.JobService;


public class JobSubmittedEvent :
    JobSubmitted
{
    public Guid JobId { get; set; }
    public Guid JobTypeId { get; set; }
    public DateTime Timestamp { get; set; }
    public TimeSpan JobTimeout { get; set; }
    public Dictionary<string, object> Job { get; set; } = null!;
    public RecurringJobSchedule? Schedule { get; set; }
}
