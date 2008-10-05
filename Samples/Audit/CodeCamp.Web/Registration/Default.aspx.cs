namespace CodeCamp.Web
{
    using System;
    using System.Web.UI;
    using Core;
    using Magnum.Common;
    using Masters;
    using Messages;

    public partial class Registration : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void submitButton_Click(object sender, EventArgs e)
        {
            using (var timer = new FunctionTimer("Registration submiteButton_Click", delegate { return; }))
            {
                var msg = new RegisterUser(inputName.Text, inputUsername.Text, inputPassword.Text, Guid.NewGuid());

                DomainContext.Publish(msg);
                ((TulsaTechFest) Master).SetNotice("Thanks! You should receive an email shortly.");
                submitButton.Enabled = false;
            }
        }
    }
}