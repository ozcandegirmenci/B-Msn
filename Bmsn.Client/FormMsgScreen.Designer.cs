namespace Bmsn.Client
{
    partial class FormMsgScreen
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
			_StateManager.Dispose();
			if (_SessionManager != null)
				_SessionManager.MainForm.client1.CommandExecutionManager.Unsubscribe(
					Bmsn.Protocol.CommandNames.SEND_MESSAGE, new Bmsn.Protocol.CommandExecuteHandler(OnSendCommandExecuted));
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMsgScreen));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.rtfMessage = new Bmsn.Client.UI.OleRichTextBox();
            this.lblWriting = new System.Windows.Forms.Label();
            this.menuStrip = new Bmsn.Client.FormMsgScreen.MessageStrip();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnBolt = new System.Windows.Forms.ToolStripButton();
            this.btnItalic = new System.Windows.Forms.ToolStripButton();
            this.btnFont = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.chkSave = new System.Windows.Forms.ToolStripButton();
            this.btnRefresh = new System.Windows.Forms.ToolStripButton();
            this.btnShowDate = new System.Windows.Forms.ToolStripButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pic = new System.Windows.Forms.PictureBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.rtfMessages = new Bmsn.Client.UI.OleRichTextBox();
            this.lblWarning = new System.Windows.Forms.Label();
            this.colorDialog = new System.Windows.Forms.ColorDialog();
            this.fontDialog = new System.Windows.Forms.FontDialog();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.menuStrip.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pic)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 69.52663F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30.47337F));
            this.tableLayoutPanel1.Controls.Add(this.panel2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnSend, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.rtfMessages, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 15);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 75.49669F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 24.50331F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(359, 453);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.rtfMessage);
            this.panel2.Controls.Add(this.lblWriting);
            this.panel2.Controls.Add(this.menuStrip);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(3, 345);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(243, 105);
            this.panel2.TabIndex = 2;
            // 
            // rtfMessage
            // 
            this.rtfMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtfMessage.Location = new System.Drawing.Point(0, 25);
            this.rtfMessage.Name = "rtfMessage";
            this.rtfMessage.Size = new System.Drawing.Size(243, 62);
            this.rtfMessage.TabIndex = 2;
            this.rtfMessage.Text = "";
            this.rtfMessage.SelectionChanged += new System.EventHandler(this.rtfMessage_SelectionChanged);
            this.rtfMessage.TextChanged += new System.EventHandler(this.rtfMessage_TextChanged);
            this.rtfMessage.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.rtfMessage_KeyPress);
            // 
            // lblWriting
            // 
            this.lblWriting.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lblWriting.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblWriting.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblWriting.Location = new System.Drawing.Point(0, 87);
            this.lblWriting.Name = "lblWriting";
            this.lblWriting.Size = new System.Drawing.Size(243, 18);
            this.lblWriting.TabIndex = 1;
            this.lblWriting.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // menuStrip
            // 
            this.menuStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator1,
            this.btnBolt,
            this.btnItalic,
            this.btnFont,
            this.toolStripButton1,
            this.toolStripSeparator2,
            this.chkSave,
            this.btnRefresh,
            this.btnShowDate});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(243, 25);
            this.menuStrip.TabIndex = 0;
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // btnBolt
            // 
            this.btnBolt.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnBolt.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.btnBolt.Image = ((System.Drawing.Image)(resources.GetObject("btnBolt.Image")));
            this.btnBolt.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnBolt.Name = "btnBolt";
            this.btnBolt.Size = new System.Drawing.Size(23, 22);
            this.btnBolt.Text = "B";
            this.btnBolt.ToolTipText = "Bolt";
            this.btnBolt.Click += new System.EventHandler(this.btnBolt_Click);
            // 
            // btnItalic
            // 
            this.btnItalic.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnItalic.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Italic);
            this.btnItalic.Image = ((System.Drawing.Image)(resources.GetObject("btnItalic.Image")));
            this.btnItalic.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnItalic.Name = "btnItalic";
            this.btnItalic.Size = new System.Drawing.Size(23, 22);
            this.btnItalic.Text = "I";
            this.btnItalic.ToolTipText = "Italic";
            this.btnItalic.Click += new System.EventHandler(this.btnItalic_Click);
            // 
            // btnFont
            // 
            this.btnFont.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnFont.Image = ((System.Drawing.Image)(resources.GetObject("btnFont.Image")));
            this.btnFont.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnFont.Name = "btnFont";
            this.btnFont.Size = new System.Drawing.Size(35, 22);
            this.btnFont.Text = "Font";
            this.btnFont.ToolTipText = "Selected Font";
            this.btnFont.Click += new System.EventHandler(this.btnFont_Click);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton1.Text = "Color";
            this.toolStripButton1.ToolTipText = "Selected Color";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // chkSave
            // 
            this.chkSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.chkSave.Image = ((System.Drawing.Image)(resources.GetObject("chkSave.Image")));
            this.chkSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.chkSave.Name = "chkSave";
            this.chkSave.Size = new System.Drawing.Size(23, 22);
            this.chkSave.Text = "toolStripButton2";
            this.chkSave.Click += new System.EventHandler(this.chkSave_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnRefresh.Image = ((System.Drawing.Image)(resources.GetObject("btnRefresh.Image")));
            this.btnRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(23, 22);
            this.btnRefresh.Text = "Refresh Client Image";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnShowDate
            // 
            this.btnShowDate.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnShowDate.Image = ((System.Drawing.Image)(resources.GetObject("btnShowDate.Image")));
            this.btnShowDate.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnShowDate.Name = "btnShowDate";
            this.btnShowDate.Size = new System.Drawing.Size(23, 22);
            this.btnShowDate.Text = "Show Message Date";
            this.btnShowDate.Click += new System.EventHandler(this.btnShowDate_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.pic);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(252, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(104, 336);
            this.panel1.TabIndex = 2;
            // 
            // pic
            // 
            this.pic.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pic.Location = new System.Drawing.Point(3, 2);
            this.pic.Name = "pic";
            this.pic.Size = new System.Drawing.Size(98, 98);
            this.pic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pic.TabIndex = 0;
            this.pic.TabStop = false;
            // 
            // btnSend
            // 
            this.btnSend.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSend.Enabled = false;
            this.btnSend.Image = global::Bmsn.Client.Properties.Resources.documents_24;
            this.btnSend.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSend.Location = new System.Drawing.Point(252, 345);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(104, 105);
            this.btnSend.TabIndex = 1;
            this.btnSend.Text = "Send";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // rtfMessages
            // 
            this.rtfMessages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtfMessages.Location = new System.Drawing.Point(3, 3);
            this.rtfMessages.Name = "rtfMessages";
            this.rtfMessages.Size = new System.Drawing.Size(243, 336);
            this.rtfMessages.TabIndex = 3;
            this.rtfMessages.Text = "";
            this.rtfMessages.KeyDown += new System.Windows.Forms.KeyEventHandler(this.rtfMessages_KeyDown);
            this.rtfMessages.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.rtfMessages_KeyPress);
            // 
            // lblWarning
            // 
            this.lblWarning.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblWarning.Location = new System.Drawing.Point(0, 0);
            this.lblWarning.Name = "lblWarning";
            this.lblWarning.Size = new System.Drawing.Size(359, 15);
            this.lblWarning.TabIndex = 1;
            this.lblWarning.Text = "...";
            this.lblWarning.Visible = false;
            // 
            // FormMsgScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(359, 468);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.lblWarning);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormMsgScreen";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pic)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.PictureBox pic;
		private System.Windows.Forms.Button btnSend;
		private UI.OleRichTextBox rtfMessage;
		private UI.OleRichTextBox rtfMessages;
		private System.Windows.Forms.Label lblWarning;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel2;
		private MessageStrip menuStrip;
		private System.Windows.Forms.ToolStripButton btnBolt;
		private System.Windows.Forms.ToolStripButton btnItalic;
		private System.Windows.Forms.ToolStripButton btnFont;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripButton toolStripButton1;
		private System.Windows.Forms.ColorDialog colorDialog;
		private System.Windows.Forms.FontDialog fontDialog;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripButton chkSave;
		private System.Windows.Forms.ToolStripButton btnRefresh;
		private System.Windows.Forms.ToolStripButton btnShowDate;
		private System.Windows.Forms.Label lblWriting;
	}
}