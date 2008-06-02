namespace WebRequestReply.UI
{
    using System;
    using System.Web.UI;
    using Core;

    public partial class _Default :
        Page, IRequestReplyView
    {
        private readonly RequestReplyController _controller;

        public _Default()
        {
            _controller = new RequestReplyController(this, Container.Instance.ServiceBus);
        }

        #region IRequestReplyView Members

        public string RequestText
        {
            get { return requestText.Text; }
        }

        public string ResponseText
        {
            set { responseBox.Text = value; }
        }

        public event EventHandler RequestEntered;

        #endregion


        protected void Button1_Click(object sender, EventArgs e)
        {
            _controller.SendRequest();
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            this.RegisterAsyncTask(new PageAsyncTask(_controller.BeginRequest, _controller.EndRequest, _controller.OnTimeout, this));
        }

    }
}
