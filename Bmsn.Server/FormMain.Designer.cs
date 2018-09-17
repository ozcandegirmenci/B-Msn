namespace Bmsn.Server
{
	partial class FormMain
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
				server.Dispose();
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.server = new Bmsn.Server.NetServer();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.lvwUsers = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.imgStatuses = new System.Windows.Forms.ImageList(this.components);
            this.lblStatus = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // server
            // 
            this.server.Started += new System.EventHandler(this.server_Started);
            this.server.Stopped += new System.EventHandler(this.server_Stopped);
            this.server.SystemErrorOccured += new Bmsn.Protocol.SystemErrorEventHandler(this.server_SystemErrorOccured);
            this.server.ClientConnected += new Bmsn.Server.ServerClientEventHandler(this.server_ClientConnected);
            this.server.ClientDisconnected += new Bmsn.Server.ServerClientEventHandler(this.server_ClientDisconnected);
            this.server.NewCommand += new Bmsn.Server.CommandOperationEventHandler(this.server_NewCommand);
            this.server.ClientStatusChanged += new Bmsn.Server.ServerClientEventHandler(this.server_ClientStatusChanged);
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(3, 3);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.Enabled = false;
            this.btnStop.Location = new System.Drawing.Point(84, 3);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 1;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // lvwUsers
            // 
            this.lvwUsers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvwUsers.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
            this.lvwUsers.Location = new System.Drawing.Point(3, 33);
            this.lvwUsers.Name = "lvwUsers";
            this.lvwUsers.Size = new System.Drawing.Size(552, 421);
            this.lvwUsers.SmallImageList = this.imgStatuses;
            this.lvwUsers.TabIndex = 2;
            this.lvwUsers.UseCompatibleStateImageBehavior = false;
            this.lvwUsers.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Client IP";
            this.columnHeader1.Width = 95;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Connect Time";
            this.columnHeader2.Width = 111;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Username";
            this.columnHeader3.Width = 109;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Status";
            this.columnHeader4.Width = 107;
            // 
            // imgStatuses
            // 
            this.imgStatuses.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgStatuses.ImageStream")));
            this.imgStatuses.TransparentColor = System.Drawing.Color.Transparent;
            this.imgStatuses.Images.SetKeyName(0, "cancel_16.gif");
            this.imgStatuses.Images.SetKeyName(1, "arrow-forward_16.gif");
            this.imgStatuses.Images.SetKeyName(2, "chat_16.gif");
            this.imgStatuses.Images.SetKeyName(3, "confirm_16.gif");
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(166, 12);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(77, 13);
            this.lblStatus.TabIndex = 3;
            this.lblStatus.Text = "Last Command";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(559, 457);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.lvwUsers);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnStart);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormMain";
            this.Text = "B-MSN Server";
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private NetServer server;
		private System.Windows.Forms.Button btnStart;
		private System.Windows.Forms.Button btnStop;
		private System.Windows.Forms.ListView lvwUsers;
		private System.Windows.Forms.Label lblStatus;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.ColumnHeader columnHeader4;
		private System.Windows.Forms.ImageList imgStatuses;

	}
}

