# In Memory

The in-memory transport is a great tool for testing, as it doesn't require a message broker to be installed or running. It's also very fast. But it isn't durable, and messages are gone if the bus is stopped or the process terminates. So, it's generally not a smart option for a production system. However, there are places where durability it not important so the cautionary tale is to proceed with caution.

::: warning
The in-memory transport is intended for use within a single process only. It cannot be used to communicate between multiple processes (even if they are on the same machine).
:::

## Broker Topology

The in-memory transport uses an in-memory routing fabric, that replicates a lot of the behavior of RabbitMQ. 

## Examples

### Minimal

<<< @/docs/code/transports/InMemoryBus.cs