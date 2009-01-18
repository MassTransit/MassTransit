namespace Mandelbrot
{
	partial class MainForm
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
			this._horizontalSplitter = new System.Windows.Forms.SplitContainer();
			this._messageTextBox = new System.Windows.Forms.TextBox();
			this._refreshButton = new System.Windows.Forms.Button();
			this._horizontalSplitter.Panel1.SuspendLayout();
			this._horizontalSplitter.Panel2.SuspendLayout();
			this._horizontalSplitter.SuspendLayout();
			this.SuspendLayout();
			// 
			// _horizontalSplitter
			// 
			this._horizontalSplitter.Dock = System.Windows.Forms.DockStyle.Fill;
			this._horizontalSplitter.Location = new System.Drawing.Point(0, 0);
			this._horizontalSplitter.Name = "_horizontalSplitter";
			this._horizontalSplitter.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// _horizontalSplitter.Panel1
			// 
			this._horizontalSplitter.Panel1.Controls.Add(this._refreshButton);
			// 
			// _horizontalSplitter.Panel2
			// 
			this._horizontalSplitter.Panel2.Controls.Add(this._messageTextBox);
			this._horizontalSplitter.Size = new System.Drawing.Size(624, 444);
			this._horizontalSplitter.SplitterDistance = 326;
			this._horizontalSplitter.TabIndex = 0;
			// 
			// _messageTextBox
			// 
			this._messageTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this._messageTextBox.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._messageTextBox.Location = new System.Drawing.Point(0, 0);
			this._messageTextBox.Multiline = true;
			this._messageTextBox.Name = "_messageTextBox";
			this._messageTextBox.Size = new System.Drawing.Size(624, 114);
			this._messageTextBox.TabIndex = 0;
			this._messageTextBox.Text = "Initializing...";
			// 
			// _refreshButton
			// 
			this._refreshButton.Location = new System.Drawing.Point(483, 40);
			this._refreshButton.Name = "_refreshButton";
			this._refreshButton.Size = new System.Drawing.Size(75, 23);
			this._refreshButton.TabIndex = 0;
			this._refreshButton.Text = "Refresh";
			this._refreshButton.UseVisualStyleBackColor = true;
			this._refreshButton.Click += new System.EventHandler(this._refreshButton_Click);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(624, 444);
			this.Controls.Add(this._horizontalSplitter);
			this.Name = "MainForm";
			this.Text = "Mandelbrot Distributed";
			this.Load += new System.EventHandler(this.MainForm_Load);
			this._horizontalSplitter.Panel1.ResumeLayout(false);
			this._horizontalSplitter.Panel2.ResumeLayout(false);
			this._horizontalSplitter.Panel2.PerformLayout();
			this._horizontalSplitter.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer _horizontalSplitter;
		private System.Windows.Forms.TextBox _messageTextBox;
		private System.Windows.Forms.Button _refreshButton;
	}
}

