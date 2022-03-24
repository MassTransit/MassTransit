# Templates

MassTransit comes with a variety of dotnet templates to help facilitate the development of MassTransit services. A video introducing the templates is available on [YouTube](https://youtu.be/nYKq61-DFBQ).

## Install the dotnet templates

```sh
dotnet new -i MassTransit.Templates
```

## Templates Provided

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
### MassTransit Consumer Saga

```
dotnet new mtsaga
```

Creates a Saga and SagaDefinition in `~/Sagas`, along with a few messages in the `~/Contracts` folder that will
work the saga.

### MassTransit Docker

```
dotnet new docker
```

Creates a `Dockefile` and a `docker-compose.yml` in the project, configured for RabbitMQ.

### MassTransit Message Consumer

```
dotnet new mtconsumer
```

Creates a Consumer and ConsumerDefinition in `~/Consumers` and an example message in `~/Contracts`.

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

### MassTransit State Machine Saga

```
dotnet new mtstatemachine
```

Creates a StateMachine Saga in `~/StateMachines` and an example event in `~/Contracts`

### MassTransit Worker

```
dotnet new mtworker -n <YOUR NAME>
```

Creates a dotnet project that is configured as a MassTransit Worker. Includes project references and an example
`Program.cs`
