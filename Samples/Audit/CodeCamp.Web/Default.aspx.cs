namespace CodeCamp.Web
{
    using System;
    using System.Linq;
    using System.Web.UI;
    using Domain;
    using Magnum;
    using Magnum.Data;
    using Masters;

    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            username.Focus();
        }

        protected void submitButton_Click(object sender, EventArgs e)
        {
            using (var timer = new FunctionTimer("_Default submiteButton_Click", x => timerLabel.Text = x))
            {
                User user;
                using (IRepository<User, Guid> repository = DomainContext.ServiceLocator.GetInstance<IRepositoryFactory>().GetRepository<User, Guid>())
                {
                    user = repository.Where(u => u.Username == username.Text).FirstOrDefault();
                }

                if (user != null)
                {
                    if (user.CheckPassword(password.Text))
                    {
                        if ((user.HasEmailBeenConfirmed ?? false))
                        {
                            ((TulsaTechFest) this.Master).SetNotice("user access granted");
                        }
                        else
                        {
                            ((TulsaTechFest) this.Master).SetError("e-mail not yet confirmed");
                        }
                    }
                    else
                    {
                        ((TulsaTechFest) this.Master).SetError(string.Format("Invalid password specified for user '{0}'", username.Text));
                    }
                }
                else
                {
                    ((TulsaTechFest) this.Master).SetError(string.Format("User not found: '{0}'", username.Text));
                }
            }
        }
    }
}