Creating Your First Consumer
============================

To create your first consumer, create a new project using Visual Studio. The project should be configured to use .NET 4.0, and should be created as a class library. In this exercise, the project name is LearningMT.Consumer.

.. image:: images/newConsumerProject.png
   :alt: create new project
   :align: left

Once the project is created, delete the Class1.cs file which was automatically created.

A reference to MassTransit must now be added, which you will do using the NuGet package manager. To add the reference, right-click on the project References and select *Manage NuGet Packages* from the context menu.

.. image:: images/addNuGetReference.png

Select the *Online* section on the left, and enter *MassTransit* into the search box. Once the results are displayed, select the *MassTransit.RabbitMQ* package and click Install. This will install the RabbitMQ transport, and its dependencies, which include MassTransit itself. You many have to click the license acceptance box to continue.

.. image:: images/onlineNuGetPackage.png

Once the package manager is finished downloading the package and adding the references, close the package manager and verify that the references have been added to your project.

.. image:: images/consumerReferences.png

Using MassTransit, there are several ways that message subscriptions can be added to a service bus instance. In this exercise, you will be creating a message *consumer*. Other types of message subscriptions include message *handlers*, consumer *instances*, and *sagas*.

Before creating your consumer, however, you must first define a message. Using MassTransit, messages should be defined using C# interfaces. To create a message, add a new interface to the project called *SubmitOrder*.

.. sourcecode:: csharp
    :linenos:

    using System;
    using MassTransit;

    public interface SubmitOrder :
        CorrelatedBy<Guid>
    {
        DateTime SubmitDate { get; }
        string CustomerNumber { get; }
        string OrderNumber { get; }
    }

The interface defines the message contract that the consumer accepts. In this exercise, you are creating a service that accepts the *SubmitOrder* command. Command message contracts are defined and owned by the service and specify the input requirements for the service.

The properties on the interface are defined with getters only, because message contents are *immutable*. The interface extends the CorrelatedBy interface, specifying that a Guid should be used for message correlation. Correlation allows the message to be tracked from the message produder to the message consumer.

