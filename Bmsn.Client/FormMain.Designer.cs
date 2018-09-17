namespace Bmsn.Client
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
			client1.CommandExecutionManager.Unsubscribe(
				Bmsn.Protocol.CommandNames.LOGIN,
				new Bmsn.Protocol.CommandExecuteHandler(OnLoginCommandExecuted));
			client1.ServerCommandsManager.Unsubscribe(
                Bmsn.Protocol.CommandNames.U_LOGIN,
				new Bmsn.Protocol.CommandExecuteHandler(OnUserConnect));
			client1.ServerCommandsManager.Unsubscribe(
                Bmsn.Protocol.CommandNames.U_DISCONNECT,
				new Bmsn.Protocol.CommandExecuteHandler(OnUserDisconnect));
			client1.ServerCommandsManager.Unsubscribe(
                Bmsn.Protocol.CommandNames.U_STATUS_CHANGED,
				new Bmsn.Protocol.CommandExecuteHandler(OnUserStatusChanged));
			client1.ServerCommandsManager.Unsubscribe(
                Bmsn.Protocol.CommandNames.GET_PICTURE,
				new Bmsn.Protocol.CommandExecuteHandler(OnGetPicture));
			client1.ServerCommandsManager.Unsubscribe(
                Bmsn.Protocol.CommandNames.CLIENT_UPDATE_PICTURE,
				new Bmsn.Protocol.CommandExecuteHandler(OnClientUpdatePicture));
			if (client1 != null)
				client1.Dispose();
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
			this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
			this.pnlLoginScreen = new System.Windows.Forms.Panel();
			this.btnLogin = new System.Windows.Forms.Button();
			this.txtPassword = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.txtUsername = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.lvwUsers = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.imgStatuses = new System.Windows.Forms.ImageList(this.components);
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.signOutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.durumToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.aktifToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.yemekteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.disardaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.disardaToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.testToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.helpToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.client1 = new Client.NetClient();
			this.toolStripContainer1.ContentPanel.SuspendLayout();
			this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
			this.toolStripContainer1.SuspendLayout();
			this.pnlLoginScreen.SuspendLayout();
			this.menuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolStripContainer1
			// 
			// 
			// toolStripContainer1.ContentPanel
			// 
			this.toolStripContainer1.ContentPanel.Controls.Add(this.pnlLoginScreen);
			this.toolStripContainer1.ContentPanel.Controls.Add(this.lvwUsers);
			this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(277, 392);
			this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
			this.toolStripContainer1.Name = "toolStripContainer1";
			this.toolStripContainer1.Size = new System.Drawing.Size(277, 416);
			this.toolStripContainer1.TabIndex = 0;
			this.toolStripContainer1.Text = "toolStripContainer1";
			// 
			// toolStripContainer1.TopToolStripPanel
			// 
			this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.menuStrip1);
			// 
			// pnlLoginScreen
			// 
			this.pnlLoginScreen.Controls.Add(this.btnLogin);
			this.pnlLoginScreen.Controls.Add(this.txtPassword);
			this.pnlLoginScreen.Controls.Add(this.label2);
			this.pnlLoginScreen.Controls.Add(this.txtUsername);
			this.pnlLoginScreen.Controls.Add(this.label1);
			this.pnlLoginScreen.Location = new System.Drawing.Point(12, 119);
			this.pnlLoginScreen.Name = "pnlLoginScreen";
			this.pnlLoginScreen.Size = new System.Drawing.Size(253, 139);
			this.pnlLoginScreen.TabIndex = 0;
			// 
			// btnLogin
			// 
			this.btnLogin.Location = new System.Drawing.Point(82, 60);
			this.btnLogin.Name = "btnLogin";
			this.btnLogin.Size = new System.Drawing.Size(168, 76);
			this.btnLogin.TabIndex = 1;
			this.btnLogin.Text = "Sign In";
			this.btnLogin.UseVisualStyleBackColor = true;
			this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
			// 
			// txtPassword
			// 
			this.txtPassword.Location = new System.Drawing.Point(82, 32);
			this.txtPassword.Name = "txtPassword";
			this.txtPassword.ReadOnly = true;
			this.txtPassword.Size = new System.Drawing.Size(168, 21);
			this.txtPassword.TabIndex = 4;
			this.txtPassword.Text = "[Not needed in demo]";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(4, 37);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(53, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "Password";
			// 
			// txtUsername
			// 
			this.txtUsername.Location = new System.Drawing.Point(82, 5);
			this.txtUsername.Name = "txtUsername";
			this.txtUsername.Size = new System.Drawing.Size(168, 21);
			this.txtUsername.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(4, 10);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(55, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "Username";
			// 
			// lvwUsers
			// 
			this.lvwUsers.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
			this.lvwUsers.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lvwUsers.Location = new System.Drawing.Point(0, 0);
			this.lvwUsers.Name = "lvwUsers";
			this.lvwUsers.Size = new System.Drawing.Size(277, 392);
			this.lvwUsers.SmallImageList = this.imgStatuses;
			this.lvwUsers.TabIndex = 1;
			this.lvwUsers.UseCompatibleStateImageBehavior = false;
			this.lvwUsers.View = System.Windows.Forms.View.Details;
			this.lvwUsers.Visible = false;
			this.lvwUsers.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lvwUsers_MouseDoubleClick);
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Username";
			this.columnHeader1.Width = 230;
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
			// menuStrip1
			// 
			this.menuStrip1.Dock = System.Windows.Forms.DockStyle.None;
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.helpToolStripMenuItem,
            this.helpToolStripMenuItem1});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(277, 24);
			this.menuStrip1.TabIndex = 0;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.signOutToolStripMenuItem,
            this.durumToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.fileToolStripMenuItem.Text = "&File";
			// 
			// signOutToolStripMenuItem
			// 
			this.signOutToolStripMenuItem.Enabled = false;
			this.signOutToolStripMenuItem.Name = "signOutToolStripMenuItem";
			this.signOutToolStripMenuItem.Size = new System.Drawing.Size(115, 22);
			this.signOutToolStripMenuItem.Text = "Signout";
			this.signOutToolStripMenuItem.Click += new System.EventHandler(this.signOutToolStripMenuItem_Click);
			// 
			// durumToolStripMenuItem
			// 
			this.durumToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aktifToolStripMenuItem,
            this.yemekteToolStripMenuItem,
            this.disardaToolStripMenuItem,
            this.disardaToolStripMenuItem1});
			this.durumToolStripMenuItem.Enabled = false;
			this.durumToolStripMenuItem.Name = "durumToolStripMenuItem";
			this.durumToolStripMenuItem.Size = new System.Drawing.Size(115, 22);
			this.durumToolStripMenuItem.Text = "State";
			// 
			// aktifToolStripMenuItem
			// 
			this.aktifToolStripMenuItem.Name = "aktifToolStripMenuItem";
			this.aktifToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
			this.aktifToolStripMenuItem.Text = "Active";
			this.aktifToolStripMenuItem.Click += new System.EventHandler(this.aktifToolStripMenuItem_Click);
			// 
			// yemekteToolStripMenuItem
			// 
			this.yemekteToolStripMenuItem.Name = "yemekteToolStripMenuItem";
			this.yemekteToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
			this.yemekteToolStripMenuItem.Text = "Out To Lunch";
			this.yemekteToolStripMenuItem.Click += new System.EventHandler(this.yemekteToolStripMenuItem_Click);
			// 
			// disardaToolStripMenuItem
			// 
			this.disardaToolStripMenuItem.Name = "disardaToolStripMenuItem";
			this.disardaToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
			this.disardaToolStripMenuItem.Text = "Waiting";
			this.disardaToolStripMenuItem.Click += new System.EventHandler(this.disardaToolStripMenuItem_Click);
			// 
			// disardaToolStripMenuItem1
			// 
			this.disardaToolStripMenuItem1.Name = "disardaToolStripMenuItem1";
			this.disardaToolStripMenuItem1.Size = new System.Drawing.Size(147, 22);
			this.disardaToolStripMenuItem1.Text = "Away";
			this.disardaToolStripMenuItem1.Click += new System.EventHandler(this.disardaToolStripMenuItem1_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(112, 6);
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(115, 22);
			this.exitToolStripMenuItem.Text = "Exit";
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
			// 
			// toolsToolStripMenuItem
			// 
			this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.optionsToolStripMenuItem});
			this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
			this.toolsToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
			this.toolsToolStripMenuItem.Text = "&Tools";
			// 
			// optionsToolStripMenuItem
			// 
			this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
			this.optionsToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
			this.optionsToolStripMenuItem.Text = "&Select My Picture";
			this.optionsToolStripMenuItem.Click += new System.EventHandler(this.optionsToolStripMenuItem_Click);
			// 
			// helpToolStripMenuItem
			// 
			this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.testToolStripMenuItem});
			this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
			this.helpToolStripMenuItem.Size = new System.Drawing.Size(41, 20);
			this.helpToolStripMenuItem.Text = "&Test";
			// 
			// testToolStripMenuItem
			// 
			this.testToolStripMenuItem.Name = "testToolStripMenuItem";
			this.testToolStripMenuItem.Size = new System.Drawing.Size(244, 22);
			this.testToolStripMenuItem.Text = "Execute Custom Test Command";
			this.testToolStripMenuItem.Click += new System.EventHandler(this.testToolStripMenuItem_Click);
			// 
			// helpToolStripMenuItem1
			// 
			this.helpToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
			this.helpToolStripMenuItem1.Name = "helpToolStripMenuItem1";
			this.helpToolStripMenuItem1.Size = new System.Drawing.Size(44, 20);
			this.helpToolStripMenuItem1.Text = "Help";
			// 
			// aboutToolStripMenuItem
			// 
			this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
			this.aboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
			this.aboutToolStripMenuItem.Text = "About";
			this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
			// 
			// client1
			// 
			this.client1.SystemError += new Bmsn.Protocol.SystemErrorEventHandler(this.client1_SystemError);
			this.client1.SocketStatusChanged += new System.EventHandler(this.client1_SocketStatusChanged);
			// 
			// FormMain
			// 
			this.AcceptButton = this.btnLogin;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(277, 416);
			this.Controls.Add(this.toolStripContainer1);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "FormMain";
			this.Text = "B-MSN Client";
			this.toolStripContainer1.ContentPanel.ResumeLayout(false);
			this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
			this.toolStripContainer1.TopToolStripPanel.PerformLayout();
			this.toolStripContainer1.ResumeLayout(false);
			this.toolStripContainer1.PerformLayout();
			this.pnlLoginScreen.ResumeLayout(false);
			this.pnlLoginScreen.PerformLayout();
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ToolStripContainer toolStripContainer1;
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem signOutToolStripMenuItem;
		private System.Windows.Forms.ListView lvwUsers;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.Panel pnlLoginScreen;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
		private System.Windows.Forms.TextBox txtPassword;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox txtUsername;
		private System.Windows.Forms.Button btnLogin;
		private System.Windows.Forms.ToolStripMenuItem durumToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem aktifToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem yemekteToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem disardaToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ImageList imgStatuses;
		private System.Windows.Forms.ToolStripMenuItem disardaToolStripMenuItem1;
		internal NetClient client1;
		private System.Windows.Forms.ToolStripMenuItem testToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
	}
}

