namespace CodeCamp.Web
{
	using System;
	using System.Threading;
	using System.Web.UI;
	using Core;

	public partial class _Default : Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			username.Focus();
		}

		protected void submitButton_Click(object sender, EventArgs e)
		{
			FunctionTimer timer = new FunctionTimer();

			User user = DomainContext.UserRepository.Get(username.Text);
			if (user != null)
			{
				if (user.CheckPassword(password.Text))
				{
					resultLabel.Text = string.Format("User access granted.");
				}
				else
				{
					resultLabel.Text = string.Format("Invalid password specified for user {0}", username.Text);
				}
			}
			else
			{
				resultLabel.Text = string.Format("User not found: {0}", username.Text);
			}

			timerLabel.Text = string.Format("Elapsed Time: {0}ms", timer.ElapsedTime.TotalMilliseconds);
		}
	}
}