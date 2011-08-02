Configuring Locations in your Application
"""""""""""""""""""""""""""""""""""""""""

One of the common and simple configuration questions we get center on
how to configure MassTransit in a console application vs a web application.

If you are using IoC the best place to configure MassTransit is at the same
time you are building up your IoC container.

Configuring MassTransit in a Console Application / Windowws Service
'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

I typically do this inside of the methods that build up my IoC container
and typically I do this in the ``main`` method.


Configuring MassTransit in a Website
''''''''''''''''''''''''''''''''''''

I still typically do this inside of the methods that build up my IoC
container, but instead of the main method it usually happens in the
``global.asax`` file.

.. note::

  a lot of our samples show using MT with another open source project
  Topshelf. This is a .Net Windows Service host. And should not be use
  with web sites.
