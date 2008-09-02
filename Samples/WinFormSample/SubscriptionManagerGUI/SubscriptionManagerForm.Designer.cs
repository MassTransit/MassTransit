namespace SubscriptionManagerGUI
{
	partial class SubscriptionManagerForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this._subscriptions = new System.Windows.Forms.TreeView();
			this.SuspendLayout();
			// 
			// _subscriptions
			// 
			this._subscriptions.Dock = System.Windows.Forms.DockStyle.Fill;
			this._subscriptions.Location = new System.Drawing.Point(0, 0);
			this._subscriptions.Name = "_subscriptions";
			this._subscriptions.Size = new System.Drawing.Size(425, 421);
			this._subscriptions.TabIndex = 3;
			// 
			// SubscriptionManagerForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(425, 421);
			this.Controls.Add(this._subscriptions);
			this.Name = "SubscriptionManagerForm";
			this.Text = "Subscription Manager";
			this.Load += new System.EventHandler(this.SubscriptionManagerForm_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TreeView _subscriptions;
	}
}

