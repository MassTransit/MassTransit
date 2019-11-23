# Amazon SQS

MassTransit combines Amazon SQS (Simple Queue Service) with SNS (Simple Notification Service) to provide both send and publish support.

Configuring a receive endpoint will use the message topology to create and subscribe SNS topics to SQS queues so that published messages will be delivered to the receive endpoint queue.

```csharp
Bus.Factory.CreateUsingAmazonSqs(x =>
{
    string region = "us-east-2";
    string accessKey = "your-iam-access-key";
    string secretKey = "your-iam-secret-key";

    x.Host(region, h =>
    {
        h.AccessKey(accessKey);
        h.SecretKey(secretKey);
    });

    x.ReceiveEndpoint("input-queue", e =>
    {
        // Default is true, change to false to prevent SNS topic subscription configuration
        e.SubscribeMessageTopics = true;

        e.Consumer(() => new MyConsumer());
    });
});
```


Any topic can be subscribed to a receive endpoint, as shown below. The topic attributes can also be configured, in case the topic needs to be created.

```csharp
Bus.Factory.CreateUsingAmazonSqs(x =>
{
    x.ReceiveEndpoint("input-queue", e =>
    {
        e.SubscribeMessageTopics = false;
        e.Subscribe("event-topic", x =>
        {
            x.TopicAttributes["some-attribute"] = "some-value";
            x.TopicSubscriptionAttributes["some-subscription-attribute"] = "some-attribute-value";
            x.TopicTags.Add("environment", "development");
        });

        e.Consumer(() => new MyConsumer());
    });
});
```