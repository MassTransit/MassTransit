namespace MassTransit
{
    using System;


    public interface IKafkaSocketConfigurator
    {
        /// <summary>
        /// Default timeout for network requests. Producer: ProduceRequests will use the lesser value of `socket.timeout.ms` and remaining `message.timeout.ms` for the first message in
        /// the batch. Consumer: FetchRequests will use `fetch.wait.max.ms` + `socket.timeout.ms`. Admin: Admin requests will use `socket.timeout.ms` or explicitly set
        /// `rd_kafka_AdminOptions_set_operation_timeout()` value.
        /// default: 60000
        /// importance: low
        /// </summary>
        TimeSpan? Timeout { set; }

        /// <summary>
        /// Broker socket send buffer size. System default is used if 0.
        /// default: 0
        /// importance: low
        /// </summary>
        int? SendBufferBytes { set; }

        /// <summary>
        /// Broker socket receive buffer size. System default is used if 0.
        /// default: 0
        /// importance: low
        /// </summary>
        int? ReceiveBufferBytes { set; }

        /// <summary>
        /// Enable TCP keep-alives (SO_KEEPALIVE) on broker sockets
        /// default: false
        /// importance: low
        /// </summary>
        bool? KeepaliveEnable { set; }

        /// <summary>
        /// Disable the Nagle algorithm (TCP_NODELAY) on broker sockets.
        /// default: false
        /// importance: low
        /// </summary>
        bool? NagleDisable { set; }

        /// <summary>
        /// Disconnect from broker when this number of send failures (e.g., timed out requests) is reached. Disable with 0. WARNING: It is highly recommended to leave this setting at its
        /// default value of 1 to avoid the client and broker to become desynchronized in case of request timeouts. NOTE: The connection is automatically re-established.
        /// default: 1
        /// importance: low
        /// </summary>
        int? MaxFails { set; }
    }
}
