namespace MassTransit.Saga.Tests
{
    using System;
    using System.Collections;
    using MassTransit.ServiceBus.Tests;
    using MassTransit.Workflow.Tests;
    using NUnit.Framework;
    using ServiceBus;

    [TestFixture]
    public class When_a_workflow_related_service_is_setting_up_the_container :
        Specification
    {
        [Test]
        public void The_service_should_register_the_supported_messages()
        {
            IWorkflowCoordinator coordinator = DynamicMock<IWorkflowCoordinator>();


            // we actually bind a service to a workflow so this need to flip

            // the service consumes messages like any other component, except
            // that when the service is bound to the workflow there are additional
            // things around it to make it happen within the context of the workflow
            // symantics

            // at this point, the workflow dispatcher should somehow handle the following:

            // 1. Add a subscription for a WorkflowComponent that will consume the message on behalf of the service
            // 2. teh WorkflowComponent<T> will retrieve the workflow from the repository and add it to the injection dictionary
            // 3. the WorkflowComponent<T> will Resolve<service> from the container and inject the workflow instance into the service
            // 4. the message will be dispatched to the service
            // 5. the service will do its thing and update the workflow as necessary
            // 6. the WorkflowComponent<T> will then update the workflow in the repository
            // 7. the WorkflowComponent<T> will handle transaction scope in regards to the message and database?

        }
    }

    public class WorkflowComponent<TWorkflow, TComponent, TMessage> : 
        IWorkflowComponent,
        Consumes<TMessage>.All
        where TMessage : class, CorrelatedBy<Guid>
        where TWorkflow : class, CorrelatedBy<Guid>
        where TComponent : class, Consumes<TMessage>.All
    {
        private readonly IWorkflowRepository _repository;
        private readonly IObjectBuilder _objectBuilder;

        // it is expected that the IWorkflowRepository will be built by a factory the will 
        // invoke a unit of work/session for the repository -- perhaps

        public WorkflowComponent(IWorkflowRepository repository, IObjectBuilder objectBuilder)
        {
            _repository = repository;
            _objectBuilder = objectBuilder;
        }

        public void Consume(TMessage message)
        {
            TWorkflow workflow = _repository.Load<TWorkflow>(message.CorrelationId);

            Hashtable arguments = new Hashtable();
            arguments.Add("workflow", workflow);

            Consumes<TMessage>.All consumer = _objectBuilder.Build<TComponent>(arguments);

            consumer.Consume(message);
        }
    }

    public interface IWorkflowRepository
    {
        TWorkflow Load<TWorkflow>(Guid workflowId) where TWorkflow : class, CorrelatedBy<Guid>;
    }

    public interface IWorkflowComponent
    {
    }

    [Serializable]
    [Reliable]
    public class UserEmailSent :
        CorrelatedBy<Guid>
    {
        private readonly Guid _correlationId;

        public UserEmailSent(Guid correlationId)
        {
            _correlationId = correlationId;
        }

        public Guid CorrelationId
        {
            get { return _correlationId; }
        }
    }
}