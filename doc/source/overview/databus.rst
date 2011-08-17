What is the Data Bus?
"""""""""""""""""""""

The data bus is the instance of IServiceBus that you will work with day in and day out. It is the 
instance that listens at ``msmq://localhost/servicea`` that you configured at start up. This is
typically the one you want set to transactional queues and want to be careful about just randomly
purging.