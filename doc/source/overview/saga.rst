Defining Sagas using the Saga State Machine
===========================================

Sagas are one of the more powerful features in MassTransit, allowing complex state and behavior to be
defined using a fluent syntax.


What is a saga?
---------------

A saga is a long-lived transaction managed by a coordinator. Sagas are initiated by an event, sagas orchestrates events, and sagas maintain the state of the overall transaction. They are designed to manage the complexity of a distributed transaction without locking and immediate consistency. They manage state, and track compensations that are required if a partial failure occurs.

We didn’t create it, we learned it from:

Cornell paper: http://www.cs.cornell.edu/andru/cs711/2002fa/reading/sagas.pdf

Arnon Rotem-Gal-Oz’s book chapter: http://www.rgoarchitects.com/Files/SOAPatterns/Saga.pdf


Defining a Saga
---------------

There are two ways to define a saga using MassTransit. The first approach is similar to creating a _consumer_
and uses interfaces on a class to define the messages that can initiate, orchestrate, or be observed by a saga
instance. The second approach creates a state machine from a class definition that defines the events, states,
and actions that make up the state machine.


Defining a Saga Using the State Machine Syntax
----------------------------------------------

To define a saga using the state machine, a class that inherits from SagaStateMachine must be created.


.. sourcecode:: csharp
    :linenos:
    
    public class AuctionSaga : 
        SagaStateMachine<AuctionSaga>,
        ISaga
    {
        static AuctionSaga()
        {
            Define(() =>
            {
                // the state machine behavior is defined here
            });
        }
        public Guid CorrelationId { get; set; }
        public IServiceBus Bus { get; set; }
    }
    
Shown above is an empty definition of a saga state machine. This is just the start, however, as there
is much more to be done. The CorrelationId is the Guid assigned to the saga when it is created. The _IServiceBus_ 
property is set before any methods on the saga instance are called, allowing it to be used by the event handlers
defined in the saga.

First, we need to declare the valid states for the saga. There are two predefined states, _Initial_ and _Completed_,
both of which must be included in the saga definition. Any states required by the saga also need to be added. Some
example states are shown below.

.. sourcecode:: csharp
    :linenos:

    public static State Initial { get; set; }
    public static State Completed { get; set; }
    public static State Open { get; set; }
    public static State Closed { get; set; }
    
As you see, the states are added as public static properties of type _State_. This allows the states to be
used in code as properties, instead of relying on strings or other symbols.

Now, let's define some events to go along with those states.

.. sourcecode:: csharp
    :linenos:

    public static Event<CreateAuction> Create { get; set; }
    public static Event<PlaceBid> Bid { get; set; }
    
Just like states, events are defined as public static properties on the saga class. The generic type
specified for the event is the message type associated with the event. When the saga is subscribed to the bus,
the message types for the events are subscribed.

The messages need to be linked to the saga instance in some way so the proper messages are delivered. The messages
are shown below.

.. sourcecode:: csharp
    :linenos:

    public interface CreateAuction :
        CorrelatedBy<Guid>
    {
        string Title { get; }
        string OwnerEmail { get; }
        decimal OpeningBid { get; }
    }
    
When an auction is created, a CreateAuction command is sent to the endpoint where the saga is subscribed. Since the 
message is correlated by Guid, the CorrelationId of the message will be used as the CorrelationId of the saga by default (this can be overridden as well).

.. sourcecode:: csharp
    :linenos:

    public interface PlaceBid
    {
        Guid BidId { get; }
        Guid AuctionId { get; }
        decimal MaximumBid { get; }
        string BidderEmail { get; }
    }

For the bid message, we want to have a unique identifier for the bid, so we have a BidId on the message. We also
need the AuctionId so that the message can be delivered to the proper saga instance. 

Now that we have defined the messages that are associated with the events defined in the saga, we need to 
specify the behavior of how and when those events can be handled. To define the behavior, we need to add
code to the Define call in the static initializer of the saga class as shown.

.. sourcecode:: csharp
    :linenos:

    static AuctionSaga()
    {
        Define(() =>
        {
            Initially(
                When(Create));
            During(Open,
                When(Bid));
        });
    }

The linkage above is pretty simple, but it defines some important characteristics of the saga. First, based
on the definition above, we can see that the Create event is only accepted when the saga is in the _Initial_
state (which is the default for newly created saga instances). When an event is handled in the initial state,
a message for which there is not an existing saga will create a new saga instance.

*A saga instance can only be created by events that appear in the Initially section.*

_NOTE: Initially() is an alias that is equivalent to specifying During(Initial)._

The During statement defines the events that are accepted in the state specified. In this case, the Bid event
is allowed while the saga is in the Open State. Since the Bid event is not accepted in the Initial state, it
cannot be used to create a new saga and will result in an error being logged (which should move the message to
the error queue and publish a Fault<PlaceBid> message in response to the command).

The Bid event is a special case, however, since the message is not correlated by a Guid. In order to deliver
the message to the proper saga instance, we need to define the relationship between the message and the saga.
This is done using the Correlate method, as shown below.

.. sourcecode:: csharp
    :linenos:

    static AuctionSaga()
    {
        Define(() =>
        {
            Correlate(Bid)
                .By((saga,message) => saga.CorrelationId == message.AuctionId);
        });
    }

By defining the correlation, the proper filter expressions are created to load the existing saga instance
for the message. It is important to realize that these translate directly into LINQ expressions that are
passed to the saga repository for loading the saga instance, so depending upon your repository implementation
you may have to tweak the syntax to get the proper result for your database provider. In most cases, a one-to-one
relationship as shown above is your best bet.

