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

### MassTransit Docker

Creates a simple docker file