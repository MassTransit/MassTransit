# Amazon SQS

Amazon SQS includes a _DelaySeconds_ property, which can be used to defer message delivery. MassTransit uses this feature to provide _scheduled_ message delivery.

### Configuration

To configure the Amazon SQS message scheduler, see the example below.

<<< @/docs/code/scheduling/SchedulingAmazonSQS.cs

::: warning
Scheduled messages cannot be canceled when using the Amazon SQS message scheduler
:::
