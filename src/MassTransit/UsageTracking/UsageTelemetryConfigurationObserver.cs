namespace MassTransit.UsageTracking;

using System;
using System.Collections.Generic;
using System.Linq;
using JobService;
using UsageTelemetry;


public class UsageTelemetryConfigurationObserver :
    IConsumerConfigurationObserver,
    ISagaConfigurationObserver,
    IActivityConfigurationObserver,
    IHandlerConfigurationObserver,
    IUsageTelemetrySource
{
    readonly EndpointUsageTelemetry _endpointUsageTelemetry;

    public UsageTelemetryConfigurationObserver(IConsumePipeConfigurator configurator, EndpointUsageTelemetry endpointUsageTelemetry)
    {
        _endpointUsageTelemetry = endpointUsageTelemetry;

        ConsumerTypes = [];
        JobConsumerTypes = [];
        HandlerTypes = [];
        MessageTypes = [];
        SagaTypes = [];
        SagaStateMachineTypes = [];
        ExecuteActivityTypes = [];
        ActivityTypes = [];
        CompensateActivityTypes = [];

        configurator.ConnectConsumerConfigurationObserver(this);
        configurator.ConnectSagaConfigurationObserver(this);
        configurator.ConnectHandlerConfigurationObserver(this);
        configurator.ConnectActivityConfigurationObserver(this);
    }

    HashSet<Type> HandlerTypes { get; }
    HashSet<Type> JobConsumerTypes { get; }
    HashSet<Type> MessageTypes { get; }
    HashSet<Type> ConsumerTypes { get; }
    HashSet<Type> SagaTypes { get; }
    HashSet<(Type sagaType, Type instanceType)> SagaStateMachineTypes { get; }
    HashSet<(Type activityType, Type argumentType)> ExecuteActivityTypes { get; }
    HashSet<(Type activityType, Type argumentType, Uri compensateAddress)> ActivityTypes { get; }
    HashSet<(Type activityType, Type logType)> CompensateActivityTypes { get; }

    public void ActivityConfigured<TActivity, TArguments>(IExecuteActivityConfigurator<TActivity, TArguments> configurator, Uri compensateAddress)
        where TActivity : class, IExecuteActivity<TArguments>
        where TArguments : class
    {
        ActivityTypes.Add((typeof(TActivity), typeof(TArguments), compensateAddress));
    }

    public void ExecuteActivityConfigured<TActivity, TArguments>(IExecuteActivityConfigurator<TActivity, TArguments> configurator)
        where TActivity : class, IExecuteActivity<TArguments>
        where TArguments : class
    {
        ExecuteActivityTypes.Add((typeof(TActivity), typeof(TArguments)));
    }

    public void CompensateActivityConfigured<TActivity, TLog>(ICompensateActivityConfigurator<TActivity, TLog> configurator)
        where TActivity : class, ICompensateActivity<TLog>
        where TLog : class
    {
        CompensateActivityTypes.Add((typeof(TActivity), typeof(TLog)));
    }

    public void ConsumerConfigured<TConsumer>(IConsumerConfigurator<TConsumer> configurator)
        where TConsumer : class
    {
        ConsumerTypes.Add(typeof(TConsumer));
    }

    public void ConsumerMessageConfigured<TConsumer, TMessage>(IConsumerMessageConfigurator<TConsumer, TMessage> configurator)
        where TConsumer : class
        where TMessage : class
    {
        if (typeof(TConsumer) == typeof(SubmitJobConsumer<TMessage>))
            JobConsumerTypes.Add(typeof(TMessage));

        MessageTypes.Add(typeof(TMessage));
    }

    public void HandlerConfigured<TMessage>(IHandlerConfigurator<TMessage> configurator)
        where TMessage : class
    {
        HandlerTypes.Add(typeof(TMessage));
    }

    public void SagaConfigured<TSaga>(ISagaConfigurator<TSaga> configurator)
        where TSaga : class, ISaga
    {
        if (SagaStateMachineTypes.All(x => x.instanceType != typeof(TSaga)))
            SagaTypes.Add(typeof(TSaga));
    }

    public void StateMachineSagaConfigured<TInstance>(ISagaConfigurator<TInstance> configurator, SagaStateMachine<TInstance> stateMachine)
        where TInstance : class, ISaga, SagaStateMachineInstance
    {
        SagaStateMachineTypes.Add((stateMachine.GetType(), typeof(TInstance)));

        SagaTypes.Remove(typeof(TInstance));
    }

    public void SagaMessageConfigured<TSaga, TMessage>(ISagaMessageConfigurator<TSaga, TMessage> configurator)
        where TSaga : class, ISaga
        where TMessage : class
    {
        MessageTypes.Add(typeof(TMessage));
    }

    public void Update()
    {
        _endpointUsageTelemetry.HandlerCount = HandlerTypes.Count;
        _endpointUsageTelemetry.ConsumerCount = ConsumerTypes.Count;
        _endpointUsageTelemetry.JobConsumerCount = JobConsumerTypes.Count;
        _endpointUsageTelemetry.SagaCount = SagaTypes.Count;
        _endpointUsageTelemetry.SagaStateMachineCount = SagaStateMachineTypes.Count;
        _endpointUsageTelemetry.ActivityCount = ActivityTypes.Count;
        _endpointUsageTelemetry.ExecuteActivityCount = ExecuteActivityTypes.Count;
        _endpointUsageTelemetry.CompensateActivityCount = CompensateActivityTypes.Count;
        _endpointUsageTelemetry.MessageCount = MessageTypes.Count;
    }
}
