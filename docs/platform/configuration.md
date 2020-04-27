# Configuration

Containers are configured using environment variables.


### MT_TRANSPORT

> `MT_TRANSPORT=ASB`

Specify the transport used by the service.

| Value | Transport
|----|-----
| RMQ | RabbitMQ (default)
| ASB | Azure Service Bus
| AMQ | ActiveMQ, including Amazon MQ
| SQS | Amazon SQS


#### RabbitMQ

| Value | Description | Default
|:----|:-----|:-----
| MT_RMQ__HOST | The host address | `rabbitmq` or `localhost`
| MT_RMQ__PORT | The host port | `5672` (or `5671` for SSL)
| MT_RMQ__VHOST | Virtual Host name | `/`
| MT_RMQ__USER | Sign in username | `guest`
| MT_RMQ__PASS | Sign in password | `guest`
| MT_RMQ__USESSL | Use SSL | `false`
| MT_RMQ__SSL__SERVERNAME | Server name matching the CN | 
| MT_RMQ__SSL__TRUST | Trust the certificate, ignoring errors | `false`
| MT_RMQ__SSL__CERTPATH | Path to a client certificate | 
| MT_RMQ__SSL__CERTPASSPHRASE | Passphrase for the certificate | 
| MT_RMQ__SSL__CERTIDENTITY | Use the certificate to authenticate | `false`


#### Azure Service Bus

| Value | Description | Default
|:----|:-----|:-----
| MT_ASB__CONNECTIONSTRING | The full connection string | 

#### ActiveMQ (including Amazon MQ)

| Value | Description | Default
|:----|:-----|:-----
| MT_AMQ__HOST | The host address | `activemq` or `localhost`
| MT_AMQ__PORT | The host port | `61616`
| MT_AMQ__USER | Sign in username | `admin`
| MT_AMQ__PASS | Sign in password | `admin`
| MT_AMQ__USESSL | Use SSL | `false`, `true` if _aws_ found in host name

#### Amazon SQS

| Value | Description | Default
|:----|:-----|:-----
| MT_SQS__REGION | The AWS region name | 
| MT_SQS__SCOPE | The scope name | 
| MT_SQS__ACCESSKEY | The AWS Access Key |
| MT_SQS__SECRETKEY | The AWS Secret Key |


### MT_SCHEDULER

> `MT_SCHEDULER=quartz`

If specified, the name of the queue for the message scheduler endpoint. The name will automatically be converted to the appropriate address for the transport.

If not specified, the transport-specific delayed message delivery mechanism is configured. For RabbitMQ, this configures the delayed exchange message scheduler via the `.UseDelayedExchangeMessageScheduler()` method.

> To configure the connection string for the [masstransit/quartz](https://hub.docker.com/r/masstransit/quartz) preconfigured Docker image, `MT_Quartz__ConnectionString` should be set to the connection string for the Quartz database.


### MT_PROMETHEUS

> `MT_PROMETHEUS=serviceName`

If present, [Prometheus](/advanced/monitoring/prometheus) metrics are enabled and exported using the specified service name. The metrics can be scraped from the service at `host:80/metrics`.

### MT_APP

> `MT_APP=/app`

By default, the MT_APP variable is set to `/app`, which is the default docker path for .NET applications that are using the platform. There typically is no reason to change this value, but it is documented for completeness.