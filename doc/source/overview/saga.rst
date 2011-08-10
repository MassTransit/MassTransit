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
        static CombineSaga()
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
MassTransit.NHibernateIntegration. It uses FluentNHibernate with NHibernate 3.1 currently.
