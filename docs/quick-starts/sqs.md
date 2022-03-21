---
prev: false
next: /usage/configuration
sidebarDepth: 0
---

# SQS

> This tutorial will get you from zero to up and running with [AWS SQS](/usage/transports/amazonsqs) and MassTransit. 

<iframe id="ytplayer" type="text/html" width="640" height="360"
  src="https://www.youtube.com/embed/ziBedZAOazA?autoplay=0">
</iframe>

- The source for this sample is available [on GitHub](https://github.com/MassTransit/Sample-GettingStarted).


## Prerequisites

::: tip NOTE
The following instructions assume you are starting from a completed [In-Memory Quick Start](/quick-starts/in-memory)
:::

This example requires the following:

- a functioning installation of the dotnet runtime and sdk (at least 6.0)
- an AWS account, where you have the ability to control the IAM permissions for SQS and SNS

## Setup AWS

1. Log into AWS
2. Create a User with `Access key - Programmatic access`
      1. Grant the user the `Sample IAM Policy` below


### Sample IAM Policy

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
                "sqs:ChangeMessageVisibility"
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

## Change the Transport to AmazonSQS

Add the _MassTransit.AmazonSQS_ package to the project.

```bash
$ dotnet add package MassTransit.AmazonSQS
```

## Edit Program.cs

Change `UsingInMemory` to `UsingAmazonSQS`

```csharp {8-16}
public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureServices((hostContext, services) =>
        {
            services.AddMassTransit(x =>
            {
                // elided ...
                x.UsingAmazonSqs((context, cfg) =>
                {
                    cfg.Host("us-east-1", h => {
                        h.AccessKey("your-iam-access-key");
                        h.SecretKey("your-iam-secret-key");
                    });

                    cfg.ConfigureEndpoints(context);
                });
            });

            services.AddHostedService<Worker>();
        });
```

### Run the project

```bash
$ dotnet run
```

The output should have changed to show the message consumer generating the output (again, press Control+C to exit). Notice that the bus address now starts with `amazonsqs`.

``` {11}
Building...
info: MassTransit[0]
      Configured endpoint Message, Consumer: GettingStarted.MessageConsumer
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
info: Microsoft.Hosting.Lifetime[0]
      Hosting environment: Development
info: Microsoft.Hosting.Lifetime[0]
      Content root path: /Users/chris/Garbage/start/GettingStarted
info: MassTransit[0]
      Bus started: amazonsqs://us-east-1/a-topic-name
info: GettingStarted.MessageConsumer[0]
      Received Text: The time is 3/24/2021 12:11:10 PM -05:00
```

At this point the service is connecting to Amazon SQS/SNS in the region `us-east-1` and publishing messages which are received by the consumer.

:tada: