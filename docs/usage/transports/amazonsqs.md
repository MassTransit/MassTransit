# Amazon SQS

> [MassTransit.AmazonSQS](https://nuget.org/packages/MassTransit.AmazonSQS/)

MassTransit combines Amazon SQS (Simple Queue Service) with SNS (Simple Notification Service) to provide both send and publish support.

Configuring a receive endpoint will use the message topology to create and subscribe SNS topics to SQS queues so that published messages will be delivered to the receive endpoint queue.

In the example below, the Amazon SQS settings are configured.

<<< @/docs/code/transports/AmazonSqsConsoleListener.cs

The configuration includes:

* The Amazon SQS host
  - Region name: `us-east-2`
  - Access key and secret key used to access the resources

Any topic can be subscribed to a receive endpoint, as shown below. The topic attributes can also be configured, in case the topic needs to be created.

<<< @/docs/code/transports/AmazonSqsReceiveEndpoint.cs

Required AWS permissions:
* _sqs:GetQueueAttributes_
* _sqs:SetQueueAttributes_
* _sqs:ReceiveMessage_
* _sqs:CreateQueue_
* _sqs:DeleteMessage_
* _sqs:SendMessage_
* _sns:GetTopicAttributes_
* _sns:CreateTopic_
* _sns:Publish_
* _sns:Subscribe_
* _sns:ListTopics_
