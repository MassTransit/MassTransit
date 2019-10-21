# Displaying configuration

A bus instance is composed of many classes, all of which are wired together to form a connection pipeline of
message processing goodness. This brings a bit of complexity, as there are many moving parts behind the curtain.
To help troubleshoot and understand how a bus is configured, it is possible to probe the bus and return an object
graph of the bus.

To probe bus configuration, use the `GetProbeResult` method as shown below.

```csharp
var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
{
    var host = cfg.Host(new Uri("rabbitmq://localhost/test"), h =>
    {
        h.Username("guest");
        h.Password("guest");
    });

    sbc.ReceiveEndpoint("input_queue", ec =>
    {
        ec.Consumer<UpdateCustomerAddressConsumer>();
    })
});

ProbeResult result = busControl.GetProbeResult();

Console.WriteLine(result.ToJsonString());
```

The resulting output for the configuration above would be similar to the following.

```json
{
  "resultId": "7f280000-2961-000c-1dcc-08d2c68a08ac",
  "probeId": "7f280000-2961-000c-fd27-08d2c68a08ab",
  "startTimestamp": "2015-09-26T15:49:16.594521Z",
  "duration": "00:00:00.4850036",
  "host": {
    "machineName": "LOCALHOST",
    "processName": "TestService",
    "processId": 5808,
    "assembly": "MassTransit",
    "assemblyVersion": "3.0.13.0",
    "frameworkVersion": "4.0.30319.42000",
    "massTransitVersion": "3.0.13.0",
    "operatingSystemVersion": "Microsoft Windows NT 6.3.9600.0"
  },
  "results": {
    "bus": {
      "address": "rabbitmq://[::1]:5672/test/bus-testservice-xhwyyybjcryy3ofjbdjcpnoenx?durable=false&autodelete=true&prefetch=8",
      "host": {
        "type": "RabbitMQ",
        "host": "[::1]",
        "port": 5672,
        "virtualHost": "test",
        "username": "guest",
        "password": "*****",
        "connected": true
      },
      "receiveEndpoint": [
        {
          "transport": {
            "type": "RabbitMQ",
            "queueName": "input_queue",
            "exchangeName": "input_queue",
            "prefetchCount": 16,
            "durable": true,
            "queueArguments": {},
            "exchangeArguments": {},
            "purgeOnStartup": true,
            "exchangeType": "fanout",
            "bindings": [
              {
                "exchange": {
                  "exchangeName": "TestService.Contracts:UpdateCustomerAddress",
                  "exchangeType": "fanout",
                  "durable": true,
                  "arguments": {}
                },
                "routingKey": "",
                "arguments": {}
              }
            ]
          },
          "filters": [
            {
              "filterType": "deadLetter",
              "filters": {
                "filterType": "move",
                "destinationAddress": "rabbitmq://[::1]:5672/test/input_queue_skipped?bind=true&queue=input_queue_skipped"
              }
            },
            {
              "filterType": "rescue",
              "filters": {
                "filterType": "moveFault",
                "destinationAddress": "rabbitmq://[::1]:5672/test/input_queue_error?bind=true&queue=input_queue_error"
              }
            },
            {
              "filterType": "deserialize",
              "deserializers": {
                "json": {
                  "contentType": "application/vnd.masstransit+json"
                },
                "bson": {
                  "contentType": "application/vnd.masstransit+bson"
                },
                "xml": {
                  "contentType": "application/vnd.masstransit+xml"
                }
              },
              "pipe": {
                "TestService.Contracts.UpdateCustomerAddress": {
                  "filters": {
                    "filterType": "instance",
                    "type": "MassTransit.Testing.MultiTestConsumer+Of<TestService.Contracts.UpdateCustomerAddress>"
                  }
                }
              }
            }
          ]
        },
        {
          "transport": {
            "type": "RabbitMQ",
            "queueName": "bus-testservice-xhwyyybjcryy3ofjbdjcpnoenx",
            "exchangeName": "bus-testservice-xhwyyybjcryy3ofjbdjcpnoenx",
            "prefetchCount": 8,
            "autoDelete": true,
            "queueArguments": {
              "x-expires": 60000
            },
            "exchangeArguments": {
              "x-expires": 60000
            },
            "exchangeType": "fanout",
            "bindings": []
          },
          "filters": [
            {
              "filterType": "deadLetter",
              "filters": {
                "filterType": "move",
                "destinationAddress": "rabbitmq://[::1]:5672/test/bus-testservice-xhwyyybjcryy3ofjbdjcpnoenx_skipped?bind=true&queue=bus-testservice-xhwyyybjcryy3ofjbdjcpnoenx_skipped"
              }
            },
            {
              "filterType": "rescue",
              "filters": {
                "filterType": "moveFault",
                "destinationAddress": "rabbitmq://[::1]:5672/test/bus-testservice-xhwyyybjcryy3ofjbdjcpnoenx_error?bind=true&queue=bus-testservice-xhwyyybjcryy3ofjbdjcpnoenx_error"
              }
            },
            {
              "filterType": "deserialize",
              "deserializers": {
                "json": {
                  "contentType": "application/vnd.masstransit+json"
                },
                "bson": {
                  "contentType": "application/vnd.masstransit+bson"
                },
                "xml": {
                  "contentType": "application/vnd.masstransit+xml"
                }
              },
              "pipe": {}
            }
          ]
        }
      ]
    }
  }
}
```