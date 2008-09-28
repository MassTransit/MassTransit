namespace CodeCamp.Service
{
    using System;
    using System.Net.Mail;
    using Magnum.Common.Repository;
    using MassTransit.Saga;
    using MassTransit.ServiceBus;
    using MassTransit.ServiceBus.Timeout.Messages;
    using Magnum.Common.DateTimeExtensions;
    using Messages;

    public class RegisterUserOrchestration :
        InitiatedBy<RegisterUser>,
        Orchestrates<UserVerificationEmailSent>,
        Orchestrates<UserVerifiedEmail>
    {
        private readonly IServiceBus _bus;
        private readonly SmtpClient _client;
        private readonly IRepository _repository;

        public RegisterUserOrchestration(IServiceBus bus, SmtpClient client, IRepository repository)
        {
            _bus = bus;
            _client = client;
            _repository = repository;
        }

        //Starts things off
        public void Consume(RegisterUser message)
        {
            var saga = new RegisterUserSaga(Guid.NewGuid(), message);
            _repository.Save(saga);

            var body = string.Format("Please verify email http://localhost/ConfirmEmail.aspx?registrationId={0}", saga.Id);
            _client.Send("bob", "dru", "Verify Email", body);

            _bus.Publish(new UserVerificationEmailSent(saga.Id));
        }

        public void Consume(UserVerificationEmailSent message)
        {
            var saga = _repository.Get<RegisterUserSaga>(message.CorrelationId);
            saga.SetPending();
            _repository.Save(saga);

            _bus.Publish(new ScheduleTimeout(message.CorrelationId, 24.Hours().FromNow()));
        }

        public void Consume(UserVerifiedEmail message)
        {
            var saga = _repository.Get<RegisterUserSaga>(message.CorrelationId);
            saga.UserHasConfirmedEmail();
            var body = string.Format("Thank you. You are now registered");
            _client.Send("bob", "dru", "Register Successful", body);
            _repository.Save(saga);
        }
    }
}