# Courier samples

Courier is MassTransit's routing-slip implementation, which makes it possible to orchestrate distributed services into a business transaction. This sample demonstrates how to create and execute a routing slip, record routing slip events, and track transaction state using [Automatonymous](https://github.com/MassTransit/Automatonymous).

<div class="alert alert-info">
Automatonymous is a free, open-source state machine library with native MassTransit support.
</div>

## Basic Sample

This sample includes multiple console applications, which can be started simultaneously, to observe how the services interact.

 1. Clone the source down to your machine.

  `git clone https://github.com/MassTransit/Sample-Courier.git`

## Registration Sample

This sample has multiple console applications, and a web API, allowing registrations to be submitted. The routing slip is tracked using a saga, and can compensate when an activity faults.

 1. Clone the source down to your machine.

  `git clone https://github.com/phatboyg/Demo-Registration.git`
