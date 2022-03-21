# Amazon SQS

> [MassTransit.AmazonSQS](https://nuget.org/packages/MassTransit.AmazonSQS/)

MassTransit combines Amazon SQS (Simple Queue Service) with SNS (Simple Notification Service) to provide both send and publish support.

Configuring a receive endpoint will use the message topology to create and subscribe SNS topics to SQS queues so that published messages will be delivered to the receive endpoint queue.

## Minimal Example 

In the example below, the Amazon SQS settings are configured.

<<< @/docs/code/transports/AmazonSqsConsoleListener.cs

## Broker Topology

With SQS/SNS, which supports topics and queues, messages are _sent_ or _published_ to SNS Topics and then routes those messages through subscriptions to the appropriate SQS Queues.

When the bus is started, MassTransit will create SNS Topics and SQS Queues for the receive endpoint.

## Configuration

The configuration includes:

* The Amazon SQS host
  - Region name: `us-east-2`
  - Access key and secret key used to access the resources

## Additional Examples

Any topic can be subscribed to a receive endpoint, as shown below. The topic attributes can also be configured, in case the topic needs to be created.

<<< @/docs/code/transports/AmazonSqsReceiveEndpoint.cs

## Errata

### Scoping

Because there is only ever one "SQS/SNS" per AWS account it can be helpful to "Scope" your queues and topics.

<<< @/docs/code/transports/AmazonSqsScopedConsoleListener.cs

### Example IAM Policy

```json
{
    "Version": "2012-10-17",
    "Statement": [
        {
            "Sid": "SqsAccess",
            "Effect": "Allow",
            "Action": [
                "sqs:SetQueueAttributes",
                "sqs:ReceiveMessage",
                "sqs:CreateQueue",
                "sqs:DeleteMessage",
                "sqs:SendMessage",
                "sqs:GetQueueUrl",
                "sqs:GetQueueAttributes",
                "sqs:ChangeMessageVisibility",
                ],
            "Resource": "arn:aws:sqs:*:YOUR_ACCOUNT_ID:*"
        },{
            "Sid": "SnsAccess",
            "Effect": "Allow",
            "Action": [
                "sns:GetTopicAttributes",
                "sns:CreateTopic",
                "sns:Publish",
                "sns:Subscribe"
            ],
            "Resource": "arn:aws:sns:*:YOUR_ACCOUNT_ID:*"
        },{
            "Sid": "SnsListAccess",
            "Effect": "Allow",
            "Action": [
                "sns:ListTopics"
            ],
            "Resource": "*"
        }
    ]
}
```