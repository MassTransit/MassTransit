namespace CodeCamp.Domain
{
    using System;
    using Magnum.Common.DateTimeExtensions;
    using MassTransit.Saga;
    using MassTransit.ServiceBus;
    using MassTransit.ServiceBus.Timeout.Messages;
    using MassTransit.ServiceBus.Util;
    using Messages;
    using Microsoft.Practices.ServiceLocation;
    using PostalService.Messages;

    public class RegisterUserSaga :
        InitiatedBy<RegisterUser>,
        Orchestrates<UserVerificationEmailSent>,
        Orchestrates<UserVerifiedEmail>,
        Orchestrates<EmailSent>,
        ISaga
    {
        private readonly Guid _correlationId = CombGuid.NewCombGuid();
        private DateTime _lastEmailSent;
        private User _user;

        public User User
        {
            get { return _user; }
        }

        #region ISaga Members

        public Guid CorrelationId
        {
            get { return _correlationId; }
        }

        public IServiceBus Bus { get; set; }

        public IServiceLocator ServiceLocator { get; set; }

        #endregion

        #region InitiatedBy<RegisterUser> Members

        public void Consume(RegisterUser message)
        {
            _user = new User(message.Name, message.Username, message.Password, message.Email);

            string body = string.Format("Please verify email http://localhost/ConfirmEmail/?registrationId={0}",
                                        _correlationId);
            Bus.Publish(new SendEmail(CorrelationId, _user.Email, "dru", "verify email", body));
        }

        #endregion

        #region Orchestrates<EmailSent> Members

        public void Consume(EmailSent message)
        {
            _lastEmailSent = message.SentAt;

            Bus.Publish(new UserVerificationEmailSent(_correlationId));
        }

        #endregion

        #region Orchestrates<UserVerificationEmailSent> Members

        public void Consume(UserVerificationEmailSent message)
        {
            _user.SetEmailPending();

            Bus.Publish(new ScheduleTimeout(CorrelationId, 24.Hours().FromNow()));
        }

        #endregion

        #region Orchestrates<UserVerifiedEmail> Members

        public void Consume(UserVerifiedEmail message)
        {
            _user.ConfirmEmail();
            string body = string.Format("Thank you. You are now registered");

            // use a new guid because we don't want any more messages to this saga about e-mails
            Bus.Publish(new SendEmail(Guid.Empty, _user.Email, "dru", "Register Successful", body));
        }

        #endregion
    }
}