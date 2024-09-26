namespace MassTransit;

using System;
using System.Threading.Tasks;
using Contracts.JobService;


public interface INotifyJobContext
{
    Task NotifyCanceled(string? reason = null);
    Task NotifyStarted();
    Task NotifyCompleted();
    Task NotifyFaulted(Exception exception, TimeSpan? delay = default);
    Task NotifyJobProgress(SetJobProgress progress);
}
