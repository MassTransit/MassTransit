#nullable enable
namespace MassTransit.Testing;

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;


public static class TelemetryMonitorExtensions
{
    /// <summary>
    /// Wraps the call on the <paramref name="publishEndpoint" /> and waits for the published message to be consumed, along with
    /// all subsequently produced messages until the specified timeout.
    /// </summary>
    /// <param name="publishEndpoint"></param>
    /// <param name="callback"></param>
    /// <param name="timeout"></param>
    /// <param name="idleTimeout"></param>
    public static async Task Wait(this IPublishEndpoint publishEndpoint, Func<IPublishEndpoint, Task>? callback, TimeSpan? timeout = null,
        TimeSpan? idleTimeout = null)
    {
        var methodName = GetTestMethodInfo();

        await using var trackedActivity = new TrackedActivity(methodName, timeout, idleTimeout);

        if (callback != null)
            await callback(publishEndpoint).ConfigureAwait(false);
    }

    /// <summary>
    /// Wraps the call on the <paramref name="sendEndpoint" /> and waits for the sent message to be consumed, along with
    /// all subsequently produced messages until the specified timeout.
    /// </summary>
    /// <param name="sendEndpoint"></param>
    /// <param name="callback"></param>
    /// <param name="timeout"></param>
    /// <param name="idleTimeout"></param>
    public static async Task Wait(this ISendEndpoint sendEndpoint, Func<ISendEndpoint, Task>? callback, TimeSpan? timeout = null,
        TimeSpan? idleTimeout = null)
    {
        var methodName = GetTestMethodInfo();

        await using var trackedActivity = new TrackedActivity(methodName, timeout, idleTimeout);

        if (callback != null)
            await callback(sendEndpoint).ConfigureAwait(false);
    }

    /// <summary>
    /// Wraps the call on the <paramref name="client" /> and waits for the request to be completed, along with
    /// all subsequently produced messages until the specified timeout.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="callback"></param>
    /// <param name="timeout"></param>
    /// <param name="idleTimeout"></param>
    public static async Task<Response<T1>> Wait<T, T1>(this IRequestClient<T> client, Func<IRequestClient<T>, Task<Response<T1>>> callback,
        TimeSpan? timeout = null, TimeSpan? idleTimeout = null)
        where T : class
        where T1 : class
    {
        if (callback == null)
            throw new ArgumentNullException(nameof(callback));

        var methodName = GetTestMethodInfo();

        await using var trackedActivity = new TrackedActivity(methodName, timeout, idleTimeout);

        Response<T1> result = await callback(client).ConfigureAwait(false);

        return result;
    }

    /// <summary>
    /// Wraps the call on the <paramref name="client" /> and waits for the request to be completed, along with
    /// all subsequently produced messages until the specified timeout.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="callback"></param>
    /// <param name="timeout"></param>
    /// <param name="idleTimeout"></param>
    public static async Task<Response<T1, T2>> Wait<T, T1, T2>(this IRequestClient<T> client, Func<IRequestClient<T>, Task<Response<T1, T2>>> callback,
        TimeSpan? timeout = null, TimeSpan? idleTimeout = null)
        where T : class
        where T1 : class
        where T2 : class
    {
        if (callback == null)
            throw new ArgumentNullException(nameof(callback));

        var methodName = GetTestMethodInfo();

        await using var trackedActivity = new TrackedActivity(methodName, timeout, idleTimeout);

        Response<T1, T2> result = await callback(client).ConfigureAwait(false);

        return result;
    }

    /// <summary>
    /// Wraps the call on the <paramref name="client" /> and waits for the request to be completed, along with
    /// all subsequently produced messages until the specified timeout.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="callback"></param>
    /// <param name="timeout"></param>
    /// <param name="idleTimeout"></param>
    public static async Task<Response<T1, T2, T3>> Wait<T, T1, T2, T3>(this IRequestClient<T> client,
        Func<IRequestClient<T>, Task<Response<T1, T2, T3>>> callback, TimeSpan? timeout = null, TimeSpan? idleTimeout = null)
        where T : class
        where T1 : class
        where T2 : class
        where T3 : class
    {
        if (callback == null)
            throw new ArgumentNullException(nameof(callback));

        var methodName = GetTestMethodInfo();

        await using var trackedActivity = new TrackedActivity(methodName, timeout, idleTimeout);

        Response<T1, T2, T3> result = await callback(client).ConfigureAwait(false);

        return result;
    }

    static string? GetTestMethodInfo()
    {
        var stackTrace = new StackTrace(2);
        var frameCount = stackTrace.FrameCount;
        for (var i = 0; i < frameCount; i++)
        {
            var frame = stackTrace.GetFrame(i);
            if (frame == null)
                continue;

            var method = frame.GetMethod();
            if (method == null)
                continue;

            if (method.GetCustomAttributes(false).Any(x =>
                {
                    var name = x.GetType().Name;
                    return name.ToLower().Contains("test") || name.ToLower().Contains("fact");
                }))
                return method.Name;
        }

        return null;
    }
}
