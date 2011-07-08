Interfaces
==========

The interfaces in question.

.. sourcecode:: csharp
    
    public class Consumes<TMessage>
    {
        public interface All : IConsumer
        {
            void Consume(TMessage message);
        }
        
        public interface Selected : All
        {
            bool Accept(TMessage message);
        }
        
        public interface For<TCorrelationId> :
            All,
            CorrelatedBy<TCorrelationId>
        {
            
        }
    }

All
""""""""""""""""""""""

``Consumes<TMessage>.All``

This interface defines the ``void Consume(TMessage message)`` method

Selected
"""""""""""""""""""""""""""

``Consumes<TMessage>.Selected``

This interface defines an additional method allowing to process only selected
messages, by implementing the ``bool Accept(TMessage message)`` method.

For<TCorrelationId>
""""""""""""""""""""""""""""""""""""""

``Consumes<TMessage>.For<TCorrelationId>``

This interface defines how to do a correlated consumer.