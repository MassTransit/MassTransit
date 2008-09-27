using System;

namespace CodeCamp.Web.Masters
{
    using System.Web.UI;

    public partial class TulsaTechFest : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public void SetNotice(string message)
        {
            if (!outputNotification.Visible) outputNotification.Visible = true;

            outputNotification.Controls.Add(new LiteralControl(message));
        }

        public void SetError(string message)
        {
            if(!outputError.Visible) outputError.Visible = true;

            outputError.Controls.Add(new LiteralControl(message));
        }
    }
}
