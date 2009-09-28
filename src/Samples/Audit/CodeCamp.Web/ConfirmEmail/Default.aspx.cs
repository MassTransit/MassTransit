namespace CodeCamp.Web
{
    using System;
    using System.Web.UI;
    using Domain;
    using Messages;

    public partial class EmailConfirmed : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var registrationId = new Guid(Request.QueryString[0]);
            DomainContext.Publish(new UserVerifiedEmail(registrationId));
        }
    }
}