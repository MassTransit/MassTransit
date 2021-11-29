namespace MassTransit.Transports
{
    using System;
    using Logging;
    using Microsoft.Extensions.Logging;


    public static class TransportLogMessages
    {
        public static readonly LogMessage<string> ConnectHost = LogContext.Define<string>(LogLevel.Debug,
            "Connect: {Host}");

        public static readonly LogMessage<string> ConnectedHost = LogContext.Define<string>(LogLevel.Debug,
            "Connected: {Host}");

        public static readonly LogMessage<string> DisconnectHost = LogContext.Define<string>(LogLevel.Debug,
            "Disconnect: {Host}");

        public static readonly LogMessage<string> DisconnectedHost = LogContext.Define<string>(LogLevel.Debug,
            "Disconnected: {Host}");

        public static readonly LogMessage<string> StoppingSendTransport = LogContext.Define<string>(LogLevel.Debug,
            "Stopping send transport: {Destination}");

        public static readonly LogMessage<Uri> StoppingReceiveTransport = LogContext.Define<Uri>(LogLevel.Debug,
            "Stopping receive transport: {InputAddress}");

        public static readonly LogMessage<Uri> ConnectReceiveEndpoint = LogContext.Define<Uri>(LogLevel.Debug,
            "Connect receive endpoint: {InputAddress}");

        public static readonly LogMessage<Uri, string> ConnectSubscriptionEndpoint = LogContext.Define<Uri, string>(LogLevel.Debug,
            "Connect subscription endpoint: {InputAddress}({SubscriptionName})");

        public static readonly LogMessage<Uri> CreateReceiveTransport = LogContext.Define<Uri>(LogLevel.Debug,
            "Create receive transport: {InputAddress}");

        public static readonly LogMessage<Uri> CreatePublishTransport = LogContext.Define<Uri>(LogLevel.Debug,
            "Create publish transport: {DestinationAddress}");

        public static readonly LogMessage<string> CreateExchange = LogContext.Define<string>(LogLevel.Debug,
            "Create exchange: {Exchange}");

        public static readonly LogMessage<string> CreateQueue = LogContext.Define<string>(LogLevel.Debug,
            "Create queue: {Queue}");

        public static readonly LogMessage<string> CreateTopic = LogContext.Define<string>(LogLevel.Debug,
            "Create topic: {Topic}");

        public static readonly LogMessage<Uri> CreateSendTransport = LogContext.Define<Uri>(LogLevel.Debug,
            "Create send transport: {DestinationAddress}");

        public static readonly LogMessage<string> DeleteQueue = LogContext.Define<string>(LogLevel.Debug,
            "Delete queue: {Queue}");

        public static readonly LogMessage<string, string> DeleteSubscription = LogContext.Define<string, string>(LogLevel.Debug,
            "Delete subscription: {Queue} {Subscription}");

        public static readonly LogMessage<string> DeleteTopic = LogContext.Define<string>(LogLevel.Debug,
            "Delete topic: {Topic}");
    }
}
