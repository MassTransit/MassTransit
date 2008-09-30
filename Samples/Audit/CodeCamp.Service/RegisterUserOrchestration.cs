namespace CodeCamp.Service
{
    using System;
    using System.Net.Mail;
    using Domain;
    using Magnum.Common.Repository;
    using MassTransit.Saga;
    using MassTransit.ServiceBus;
    using MassTransit.ServiceBus.Timeout.Messages;
    using Magnum.Common.DateTimeExtensions;
    using MassTransit.ServiceBus.Util;
    using Messages;
    using PostalService.Messages;

    public class RegisterUserOrchestration :
        InitiatedBy<RegisterUser>,
        Orchestrates<UserVerificationEmailSent>,
        Orchestrates<UserVerifiedEmail>,
        Orchestrates<EmailSent>,
        ISaga
    {
        private IServiceBus _bus;
        private Guid _correlationId = CombGuid.NewCombGuid();
        private IObjectBuilder _builder;
        private User _user;
        private DateTime _lastEmailSent;

        public RegisterUserOrchestration()
        {
        }

        //Starts things off
        public void Consume(RegisterUser message)
        {
            _user = new User(message.Name, message.Username, message.Password);

            var body = string.Format("Please verify email http://localhost/ConfirmEmail.aspx?registrationId={0}", this._correlationId);
            _bus.Publish(new SendEmail(_correlationId, "bob","dru","verify email", body));

            _bus.Publish(new UserVerificationEmailSent(this._correlationId));
        }

        public void Consume(UserVerificationEmailSent message)
        {
            _user.SetPending();

            _bus.Publish(new ScheduleTimeout(message.CorrelationId, 24.Hours().FromNow()));
        }

        public void Consume(UserVerifiedEmail message)
        {
            _user.EmailHasBeenConfirmed();
            var body = string.Format("Thank you. You are now registered");

            // use a new guid because we don't want any more messages to this saga about e-mails
            _bus.Publish(new SendEmail(Guid.Empty, "bob", "dru", "Register Successful", body));
        }

        public Guid CorrelationId
        {
            get { return _correlationId; }
        }

        public IServiceBus Bus
        {
            get { return _bus; }
            set { _bus = value; }
        }

        public IObjectBuilder Builder
        {
            get { return _builder; }
            set { _builder = value; }
        }

        public User User
        {
            get { return _user; }
        }

        public void Consume(EmailSent message)
        {
            _lastEmailSent = message.SentAt;
            
        }
    }
}