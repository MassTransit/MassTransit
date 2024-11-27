namespace MassTransit;

using System;
using System.Collections.Generic;
using Transports;


public static class SendHeadersExtensions
{
    /// <summary>
    /// Copy the headers from an existing header collection into the <paramref name="sendHeaders" /> header collection
    /// </summary>
    /// <param name="sendHeaders"></param>
    /// <param name="headers">The source header collection</param>
    /// <exception cref="ArgumentNullException"></exception>
    public static void CopyFrom(this SendHeaders sendHeaders, Headers headers)
    {
        if (sendHeaders == null)
            throw new ArgumentNullException(nameof(sendHeaders));
        if (headers == null)
            throw new ArgumentNullException(nameof(headers));

        foreach (var header in headers)
            sendHeaders.Set(header.Key, header.Value);
    }

    /// <summary>
    /// Copy the headers from an existing header collection into the <paramref name="sendHeaders" /> header collection
    /// </summary>
    /// <param name="adapter">The dictionary adapter for setting headers</param>
    /// <param name="sendHeaders">The send header collection</param>
    /// <param name="headers">The source header collection</param>
    /// <exception cref="ArgumentNullException"></exception>
    public static void CopyFrom<T>(this ITransportSetHeaderAdapter<T> adapter, IDictionary<string, T> sendHeaders, Headers headers)
    {
        if (adapter == null)
            throw new ArgumentNullException(nameof(adapter));
        if (headers == null)
            throw new ArgumentNullException(nameof(headers));

        foreach (var header in headers)
            adapter.Set(sendHeaders, header);
    }
}
