# Templates

MassTransit includes several `dotnet new` templates to create MassTransit project and components.

A video introducing the templates is available on [YouTube](https://youtu.be/nYKq61-DFBQ).

## Installation

```sh
dotnet new -i MassTransit.Templates
```

One installed, typing `dotnet new` will display the available templates:

```
Template Name                              Short Name      Language  Tags
-----------------------------------------  --------------  --------  ---------------------------
MassTransit Consumer Saga                  mtsaga          [C#]      MassTransit/Saga
MassTransit Docker                         mtdocker        [C#]      MassTransit/Docker
MassTransit Message Consumer               mtconsumer      [C#]      MassTransit/Consumer
MassTransit Routing Slip Activity          mtactivity      [C#]      MassTransit/Activity
MassTransit Routing Slip Execute Activity  mtexecactivity  [C#]      MassTransit/ExecuteActivity
MassTransit State Machine Saga             mtstatemachine  [C#]      MassTransit/StateMachine
MassTransit Worker                         mtworker        [C#]      MassTransit/Worker
```

## Projects

### MassTransit Worker

```
dotnet new mtworker -n <YOUR NAME>
```

Creates a dotnet project that is configured as a MassTransit Worker. Includes project references and an example
`Program.cs`

### MassTransit Docker

```
dotnet new docker
```

Creates a `Dockerfile` and a `docker-compose.yml` in the project, configured for RabbitMQ.


## Items

### MassTransit Consumer

```
dotnet new mtconsumer
```

Creates a Consumer and ConsumerDefinition in `~/Consumers` and an example message in `~/Contracts`.

### MassTransit Saga State Machine

```
dotnet new mtstatemachine
```

Creates a StateMachine Saga in `~/StateMachines` and an example event in `~/Contracts`


### MassTransit Consumer Saga

```
dotnet new mtsaga
```

Creates a Saga and SagaDefinition in `~/Sagas`, along with a few messages in the `~/Contracts` folder that will
work the saga.

### MassTransit Routing Slip Activity

```
dotnet new mtactivity
```

Creates an Activity, ActivityArguments, and ActivityLog in `~/Activities`

### MassTransit Routing Slip Execute Activity

```
dotnet new mtexecactivity
```

Creates an Activity, ActivityArguments in `~/Activities`

