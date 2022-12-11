---
repo: "MassTransit/Sample-JobConsumers"
useTitle: false
youtube: abc
---

MassTransit includes a job service that keeps track of each job, assigns jobs to service instances, and schedules job retries when necessary. The job service uses three saga state machines and the default configuration uses an in-memory saga repository, which is not durable. When using job consumers for production use cases, configuring durable saga repositories is highly recommended to avoid possible message loss. Check out the sample project on GitHub, which includes the Entity Framework configuration for the job service state machines.
