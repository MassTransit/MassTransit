What is a Control Bus?
""""""""""""""""""""""

The control bus concept allows 'control' messages to skip ahead of 'data' messages. When you fire
up a ServiceBus instance at ``msmq://localhost/servicea`` you will also get a bus instance at 
``msmq://localhost/servicea_control`` (similar approaches are taken for RabbitMQ and ActiveMQ 
though the pattern may be different). This queue is monitored by your instance of the IServiceBus
on the 'ControlBus' property. Control messages are not transactional and are more about information
passing and management issues. They should not be used for business critical communication, they are
usually temporary in nature (if your instance isn't there it doesn't get control messages) and are
usually purged on start up.

Inspiration: 

  http://www.eaipatterns.com/ControlBus.html