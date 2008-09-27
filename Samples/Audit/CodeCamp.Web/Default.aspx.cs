namespace CodeCamp.Web
{
	using System;
	using System.Web.UI;
	using Core;
	using Magnum.Common;
	using Masters;

    public partial class _Default : Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			username.Focus();
		}

        protected void submitButton_Click(object sender, EventArgs e)
        {
            using (var timer = new FunctionTimer("_Default submiteButton_Click", delegate(string stuff) { return; }))
            {
                User user = DomainContext.UserRepository.Get(username.Text);

                if (user != null)
                {
                    if (user.CheckPassword(password.Text))
                    {
                        ((TulsaTechFest)this.Master).SetNotice("user access granted");
                    }
                    else
                    {
                        ((TulsaTechFest)this.Master).SetError(string.Format("Invalid password specified for user '{0}'", username.Text));
                    }
                }
                else
                {
                    ((TulsaTechFest)this.Master).SetError(string.Format("User not found: '{0}'", username.Text));
                }

                timerLabel.Text = string.Format("Elapsed Time: {0}", timer.Header);
            }
        }
	}
}