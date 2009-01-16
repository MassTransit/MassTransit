namespace VendorWebClient
{
    partial class Form1
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
			this.label1 = new System.Windows.Forms.Label();
			this.partNumberBox = new System.Windows.Forms.TextBox();
			this.checkInventoryButton = new System.Windows.Forms.Button();
			this.onHandBox = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.onOrderBox = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.responsePartNumber = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.responseTime = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(69, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Part Number:";
			// 
			// partNumberBox
			// 
			this.partNumberBox.Location = new System.Drawing.Point(87, 6);
			this.partNumberBox.Name = "partNumberBox";
			this.partNumberBox.Size = new System.Drawing.Size(155, 20);
			this.partNumberBox.TabIndex = 1;
			// 
			// checkInventoryButton
			// 
			this.checkInventoryButton.Location = new System.Drawing.Point(132, 32);
			this.checkInventoryButton.Name = "checkInventoryButton";
			this.checkInventoryButton.Size = new System.Drawing.Size(110, 23);
			this.checkInventoryButton.TabIndex = 2;
			this.checkInventoryButton.Text = "Check Inventory";
			this.checkInventoryButton.UseVisualStyleBackColor = true;
			this.checkInventoryButton.Click += new System.EventHandler(this.checkInventoryButton_Click);
			// 
			// onHandBox
			// 
			this.onHandBox.Location = new System.Drawing.Point(152, 85);
			this.onHandBox.Name = "onHandBox";
			this.onHandBox.ReadOnly = true;
			this.onHandBox.Size = new System.Drawing.Size(90, 20);
			this.onHandBox.TabIndex = 4;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 88);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(95, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "Quantity On Hand:";
			// 
			// onOrderBox
			// 
			this.onOrderBox.Location = new System.Drawing.Point(152, 111);
			this.onOrderBox.Name = "onOrderBox";
			this.onOrderBox.ReadOnly = true;
			this.onOrderBox.Size = new System.Drawing.Size(90, 20);
			this.onOrderBox.TabIndex = 6;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(12, 114);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(95, 13);
			this.label3.TabIndex = 5;
			this.label3.Text = "Quantity On Order:";
			// 
			// responsePartNumber
			// 
			this.responsePartNumber.Location = new System.Drawing.Point(152, 59);
			this.responsePartNumber.Name = "responsePartNumber";
			this.responsePartNumber.ReadOnly = true;
			this.responsePartNumber.Size = new System.Drawing.Size(90, 20);
			this.responsePartNumber.TabIndex = 8;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(12, 62);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(69, 13);
			this.label4.TabIndex = 7;
			this.label4.Text = "Part Number:";
			// 
			// responseTime
			// 
			this.responseTime.Location = new System.Drawing.Point(152, 137);
			this.responseTime.Name = "responseTime";
			this.responseTime.ReadOnly = true;
			this.responseTime.Size = new System.Drawing.Size(90, 20);
			this.responseTime.TabIndex = 10;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(12, 140);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(84, 13);
			this.label5.TabIndex = 9;
			this.label5.Text = "Response Time:";
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(260, 171);
			this.Controls.Add(this.responseTime);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.responsePartNumber);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.onOrderBox);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.onHandBox);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.checkInventoryButton);
			this.Controls.Add(this.partNumberBox);
			this.Controls.Add(this.label1);
			this.Name = "Form1";
			this.Text = "Form1";
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox partNumberBox;
        private System.Windows.Forms.Button checkInventoryButton;
        private System.Windows.Forms.TextBox onHandBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox onOrderBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox responsePartNumber;
        private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox responseTime;
		private System.Windows.Forms.Label label5;
    }
}

