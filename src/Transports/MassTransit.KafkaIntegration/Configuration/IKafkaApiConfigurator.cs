namespace MassTransit
{
    using System;


    public interface IKafkaApiConfigurator
    {
        /// <summary>
        /// Request broker's supported API versions to adjust functionality to available protocol features. If set to false, or the ApiVersionRequest fails, the fallback
        /// version
        /// `broker.version.fallback` will be used. **NOTE**: Depends on broker version &gt;=0.10.0. If the request is not supported by (an older) broker the
        /// `broker.version.fallback`
        /// fallback is used.
        /// default: true
        /// importance: high
        /// </summary>
        bool? Request { set; }

        /// <summary>
        /// Timeout for broker API version requests.
        /// default: 10000
        /// importance: low
        /// </summary>
        TimeSpan? RequestTimeout { set; }

        /// <summary>
        /// Dictates how long the `broker.version.fallback` fallback is used in the case the ApiVersionRequest fails. **NOTE**: The ApiVersionRequest is only issued when a
        /// new connection
        /// to the broker is made (such as after an upgrade).
        /// default: 0
        /// importance: medium
        /// </summary>
        TimeSpan? FallbackTimeout { set; }

        /// <summary>
        /// Older broker versions (before 0.10.0) provide no way for a client to query for supported protocol features (ApiVersionRequest, see `api.version.request`)
        /// making it impossible
        /// for the client to know what features it may use. As a workaround a user may set this property to the expected broker version and the client will automatically
        /// adjust its
        /// feature set accordingly if the ApiVersionRequest fails (or is disabled). The fallback broker version will be used for `api.version.fallback.ms`. Valid values
        /// are: 0.9.0,
        /// 0.8.2, 0.8.1, 0.8.0. Any other value &gt;= 0.10, such as 0.10.2.1, enables ApiVersionRequests.
        /// default: 0.10.0
        /// importance: medium
        /// </summary>
        string BrokerVersionFallback { set; }
    }
}
