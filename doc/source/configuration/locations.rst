Configuring Locations in your Application
"""""""""""""""""""""""""""""""""""""""""

Now that we know how to configure the MassTransit system, the next question is usually
where should I do the configuration? 

The short answer is: Configure it when you are configuring your IoC Container.

Configuring MassTransit in a Console Application / Windowws Service
'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

I typically do this inside of the methods that build up my IoC container
and typically I do this in the ``main`` method.

.. sourcecode:: csharp
    
    public class Program
    {
        public void static main(string[] args)
        {
            //configure MT
            
            //execute program logic
        }
    }


Configuring MassTransit in a Website
''''''''''''''''''''''''''''''''''''

I still typically do this inside of the methods that build up my IoC
container, but instead of the main method it usually happens in the
``global.asax`` file.

.. note::

    a lot of our samples show using MT with another open source project
    Topshelf. This is a .Net Windows Service host. And should not be use
    with web sites.

.. sourcecode:: csharp
    
    public class Global : HttpApplication
    {
        protected void Application_Start()
        {
            //configure MT
            
            //execute program logic
        }
    }
