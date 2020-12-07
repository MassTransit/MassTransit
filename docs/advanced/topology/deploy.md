# Deploy Topology

There are some scenarios, such as when using Azure Functions, where it may be necessary to deploy the topology to the broker separately, without actually starting the service (and thereby consuming messages). To support this, MassTransit has a `DeployTopologyOnly` flag that can be specified when configuring the bus. When used with the `DeployAsync` method, a simple console application can be created that creates all the exchanges/topics, queues, and subscriptions/bindings.

To deploy the broker topology using a console application, see the example below.

<<< @/docs/code/containers/MicrosoftDeployTopology.cs




