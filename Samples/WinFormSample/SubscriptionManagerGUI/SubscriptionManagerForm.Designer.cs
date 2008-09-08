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
			this.subscriptionTree = new System.Windows.Forms.TreeView();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.timeoutList = new System.Windows.Forms.ListView();
			this.timeHeader = new System.Windows.Forms.ColumnHeader();
			this.idHeader = new System.Windows.Forms.ColumnHeader();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// subscriptionTree
			// 
			this.subscriptionTree.Dock = System.Windows.Forms.DockStyle.Fill;
			this.subscriptionTree.Location = new System.Drawing.Point(3, 23);
			this.subscriptionTree.Name = "subscriptionTree";
			this.subscriptionTree.Size = new System.Drawing.Size(265, 282);
			this.subscriptionTree.TabIndex = 3;
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel1.Controls.Add(this.label2, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.subscriptionTree, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.timeoutList, 2, 1);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 2;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.5625F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 93.4375F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(543, 308);
			this.tableLayoutPanel1.TabIndex = 4;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.BackColor = System.Drawing.SystemColors.ActiveCaption;
			this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label1.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.label1.Location = new System.Drawing.Point(3, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(265, 20);
			this.label1.TabIndex = 4;
			this.label1.Text = "Subscriptions";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.BackColor = System.Drawing.SystemColors.ActiveCaption;
			this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label2.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.label2.Location = new System.Drawing.Point(274, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(266, 20);
			this.label2.TabIndex = 5;
			this.label2.Text = "Timeouts";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// timeoutList
			// 
			this.timeoutList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.timeHeader,
            this.idHeader});
			this.timeoutList.Dock = System.Windows.Forms.DockStyle.Fill;
			this.timeoutList.Location = new System.Drawing.Point(274, 23);
			this.timeoutList.Name = "timeoutList";
			this.timeoutList.Size = new System.Drawing.Size(266, 282);
			this.timeoutList.TabIndex = 6;
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
			// SubscriptionManagerForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(543, 308);
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "SubscriptionManagerForm";
			this.Text = "Subscription Manager";
			this.Load += new System.EventHandler(this.SubscriptionManagerForm_Load);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TreeView subscriptionTree;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ListView timeoutList;
		private System.Windows.Forms.ColumnHeader timeHeader;
		private System.Windows.Forms.ColumnHeader idHeader;
	}
}

