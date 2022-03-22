# Amazon SQS

> [MassTransit.AmazonSQS](https://nuget.org/packages/MassTransit.AmazonSQS/)

MassTransit combines Amazon SQS (Simple Queue Service) with SNS (Simple Notification Service) to provide both send and publish support.

Configuring a receive endpoint will use the message topology to create and subscribe SNS topics to SQS queues so that published messages will be delivered to the receive endpoint queue.

## Example 

In the example below, the Amazon SQS settings are configured.

<<< @/docs/code/transports/AmazonSqsConsoleListener.cs

The configuration includes:

* The Amazon SQS host
  - Region name: `us-east-2`
  - Access key and secret key used to access the resources

Any topic can be subscribed to a receive endpoint, as shown below. The topic attributes can also be configured, in case the topic needs to be created.

<<< @/docs/code/transports/AmazonSqsReceiveEndpoint.cs

## Scoping

Because there is only ever one "SQS/SNS" per AWS account it can be helpful to "Scope" your queues and topics.

<<< @/docs/code/transports/AmazonSqsScopedConsoleListener.cs

## Example IAM Policy

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
                "sqs:SendMessageBatch",
                "sqs:GetQueueUrl",
                "sqs:GetQueueAttributes",
                "sqs:ChangeMessageVisibility",
                "sqs:ChangeMessageVisibilityBatch"
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
            "Resource": "arn:aws:sns:*:YOUR_ACCOUNT_ID:*"
        }
    ]
}
```