*NOTE: Since the CreateAuction message is correlated by Guid, the default correlation is used.*

Now we need to define some behavior to happen when the events occur. We've already defined the events, we 
just need to link up some behavior.

.. sourcecode:: csharp
    :linenos:

    static AuctionSaga()
    {
        Define(() =>
        {
            Initially(
                When(Create)
                    .Then((saga,message) => 
                    {
                        saga.OpeningBid = message.OpeningBid;
                        saga.OwnerEmail = message.OwnerEmail;
                        saga.Title = message.Title;
                    })
                    .TransitionTo(Open));
        });
    }    
    //
    public decimal OpeningBid { get; set; }
    public string OwnerEmail { get; set; }
    public string Title { get; set; }
    
Two simple behavior steps have been defined above. The first, an anonymous method called with the saga instance
and the message, initializes some properties on the saga. The second transitions the state of the saga to Open.
Properties were also added to store the auction details that were provided in the CreateAuction message.

.. sourcecode:: csharp
    :linenos:

    static AuctionSaga()
    {
        Define(() =>
        {
            During(Open,
                When(Bid)
                    .Call((saga,message) => saga.Handle(message),
                        InCaseOf<UnderBidException>()
                            .Publish((saga,message,ex) => new OutBid(message.BidId))));
        });
    }
    void Handle(PlaceBid bid)
    {
        if(!CurrentBid.HasValue || bid.MaximumBid > CurrentBid)
        {
            if(HighBidder != null)
            {
                Bus.Publish(new Outbid(HighBidId));
            }
            CurrentBid = bid.MaximumBid;
            HighBidder = bid.BidderEmail;
            HighBidId = bid.BidId;
        }
        else
        {
            throw new UnderBidException();
        }
    }
    //
    public decimal? CurrentBid { get; set; }
    public string HighBidder { get; set; }
    public Guid HighBidId { get; set; }

Above, the behavior for accepting a bid is defined. If the bid received is higher than the current bid,
the current bid is updated and the high bidder information is stored with the saga instance. If there was a high
bidder, a message is published indicating the a previous bidder was outbid, allowing actions to be taken such as
sending an email to the previous high bidder. If the new bid is too low, and exception is thrown which is caught by the 
InCaseOf method. This specifies an exception handler for the Call method. Multiple exception handlers can be specified and they are evaluated in a chain-of-command order where the first one that matches the type (IsAssignableFrom) is invoked.

The use of the Bus property is also demonstrated in the Handle method, as it is used to publish the outbid message.


Combining Events (think Fork/Join)
----------------------------------

In some cases, you may want to create a saga that orchestrates several child sagas or initiate multiple concurrent commands
and continue processing once all of the commands have been acknowledged. This can be done using a clever construct known as Combine(). For example, the saga below sends two requests and handles the response to each request separately. An additional Combine statement signifies that the two events have completed and triggers a third event on the saga instance.

.. sourcecode:: csharp
    :linenos:

    static SupervisorSaga()
    {
        Define(() =>
        {
            Initially(
                When(Create)
                    .Then((saga,message) => 
                    {
                        saga.PostalCode = message.PostalCode;
                    })
                    .Publish((saga,message) => new RequestPostalCodeDetails(saga.PostalCode))
                    .Publish((saga,message) => new RequestGeolocation(saga.PostalCode))
                    .TransitionTo(Waiting));
                    
            During(Waiting,
                When(PostalCodeDetailsReceived)
                    .Then((saga,message) => 
                    {
                        saga.City = message.City;
                        saga.State = message.State;
                    }),
                When(GeolocationReceived)
                    .Then((saga,message) =>
                    {
                        saga.Latitude = message.Latitude;
                        saga.Longitude = message.Longitude;
                    }));
                    
            Combine(PostalCodeDetailsReceived, GeolocationReceived)
                .Into(ReadyToProceed, saga => saga.ReadyFlags);
                
            During(Waiting,
                When(ReadyToProceed)
                    .Then((saga,message) =>
                    {
                        saga.Bus.Publish(new PostalCodeDetails(...));
                    })
                    .Complete());
        });
    }
    //
    public int ReadyFlags { get; set; }
    public static Event<CreatePostalCodeDetailsRequest> Create { get; set; }
    public static Event<PostalCodeDetailsResponse> PostalCodeDetailsReceived { get; set; }
    public static Event<GeolocationResponse> GeolocationReceived { get; set; }
    public static Event ReadyToProceed { get; set; }

The combine method declares a set of events that must be triggered before the combined event is triggered. In
this case, the ReadyToProceed event is fired when the two separate result messages have both been received.
The reception and handling of those messages is done separately as each individual response is received.

This is a pretty simple example of the saga, but with this great power comes great responsibility.

(and with that, I'm too tired to continue for now and must rest)

Subscribing to the Saga
-----------------------

Once the saga has been defined, it is subscribed to the bus using the Saga subscription method.

.. sourcecode:: csharp
    :linenos:

    public class Program
    {
        public static void Main()
        {
            Bus.Initialize(sbc =>
            {
                sbc.ReceiveFrom("loopback://localhost/my_saga_bus");
                sbc.Subscribe(subs =>
                {
                    subs.Saga<AuctionSaga>(new InMemorySagaRepository<AuctionSaga>())
                        .Permanent();
                });
            });
        }
    }

NOTE: The example above uses an in-memory transport and saga repository, which is not durable. It is shown
for testing purposes only. There is a library for use with NHibernate provided with MassTransit, called
MassTransit.NHibernateIntegration. It uses FluentNHibernate with NHibernate currently.
