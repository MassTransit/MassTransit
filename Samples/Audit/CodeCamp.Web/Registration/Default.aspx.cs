namespace CodeCamp.Web
{
    using System;
    using System.Web.UI;
    using Domain;
    using Magnum;
    using MassTransit.Util;
    using Masters;
    using Messages;

    public partial class Registration : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void submitButton_Click(object sender, EventArgs e)
        {
            using (var timer = new FunctionTimer("Registration submiteButton_Click", x => timerLabel.Text = x))
            {
                var msg = new RegisterUser(CombGuid.NewCombGuid(), inputName.Text, inputUsername.Text, inputPassword.Text, inputEmail.Text);

                DomainContext.Publish(msg);
                ((TulsaTechFest) Master).SetNotice("Thanks! You should receive an email shortly.");
                submitButton.Enabled = false;
            }
        }
    }
}