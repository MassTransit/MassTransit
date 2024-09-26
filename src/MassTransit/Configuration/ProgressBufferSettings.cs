namespace MassTransit;

using System;


/// <summary>
/// Specifies the settings for the progress buffer, which defers updating the job progress until the
/// thresholds (steps or duration) have been reached.
/// </summary>
public class ProgressBufferSettings
{
    public ProgressBufferSettings()
    {
        Enabled = true;
        UpdateLimit = 1000;
        TimeLimit = TimeSpan.FromSeconds(30);
    }

    /// <summary>
    /// If true, job progress updates are buffered to avoid overloading the job saga
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// The number of progress updates reported before the value is sent to the job saga
    /// </summary>
    public int UpdateLimit { get; set; }

    /// <summary>
    /// The time period after which the progress should be reported
    /// </summary>
    public TimeSpan TimeLimit { get; set; }
}
