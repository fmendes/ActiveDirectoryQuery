namespace ADInspector
{
	partial class frmMain
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
			this.lblUserId = new System.Windows.Forms.Label();
			this.tbUserId = new System.Windows.Forms.TextBox();
			this.btnQuery = new System.Windows.Forms.Button();
			this.tvValues = new System.Windows.Forms.TreeView();
			this.SuspendLayout();
			// 
			// lblUserId
			// 
			this.lblUserId.AutoSize = true;
			this.lblUserId.Location = new System.Drawing.Point(12, 9);
			this.lblUserId.Name = "lblUserId";
			this.lblUserId.Size = new System.Drawing.Size(44, 13);
			this.lblUserId.TabIndex = 0;
			this.lblUserId.Text = "User Id:";
			// 
			// tbUserId
			// 
			this.tbUserId.Location = new System.Drawing.Point(62, 6);
			this.tbUserId.Name = "tbUserId";
			this.tbUserId.Size = new System.Drawing.Size(254, 20);
			this.tbUserId.TabIndex = 1;
			this.tbUserId.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbUserId_KeyPress);
			// 
			// btnQuery
			// 
			this.btnQuery.Location = new System.Drawing.Point(322, 4);
			this.btnQuery.Name = "btnQuery";
			this.btnQuery.Size = new System.Drawing.Size(75, 23);
			this.btnQuery.TabIndex = 2;
			this.btnQuery.Text = "&Search";
			this.btnQuery.UseVisualStyleBackColor = true;
			this.btnQuery.Click += new System.EventHandler(this.btnQuery_Click);
			// 
			// tvValues
			// 
			this.tvValues.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tvValues.Location = new System.Drawing.Point(1, 32);
			this.tvValues.Name = "tvValues";
			this.tvValues.Size = new System.Drawing.Size(406, 388);
			this.tvValues.TabIndex = 3;
			// 
			// frmMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(409, 420);
			this.Controls.Add(this.tvValues);
			this.Controls.Add(this.btnQuery);
			this.Controls.Add(this.tbUserId);
			this.Controls.Add(this.lblUserId);
			this.Name = "frmMain";
			this.Text = "Active Directory Inspector";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label lblUserId;
		private System.Windows.Forms.TextBox tbUserId;
		private System.Windows.Forms.Button btnQuery;
		private System.Windows.Forms.TreeView tvValues;
	}
}

