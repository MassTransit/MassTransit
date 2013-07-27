TOSCA Example
===

This is an example demostrating different aspects of configuring MassTransit and using it to coordinate a multiple actor workflow (ordering a drink). 

Getting Started
---

 1. Install [Erlang]
 1. Install [RabbitMQ]
 1. Install RabbitMQ Management Plugins
  2. Run `rabbitmq-plugins enable rabbitmq_management` at RabbitMQ command prompt to install management tools
  2. Restart service & browse to http://localhost:15672/
  2. Login with guest : guest
 1. Open up the solution 
 1. Build

Now you should run the Barista and Cashier app before opening up an instance of a customer. You can run multiple customer apps and see what's happens when more than one orders at the same time.


  [Erlang]: http://www.erlang.org/download.html
  [RabbitMQ]: http://www.rabbitmq.com/download.html