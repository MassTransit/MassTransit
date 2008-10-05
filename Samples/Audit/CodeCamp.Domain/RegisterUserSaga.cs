using System;
using CodeCamp.Messages;
using Magnum.Common.DateTimeExtensions;
using MassTransit.Saga;
using MassTransit.ServiceBus;
using MassTransit.ServiceBus.Timeout.Messages;
using MassTransit.ServiceBus.Util;
using PostalService.Messages;

namespace CodeCamp.Domain
{
    public class RegisterUserSaga :
        InitiatedBy<RegisterUser>,
        Orchestrates<UserVerificationEmailSent>,
        Orchestrates<UserVerifiedEmail>,
        Orchestrates<EmailSent>,
        ISaga
    {
        private readonly Guid _correlationId = CombGuid.NewCombGuid();
        private IServiceBus _bus;
        private DateTime _lastEmailSent;
        private User _user;

        public User User
        {
            get { return _user; }
        }

        //Starts things off

        #region InitiatedBy<RegisterUser> Members

        public void Consume(RegisterUser message)
        {
            _user = new User(message.Name, message.Username, message.Password);

            string body = string.Format("Please verify email http://localhost/ConfirmEmail/?registrationId={0}",
                                        _correlationId);
            _bus.Publish(new SendEmail(_correlationId, "bob", "dru", "verify email", body));

            _bus.Publish(new UserVerificationEmailSent(_correlationId));
        }

        #endregion

        #region ISaga Members

        public Guid CorrelationId
        {
            get { return _correlationId; }
        }

        public IServiceBus Bus
        {
            get { return _bus; }
            set { _bus = value; }
        }

        public IObjectBuilder Builder { get; set; }

        #endregion

        #region Orchestrates<EmailSent> Members

        public void Consume(EmailSent message)
        {
            _lastEmailSent = message.SentAt;
        }

        #endregion

        #region Orchestrates<UserVerificationEmailSent> Members

        public void Consume(UserVerificationEmailSent message)
        {
            _user.SetPending();

            _bus.Publish(new ScheduleTimeout(message.CorrelationId, 24.Hours().FromNow()));
        }

        #endregion

        #region Orchestrates<UserVerifiedEmail> Members

        public void Consume(UserVerifiedEmail message)
        {
            _user.EmailHasBeenConfirmed();
            string body = string.Format("Thank you. You are now registered");

            // use a new guid because we don't want any more messages to this saga about e-mails
            _bus.Publish(new SendEmail(Guid.Empty, "bob", "dru", "Register Successful", body));
        }

        #endregion
    }
}