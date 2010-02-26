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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.subscriptionTree = new System.Windows.Forms.TreeView();
            this.lblSubscriptions = new System.Windows.Forms.Label();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.lblTimeouts = new System.Windows.Forms.Label();
            this.timeoutList = new System.Windows.Forms.ListView();
            this.timeHeader = new System.Windows.Forms.ColumnHeader();
            this.idHeader = new System.Windows.Forms.ColumnHeader();
            this.lblHeartbeats = new System.Windows.Forms.Label();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.heartbeatList = new System.Windows.Forms.ListView();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.subscriptionTree);
            this.splitContainer1.Panel1.Controls.Add(this.lblSubscriptions);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(789, 409);
            this.splitContainer1.SplitterDistance = 263;
            this.splitContainer1.TabIndex = 0;
            // 
            // subscriptionTree
            // 
            this.subscriptionTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.subscriptionTree.Location = new System.Drawing.Point(0, 15);
            this.subscriptionTree.Name = "subscriptionTree";
            this.subscriptionTree.Size = new System.Drawing.Size(263, 394);
            this.subscriptionTree.TabIndex = 6;
            // 
            // lblSubscriptions
            // 
            this.lblSubscriptions.AutoSize = true;
            this.lblSubscriptions.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.lblSubscriptions.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblSubscriptions.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSubscriptions.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lblSubscriptions.Location = new System.Drawing.Point(0, 0);
            this.lblSubscriptions.Name = "lblSubscriptions";
            this.lblSubscriptions.Size = new System.Drawing.Size(83, 15);
            this.lblSubscriptions.TabIndex = 5;
            this.lblSubscriptions.Text = "Subscriptions";
            this.lblSubscriptions.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.timeoutList);
            this.splitContainer2.Panel1.Controls.Add(this.lblTimeouts);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.heartbeatList);
            this.splitContainer2.Panel2.Controls.Add(this.lblHeartbeats);
            this.splitContainer2.Size = new System.Drawing.Size(522, 409);
            this.splitContainer2.SplitterDistance = 174;
            this.splitContainer2.TabIndex = 0;
            // 
            // lblTimeouts
            // 
            this.lblTimeouts.AutoSize = true;
            this.lblTimeouts.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.lblTimeouts.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblTimeouts.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTimeouts.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lblTimeouts.Location = new System.Drawing.Point(0, 0);
            this.lblTimeouts.Name = "lblTimeouts";
            this.lblTimeouts.Size = new System.Drawing.Size(59, 15);
            this.lblTimeouts.TabIndex = 6;
            this.lblTimeouts.Text = "Timeouts";
            this.lblTimeouts.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // timeoutList
            // 
            this.timeoutList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.timeHeader,
            this.idHeader});
            this.timeoutList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.timeoutList.Location = new System.Drawing.Point(0, 15);
            this.timeoutList.Name = "timeoutList";
            this.timeoutList.Size = new System.Drawing.Size(522, 159);
            this.timeoutList.TabIndex = 7;
            this.timeoutList.UseCompatibleStateImageBehavior = false;
            this.timeoutList.View = System.Windows.Forms.View.Details;
            // 
            // timeHeader
            // 
            this.timeHeader.Text = "Time";
            this.timeHeader.Width = 100;
            // 
            // idHeader
            // 
            this.idHeader.Text = "Id";
            this.idHeader.Width = 120;
            // 
            // lblHeartbeats
            // 
            this.lblHeartbeats.AutoSize = true;
            this.lblHeartbeats.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.lblHeartbeats.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblHeartbeats.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeartbeats.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lblHeartbeats.Location = new System.Drawing.Point(0, 0);
            this.lblHeartbeats.Name = "lblHeartbeats";
            this.lblHeartbeats.Size = new System.Drawing.Size(68, 15);
            this.lblHeartbeats.TabIndex = 0;
            this.lblHeartbeats.Text = "Heartbeats";
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Address";
            this.columnHeader1.Width = 223;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "First Detected On";
            this.columnHeader2.Width = 110;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Last Detected At";
            // 
            // heartbeatList
            // 
            this.heartbeatList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.heartbeatList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.heartbeatList.Location = new System.Drawing.Point(0, 15);
            this.heartbeatList.Name = "heartbeatList";
            this.heartbeatList.Size = new System.Drawing.Size(522, 216);
            this.heartbeatList.TabIndex = 10;
            this.heartbeatList.UseCompatibleStateImageBehavior = false;
            this.heartbeatList.View = System.Windows.Forms.View.Details;
            // 
            // SubscriptionManagerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(789, 409);
            this.Controls.Add(this.splitContainer1);
            this.Name = "SubscriptionManagerForm";
            this.Text = "MassTransit System View";
            this.Load += new System.EventHandler(this.SubscriptionManagerForm_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView subscriptionTree;
        private System.Windows.Forms.Label lblSubscriptions;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.ListView timeoutList;
        private System.Windows.Forms.ColumnHeader timeHeader;
        private System.Windows.Forms.ColumnHeader idHeader;
        private System.Windows.Forms.Label lblTimeouts;
        private System.Windows.Forms.Label lblHeartbeats;
        private System.Windows.Forms.ListView heartbeatList;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;

    }
}

