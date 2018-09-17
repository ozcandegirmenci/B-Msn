using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Bmsn.Client.UI;
using Bmsn.Protocol;

namespace Bmsn.Client
{
    /// <summary>
    /// Messaging window
    /// </summary>
    public partial class FormMsgScreen : Form
	{
        #region Types

        /// <summary>
        /// Manages messages in this form
        /// </summary>
        private class MessageManager : IDisposable
        {
            #region Members

            private Font _SenderFont = null;
            private FormMsgScreen _Form = null;

            #endregion

            #region Properties

            /// <summary>
            /// Gets the messaging textbox
            /// </summary>
            public RichTextBox TextBox
            {
                get { return _Form.rtfMessages; }
            }

            /// <summary>
            /// Gets the lendgth
            /// </summary>
            public int Length { get; private set; }

            /// <summary>
            /// Gets that is manager operating a message at the moment or not?
            /// </summary>
            public bool IsOperating { get; private set; }

            #endregion

            #region Initialization

            /// <summary>
            /// Initialize a new instance of this class
            /// </summary>
            /// <param name="form"></param>
            public MessageManager(FormMsgScreen form)
            {
                _Form = form;
                _SenderFont = new Font(TextBox.Font, FontStyle.Bold);
            }

            #endregion

            #region Public Methods

            /// <summary>
            /// Adds given message with the message type to the message texbox
            /// </summary>
            /// <param name="message"></param>
            /// <param name="type"></param>
            public void AppendMessage(string message, MessageType type)
            {
                try
                {
                    while (IsOperating)
                    {
                        Thread.Sleep(5);
                    }

                    IsOperating = true;

                    TextBox.SelectionStart = Length;
                    TextBox.SelectionLength = 0;

                    switch (type)
                    {
                        case MessageType.CommandError:
                            AppendSender("Engine", Color.Olive);
                            AppendError(message);
                            break;
                        case MessageType.SendedByMe:
                            AppendSender("Me", Color.Navy);
                            AppendRtf(message);
                            break;
                        case MessageType.SendedByTarget:
                            AppendSender(_Form.TargetUser, Color.Red);
                            AppendRtf(message);
                            break;
                        case MessageType.ServerError:
                            AppendSender("Server", Color.Gray);
                            AppendError(message);
                            break;
                    }

                    Length = TextBox.Text.Length;
                    TextBox.Select(Length, 0);
                    TextBox.Focus();
                    TextBox.ScrollToCaret();

                    IsOperating = false;
                }
                catch (Exception ex)
                {
                    try
                    {
                        IsOperating = false;
                        AppendMessage(ex.Message, MessageType.CommandError);
                    }
                    catch
                    {
                        TextBox.Text = "ERROR";
                        IsOperating = false;
                    }
                }
            }

            /// <summary>
            /// Releases all unmanaged resources
            /// </summary>
            public void Dispose()
            {
                Dispose(true);
            }

            #endregion

            #region Protected Methods

            /// <summary>
            /// Releases all unmanaged resources
            /// </summary>
            /// <param name="disposing"></param>
            protected virtual void Dispose(bool disposing)
            {
                if (disposing)
                {
                    if (_Form.TargetUser != null)
                    {
                        if (_Form.chkSave.Checked)
                        {
                            try
                            {

                                string dir = Path.GetDirectoryName(Application.ExecutablePath);
                                if (!Directory.Exists(dir))
                                    Directory.CreateDirectory(dir);

                                DateTime d = DateTime.Now;
                                string root = string.Format(@"{0}\{1}_{2}_{3}_{4}",
                                    dir, _Form.TargetUser, d.Day, d.Month, d.Year);
                                string filename = "";
                                int count = 0;
                                while (true)
                                {
                                    filename = string.Format("{0}_{1}.rtf", root, count);

                                    if (!File.Exists(filename))
                                        break;
                                    count++;
                                }
                                TextBox.SaveFile(filename, RichTextBoxStreamType.RichText);
                            }
                            catch // (Exception ex)
                            {

                            }
                        }
                    }
                    if (_SenderFont != null)
                        _SenderFont.Dispose();
                }
            }

            #endregion

            #region Private Methods

            /// <summary>
            /// Appends sender to the textbox
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="color"></param>
            private void AppendSender(string sender, Color color)
            {
                TextBox.SelectionIndent = 0;
                TextBox.SelectionFont = _SenderFont;
                TextBox.SelectionColor = color;
                TextBox.SelectedText = sender + " : " + (_Form.btnShowDate.Checked ? DateTime.Now.ToString() + "\n" : string.Empty);
                TextBox.SelectionIndent = 10;
                TextBox.SelectionFont = TextBox.Font;
            }

            /// <summary>
            /// Appends error message
            /// </summary>
            /// <param name="message"></param>
            private void AppendError(string message)
            {
                TextBox.SelectionColor = Color.Gray;
                TextBox.SelectedText = message + "\n";
            }

            /// <summary>
            /// Appends a richtext message
            /// </summary>
            /// <param name="message"></param>
            private void AppendRtf(string message)
            {
                TextBox.SelectedRtf = message;
            }

            #endregion
        }

        /// <summary>
        /// Represents a smiliey information
        /// </summary>
        private class SmileyInfo
        {
            #region Members

            public int ImageIndex = -1;
            public string Text = string.Empty;
            public string Command = string.Empty;

            #endregion

            #region Initialization

            /// <summary>
            /// Initialize a new instance of this class with the provided values
            /// </summary>
            /// <param name="imgIndex"></param>
            /// <param name="text"></param>
            /// <param name="command"></param>
            public SmileyInfo(int imgIndex, string text, string command)
            {
                ImageIndex = imgIndex;
                Text = text;
                Command = command;
            }

            #endregion
        }

        /// <summary>
        /// Represents type of the message
        /// </summary>
        private enum MessageType
        {
            /// <summary>
            /// Message is sended by ME
            /// </summary>
            SendedByMe,
            /// <summary>
            /// Message is sended by target person
            /// </summary>
            SendedByTarget,
            /// <summary>
            /// Message is a server error
            /// </summary>
            ServerError,
            /// <summary>
            /// Message is a command error
            /// </summary>
            CommandError
        }

        /// <summary>
        /// Customized <see cref="ToolStrip"/> for messaging menu
        /// </summary>
        private class MessageStrip : ToolStrip
        {
            #region Members

            internal OleRichTextBox Message;

            #endregion

            #region Initialization

            /// <summary>
            /// Initialize a new instance of this class
            /// </summary>
            public MessageStrip()
            {
                var button = new ToolStripDropDownButton();
                button.ToolTipText = "Insert Smiley";
                button.ImageTransparentColor = Color.White;

                var smileyMenu = new SmileyMenuItem(this, button);
                ImageList = _SmileyImages;

                Items.Add(button);
                button.DropDownItems.Add(smileyMenu);
                button.Image = _SmileyImages.Images[smileyMenu.ImageIndex];

                GripStyle = ToolStripGripStyle.Hidden;
            }

            #endregion
        }

        /// <summary>
        /// Represents a menu item for inserting smilies
        /// </summary>
        private class SmileyMenuItem : ToolStripControlHost
        {
            #region Members

            private SmileyInfo _Data;
            private SmileyPanel _Panel;
            private MessageStrip _ToolStrip;
            private ToolStripDropDownButton _Button;

            #endregion

            #region Properties

            /// <summary>
            /// Gets or sets the selected smiley data
            /// </summary>
            internal SmileyInfo Data
            {
                get { return _Data; }
                set
                {
                    _Data = value;
                    if (value == null)
                        ImageIndex = _SmileyDatas[":)"].ImageIndex;
                    else
                        ImageIndex = value.ImageIndex;
                }
            }

            #endregion

            #region Initialization

            /// <summary>
            /// Initialize a new instance of this class with the provided values
            /// </summary>
            /// <param name="toolStrip"></param>
            /// <param name="button"></param>
            public SmileyMenuItem(MessageStrip toolStrip, ToolStripDropDownButton button)
                : base(new SmileyPanel())
            {
                _Button = button;
                _ToolStrip = toolStrip;
                Data = _SmileyDatas[":)"];
                _Panel = (SmileyPanel)Control;
            }

            #endregion

            #region Public Methods

            public override Size GetPreferredSize(Size constrainingSize)
            {
                return Control.Size;
            }

            #endregion

            #region Protected Methods

            /// <summary>
            /// Handles on click
            /// </summary>
            /// <param name="e"></param>
            protected override void OnClick(EventArgs e)
            {
                base.OnClick(e);
                DoPerformClick();
                _Button.HideDropDown();
                _ToolStrip.Message.Focus();
            }

            #endregion

            #region Private Methods

            /// <summary>
            /// Performs click operation on a menu item
            /// </summary>
            private void DoPerformClick()
            {
                if (_Panel.OverItem == null)
                {
                    Data = null;
                }
                else
                {
                    Data = _Panel.OverItem.Data;
                }

                _ToolStrip.Message.InsertImage(_SmileyImages.Images[ImageIndex]);
            }

            #endregion
        }

        /// <summary>
        /// Represents a panel control for hosting smiley menu items
        /// </summary>
        private class SmileyPanel : Panel
        {
            #region Types

            /// <summary>
            /// Represents smiley item
            /// </summary>
            internal class SmileyItem
            {
                #region Properties

                /// <summary>
                /// Gets or sets the smiley bounds in the panel
                /// </summary>
                public Rectangle Bounds { get; set; }

                /// <summary>
                /// Gets or sets the smiley image
                /// </summary>
                public Image Image { get; set; }

                /// <summary>
                /// Gets or sets the smiley information
                /// </summary>
                public SmileyInfo Data { get; set; }

                #endregion
            }

            #endregion

            #region Members

            private List<SmileyItem> _Items;
            private SmileyItem _OverItem;
            private ToolTip _Tooltip;

            #endregion

            #region Properties

            /// <summary>
            /// Gets or sets the over item
            /// </summary>
            internal SmileyItem OverItem
            {
                get { return _OverItem; }
                set
                {
                    if (_OverItem == value)
                        return;

                    GraphicsPath path = new GraphicsPath();
                    if (_OverItem != null)
                        path.AddRectangle(_OverItem.Bounds);
                    _OverItem = value;
                    if (_OverItem != null)
                        path.AddRectangle(_OverItem.Bounds);

                    Region rgn = new Region(path);
                    Invalidate(rgn);
                    rgn.Dispose();
                    path.Dispose();

                    if (value == null)
                        _Tooltip.SetToolTip(this, null);
                    else
                        _Tooltip.SetToolTip(this, value.Data.Text + " " + value.Data.Command);
                }
            }

            /// <summary>
            /// Gets the number of items per line
            /// </summary>
            public int ItemsPerLine { get; private set; } = 15;

            /// <summary>
            /// Gets the size of one smiley menu item
            /// </summary>
            public Size OneItemSize { get; private set; } = new Size(24, 24);

            #endregion

            #region Initialization

            /// <summary>
            /// Initialize a new instance of this class
            /// </summary>
            public SmileyPanel()
            {
                base.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
                base.SetStyle(ControlStyles.StandardClick, true);
                base.BackColor = Color.Transparent;
                _Items = new List<SmileyItem>();
                _Tooltip = new ToolTip();

                var x = 3;
                var y = 3;

                var width = 6 + (ItemsPerLine * OneItemSize.Width);
                var height = 6 + Convert.ToInt32(Math.Ceiling((double)_SmileyDatas.Count / (double)ItemsPerLine)) * OneItemSize.Height;

                Size = new Size(width, height);
                var pos = 0;

                foreach (string key in _SmileyDatas.Keys)
                {
                    SmileyInfo data = _SmileyDatas[key];
                    SmileyItem item = new SmileyItem
                    {
                        Data = data,
                        Image = _SmileyImages.Images[data.ImageIndex],
                        Bounds = new Rectangle(x, y, OneItemSize.Width, OneItemSize.Height)
                    };
                    pos++;

                    if (pos == ItemsPerLine)
                    {
                        y += OneItemSize.Height;
                        x = 3;
                        pos = 0;
                    }
                    else
                    {
                        x += OneItemSize.Width;
                    }
                    _Items.Add(item);
                }
            }

            #endregion

            #region Protected Methods

            protected override void OnPaint(PaintEventArgs e)
            {
                Point pt = PointToClient(Cursor.Position);
                using (Pen normalBorder = new Pen(Color.DarkGray))
                {
                    using (Pen highlight = new Pen(Color.Navy))
                    {
                        for (int i = 0; i < _Items.Count; i++)
                        {
                            DrawItem(e.Graphics, _Items[i], pt, normalBorder, highlight);
                        }
                    }
                }
            }

            protected override void OnMouseLeave(EventArgs e)
            {
                base.OnMouseLeave(e);
                OverItem = null;
            }

            protected override void OnMouseMove(MouseEventArgs e)
            {
                base.OnMouseMove(e);
                for (int i = 0; i < _Items.Count; i++)
                {
                    if (_Items[i].Bounds.Contains(e.X, e.Y))
                    {
                        OverItem = _Items[i];
                        return;
                    }
                }

                OverItem = null;
            }

            #endregion

            #region Private Methods

            /// <summary>
            /// Draws an item
            /// </summary>
            /// <param name="g"></param>
            /// <param name="item"></param>
            /// <param name="pt"></param>
            /// <param name="normal"></param>
            /// <param name="highlight"></param>
            private void DrawItem(Graphics g, SmileyItem item, Point pt, Pen normal, Pen highlight)
            {
                var rct = item.Bounds;
                rct.Inflate(-1, -1);
                if (item.Bounds.Contains(pt))
                {
                    using (var br = new LinearGradientBrush(item.Bounds, Color.MistyRose, Color.Yellow, LinearGradientMode.Vertical))
                    {
                        g.FillRectangle(br, rct);
                    }
                    g.DrawRectangle(highlight, rct);
                    rct.Inflate(-2, -2);
                    g.DrawImage(item.Image, rct.X, rct.Y, 19, 19);
                }
                else
                {
                    g.DrawRectangle(normal, rct);
                    rct.Inflate(-2, -2);
                    g.DrawImage(item.Image, rct.X, rct.Y, 19, 19);
                }
            }

            #endregion
        }

        /// <summary>
        /// MAnages the forms state
        /// </summary>
        private class FormWritingStateManager : IDisposable
        {
            #region Members

            private FormMsgScreen _Form;
            private System.Timers.Timer timer;
            private DateTime lastKeyPress = DateTime.Now;
            private bool _State;

            #endregion

            #region Initialization

            /// <summary>
            /// Initialize a new instance of this class
            /// </summary>
            /// <param name="form"></param>
            public FormWritingStateManager(FormMsgScreen form)
            {
                _Form = form;

                timer = new System.Timers.Timer
                {
                    Interval = 300
                };
                timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
                _Form.rtfMessage.KeyDown += new KeyEventHandler(rtfMessage_KeyDown);
            }

            #endregion

            #region Public Methods

            /// <summary>
            /// Resets writing state
            /// </summary>
            internal void Reset()
            {
                timer.Enabled = false;
                SetWritingState(false);
            }

            /// <summary>
            /// Releases all unmanaged resources
            /// </summary>
            public void Dispose()
            {
                Dispose(true);
            }

            #endregion

            #region Protected Methods

            protected virtual void Dispose(bool disposing)
            {
                if (disposing)
                {
                    timer.Enabled = false;
                    SetWritingState(false);
                    _Form.rtfMessage.KeyDown -= new KeyEventHandler(rtfMessage_KeyDown);
                }
            }

            #endregion

            #region Private Methods

            private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
            {
                if (DateTime.Now.Subtract(lastKeyPress).TotalMilliseconds > 1000)
                {
                    SetWritingState(false);
                    timer.Enabled = false;
                }
            }

            /// <summary>
            /// Sets writing state
            /// </summary>
            /// <param name="state"></param>
            private void SetWritingState(bool state)
            {
                if (_State == state)
                    return;

                _State = state;

                NetCommand command = new NetCommand();
                command.Name = CommandNames.INFORM_WRITING_STATE;
                command.Parameters.Add(new ParameterString("un", _Form.TargetUser));
                command.Parameters.Add(new ParameterBoolean("st", state));
                _Form._SessionManager.AddCommand(command);
            }

            private void rtfMessage_KeyDown(object sender, KeyEventArgs e)
            {
                lastKeyPress = DateTime.Now;
                if (!timer.Enabled)
                {
                    timer.Enabled = true;
                    SetWritingState(true);
                }
            }

            #endregion
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct FLASHWINFO
        {
            public uint cbSize;
            public IntPtr hWnd;
            public uint flags;
            public uint uCount;
            public uint dwTimeout;
        }

        #endregion

        #region Members

        private const uint FLASHW_TRAY = 0x00000002;
        private const uint FLASHW_CAPTION = 0x00000001;
        private const uint FLASHW_STOP = 0;
        private const uint FLASHW_TIMER = 0x00000004;
        private const uint FLASHW_TIMERNOFG = 0x0000000C;
        private const uint FLASHW_ALL = 0x00000003;

        private static ImageList _SmileyImages;
        private static Dictionary<string, SmileyInfo> _SmileyDatas;
        private static Dictionary<char, List<SmileyInfo>> _KeywordChars;

        private string _TargetUser = string.Empty;
        private MessagingSessionsManager _SessionManager;
        private MessageManager _MessageManager;
        private FormWritingStateManager _StateManager;
        private DateTime _LastMessageDate = DateTime.MinValue;
        private bool _IsWritingMessage;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the target user
        /// </summary>
        internal string TargetUser
        {
            get { return _TargetUser; }
            private set
            {
                _TargetUser = value;
                base.Text = "IM - " + value;
            }
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Static initializer
        /// </summary>
        static FormMsgScreen()
		{
			_SmileyImages = new ImageList();
			_SmileyImages.ColorDepth = ColorDepth.Depth32Bit;
			_SmileyImages.TransparentColor = Color.Transparent;
			_SmileyImages.ImageSize = new Size(19, 19);
			_SmileyImages.Images.AddStrip(Properties.Resources.Smiley);

			_SmileyDatas = new Dictionary<string,SmileyInfo>();
			SmileyInfo[] datas = new SmileyInfo[45];
			datas[0] = new SmileyInfo(13, "Smile", ":)");
			datas[1] = new SmileyInfo(14, "Sad", ":(");
			datas[2] = new SmileyInfo(1, "Angry", ":@");
			datas[3] = new SmileyInfo(19, ":P", ":P"); // 4
			datas[4] = new SmileyInfo(4, "Confused", ":S");
			datas[5] = new SmileyInfo(5, "Cry", ":'(");
			datas[6] = new SmileyInfo(7, "Shame", ":$");
			datas[7] = new SmileyInfo(8, "E-Mail", "(E)");//8
			datas[8] = new SmileyInfo(15, "Attractive", "(H)"); //9
			datas[9] = new SmileyInfo(16, "Big Smile", ":D"); //10
			datas[10] = new SmileyInfo(12, ":-O", ":-O"); //11
			datas[11] = new SmileyInfo(21, "Wink", ";)");
			datas[12] = new SmileyInfo(20, "Disappointment", ":|");
			datas[13] = new SmileyInfo(18, "(Y)", "(Y)");
			datas[14] = new SmileyInfo(17, "(N)", "(N)");
			datas[15] = new SmileyInfo(0, "Angel", "(A)"); //16
			datas[16] = new SmileyInfo(9, "Heart", "(L)");//17
			datas[17] = new SmileyInfo(2, "(U)", "(U)");
			datas[18] = new SmileyInfo(3, "Paste", "(^)");
			datas[19] = new SmileyInfo(10, "Kiss", "(K)");
			datas[20] = new SmileyInfo(11, "Good Idea", "(I)");
			datas[21] = new SmileyInfo(6, "Satan", "(6)");
			datas[22] = new SmileyInfo(22, "Sleepy", "|-)");
			datas[23] = new SmileyInfo(23, "Dont Tell", ":-#");
			datas[24] = new SmileyInfo(24, "Toothy", "8o|");
			datas[25] = new SmileyInfo(25, "Whisper", ":-*");
			datas[26] = new SmileyInfo(26, "Sick", "+o(");
			datas[27] = new SmileyInfo(27, "Ironic", "^o)");
			datas[28] = new SmileyInfo(28, "Tray", "(pl)");
			datas[29] = new SmileyInfo(29, "Bowl", "(||)");
			datas[30] = new SmileyInfo(30, "Be right back", "(brb)");
			datas[31] = new SmileyInfo(31, "Cell Phone", "(mp)");
			datas[32] = new SmileyInfo(32, "Rain", "(st)");
			datas[33] = new SmileyInfo(33, "Tortoise", "(tu)");
			datas[34] = new SmileyInfo(34, "Plane", "(ap)");
			datas[35] = new SmileyInfo(35, "Car", "(au)");
			datas[36] = new SmileyInfo(36, "Party", "<:o)");
			datas[37] = new SmileyInfo(37, "Storm", "(li)");
			datas[38] = new SmileyInfo(38, "(ip)", "(ip)");
			datas[39] = new SmileyInfo(39, "(G)", "(G)");
			datas[40] = new SmileyInfo(40, "Camera", "(P)");
			datas[41] = new SmileyInfo(41, "Phone", "(T)");
			datas[42] = new SmileyInfo(42, "Coffee Cup", "(C)");
			datas[43] = new SmileyInfo(43, "Footbal Ball", "(so)");
			datas[44] = new SmileyInfo(44, "Pizza", "(pi)");

			foreach (SmileyInfo data in datas)
			{
				_SmileyDatas.Add(data.Command, data);
			}

			_KeywordChars = new Dictionary<char,List<SmileyInfo>>();
			foreach (SmileyInfo data in datas)
			{
				foreach (char c in data.Command)
				{
					List<SmileyInfo> list = null;
					if (_KeywordChars.ContainsKey(c))
						list = _KeywordChars[c];
					if (list == null)
					{
                        list = new List<SmileyInfo>
                        {
                            data
                        };
                        _KeywordChars.Add(c, list);
					}
					else
					{
						list.Add(data);
					}
				}
			}
		}

        /// <summary>
        /// Initialize a new instance of this class
        /// </summary>
		public FormMsgScreen()
		{
			InitializeComponent();

			// set messaging rtf box
			menuStrip.Message = rtfMessage;
			_StateManager = new FormWritingStateManager(this);

			rtfMessage.KeyDown += new KeyEventHandler(rtfMessage_KeyDown);
		}

        /// <summary>
        /// Initialize a new instance of this class
        /// </summary>
        /// <param name="manager"></param>
        internal FormMsgScreen(MessagingSessionsManager manager, string userName) : this()
		{
			_SessionManager = manager;
            TargetUser = userName;
			_MessageManager = new MessageManager(this);

			_SessionManager.MainForm.client1.CommandExecutionManager.Subscribe(
				CommandNames.SEND_MESSAGE, new CommandExecuteHandler(OnSendCommandExecuted));
		}

        #endregion

        #region Public Methods

        /// <summary>
        /// Forces Get Picture command for the target client
        /// </summary>
        internal void InvokeRefreshImage()
        {
            BeginInvoke(new EventHandler(btnRefresh_Click), new object[] { null, null });
        }

        /// <summary>
        /// User connection state is changed
        /// </summary>
        /// <param name="connected"></param>
        internal void UserConnectionStateChanged(bool connected)
        {
            lblWarning.Text = "User disconnected";
            lblWarning.Visible = !connected;
            rtfMessage.Enabled = connected;
            rtfMessages.Enabled = connected;
            menuStrip.Enabled = connected;
            btnSend.Enabled = connected;
        }

        /// <summary>
        /// User state is changing
        /// </summary>
        /// <param name="value"></param>
        internal void InformState(bool value)
        {
            _IsWritingMessage = value;
            SetBottomLabelText();
        }

        /// <summary>
        /// Appends a message from user
        /// </summary>
        /// <param name="msg"></param>
        internal void AddMessage(string msg)
        {
            _MessageManager.AppendMessage(msg, MessageType.SendedByTarget);

            FlashWindow(Handle);
            rtfMessage.Focus();

            _LastMessageDate = DateTime.Now;
            SetBottomLabelText();
        }

        /// <summary>
        /// Appends a server error
        /// </summary>
        /// <param name="msg"></param>
        internal void AddServerError(string msg)
        {
            _MessageManager.AppendMessage(msg, MessageType.ServerError);
            rtfMessage.Focus();
        }

        /// <summary>
        /// Flashes window
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        internal static bool FlashWindow(IntPtr hWnd)
        {
            FLASHWINFO pfwi = new FLASHWINFO();
            pfwi.cbSize = Convert.ToUInt32(Marshal.SizeOf(pfwi));
            pfwi.hWnd = hWnd;
            pfwi.flags = FLASHW_TRAY;
            pfwi.uCount = 5;
            pfwi.dwTimeout = 0;

            return FlashWindowEx(ref pfwi);
        }

        #endregion

        #region Protected Methods

        protected override void OnActivated(EventArgs e)
		{
			base.OnActivated(e);
			rtfMessage.Focus();
		}
        
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			RefreshImage();
		}

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            rtfMessage.Focus();
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            rtfMessage.Focus();
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            rtfMessage.Focus();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            rtfMessage.Focus();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _MessageManager.Dispose();
            base.OnClosing(e);
        }

        #endregion

        #region Private Methods

        private void RefreshImage()
		{
			var command = new NetCommand(CommandNames.GET_PICTURE);
			command.Parameters.Add(new ParameterString("un", TargetUser));
			command.Executed += new EventHandler(GetPictureCommand_Executed);
			_SessionManager.AddCommand(command);
		}

        private void GetPictureCommand_Executed(object sender, EventArgs e)
		{
            var command = sender as NetCommand;
            if (command == null || command.Response == null)
            {
                return;
            }
			if (command.Response.Okey)
			{
				var rp = command.Response.Parameters["rp"] as ParameterResponse;
				if (rp.Value.Okey)
				{
					var img = rp.Value.Parameters["img"] as ParameterByteArray;
                    if (img.Value == null)
                    {
                        pic.Image = null;
                    }
                    else
                    {
                        pic.Image = Image.FromStream(new MemoryStream(img.Value));
                    }
				}

			}
			else
            {
				pic.Image = null;
			}
		}

        private void SetBottomLabelText()
		{ 
			if (_IsWritingMessage)
			{
				lblWriting.Text = string.Format("'{0}' is writing a message ...", TargetUser);
			}else
			{
				if (_LastMessageDate == DateTime.MinValue)
					lblWriting.Text = string.Empty;
				else
					lblWriting.Text = "Last message received at " + _LastMessageDate.ToShortTimeString();
			}
		}

        private void btnSend_Click(object sender, EventArgs e)
		{
			if (rtfMessage.Text == string.Empty)
				return;

			var command = new NetCommand(CommandNames.SEND_MESSAGE);
			command.Parameters.Add(new ParameterString("un", TargetUser));
			command.Parameters.Add(new ParameterString("msg", rtfMessage.Rtf));

			_SessionManager.AddCommand(command);

			_MessageManager.AppendMessage(rtfMessage.Rtf, MessageType.SendedByMe);
			rtfMessage.Text = string.Empty;
			rtfMessage.Focus();

			_StateManager.Reset();
		}

        private void OnSendCommandExecuted(NetCommand command)
		{
			if (!command.Response.Okey)
			{
				var uname = command.Parameters["un"] as ParameterString;
				if (uname != null && uname.Value == TargetUser)
				{
					var msg = command.Parameters["msg"] as ParameterString;
					BeginInvoke(new EventHandler(OnMessageNotDeliver), new object[] { msg.Value, null });
				}
			}
		}

        private void OnMessageNotDeliver(object sender, EventArgs e)
		{
			var msg = sender as string;
			if (msg == null)
				return;

			_MessageManager.AppendMessage("Following messages could not delivered", MessageType.CommandError);
			_MessageManager.AppendMessage(msg, MessageType.SendedByMe);
		}

		private void rtfMessage_TextChanged(object sender, EventArgs e)
		{
			if (rtfMessage.Text.Length > 0)
				btnSend.Enabled = true;
			else
				btnSend.Enabled = false;
		}

		private void rtfMessage_KeyDown(object sender, KeyEventArgs e)
		{
			if (!e.Shift)
			{
				if ((e.KeyCode == Keys.Enter)
					|| (e.KeyCode == Keys.Return))
				{
					e.Handled = true;

					// Text or Rtf
					if (rtfMessage.Text == string.Empty)
						return;
					btnSend.PerformClick();
				}

			}
		}

		private void rtfMessages_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Control)
			{
				Keys key = (e.KeyData & (~(Keys.ControlKey)));
				switch (key)
				{
					case Keys.C:
					case Keys.V:
						return;
				}
			}
			e.Handled = true;
			rtfMessage.Focus();
		}

		private void rtfMessage_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (_KeywordChars.ContainsKey(e.KeyChar))
			{
				var list = _KeywordChars[e.KeyChar];
				if (list != null)
				{
					var text = rtfMessage.Text;

					var cursor = rtfMessage.SelectionStart;
					text = text.Insert(cursor, e.KeyChar.ToString());
					foreach (SmileyInfo data in list)
					{
						var index = data.Command.IndexOf(e.KeyChar);
						while (index != -1)
						{
							if (ProcessSmileCommand(data, index, text, cursor))
							{
								e.Handled = true;
								InsertSmiley(data);
								return;
							}

							index = data.Command.IndexOf(e.KeyChar, index + 1, data.Command.Length - index - 1);
						}
					}
				}
			}
		}

        private bool ProcessSmileCommand(SmileyInfo data, int pos, string text, int cursor)
		{
			int begin = cursor - pos;
			if (begin < 0)
				return false;
			if (begin + data.Command.Length > text.Length)
				return false;
			string subStr = text.Substring(begin, data.Command.Length);
			if (subStr == data.Command)
			{
				rtfMessage.Select(cursor - pos, data.Command.Length - 1);
				rtfMessage.Cut();
				return true;
			}
			return false;
		}

        private void InsertSmiley(SmileyInfo data)
		{
			if (data == null)
				return;

			rtfMessage.Focus();
			rtfMessage.InsertImage(_SmileyImages.Images[data.ImageIndex]);
		}

		private void btnBolt_Click(object sender, EventArgs e)
		{
			if ((rtfMessage.SelectionFont.Style & FontStyle.Bold) == FontStyle.Bold)
			{
				rtfMessage.SelectionFont = new Font(rtfMessage.SelectionFont,
					rtfMessage.SelectionFont.Style & ~FontStyle.Bold);
			}
			else
			{
				rtfMessage.SelectionFont = new Font(rtfMessage.SelectionFont,
					rtfMessage.SelectionFont.Style | FontStyle.Bold);
			}
		}

		private void btnItalic_Click(object sender, EventArgs e)
		{
			if ((rtfMessage.SelectionFont.Style & FontStyle.Italic) == FontStyle.Italic)
			{
				rtfMessage.SelectionFont = new Font(rtfMessage.SelectionFont,
				rtfMessage.SelectionFont.Style & ~FontStyle.Italic);
			}
			else
			{
				rtfMessage.SelectionFont = new Font(rtfMessage.SelectionFont,
					rtfMessage.SelectionFont.Style | FontStyle.Italic);
			}
		}

		private void btnFont_Click(object sender, EventArgs e)
		{
			fontDialog.Font = rtfMessage.SelectionFont;
			if (fontDialog.ShowDialog() == DialogResult.OK)
			{
				rtfMessage.SelectionFont = fontDialog.Font;
			}
		}

		private void toolStripButton1_Click(object sender, EventArgs e)
		{
			colorDialog.Color = rtfMessage.SelectionColor;
			if (colorDialog.ShowDialog() == DialogResult.OK)
			{
				rtfMessage.SelectionColor = colorDialog.Color;
			}
		}

		private void btnShowDate_Click(object sender, EventArgs e)
		{
			btnShowDate.Checked = !btnShowDate.Checked;
		}

		private void chkSave_Click(object sender, EventArgs e)
		{
			chkSave.Checked = !chkSave.Checked;
		}

		private void btnRefresh_Click(object sender, EventArgs e)
		{
			RefreshImage();
		}

		private void rtfMessage_SelectionChanged(object sender, EventArgs e)
		{
			btnBolt.Checked = ((rtfMessage.SelectionFont.Style & FontStyle.Bold) == FontStyle.Bold);
			btnItalic.Checked = ((rtfMessage.SelectionFont.Style & FontStyle.Italic) == FontStyle.Italic);
		}

		private void rtfMessages_KeyPress(object sender, KeyPressEventArgs e)
		{
			e.Handled = true;
		}

        #endregion

        #region Impports

        [DllImport("user32.dll")]
		public static extern bool FlashWindowEx(ref FLASHWINFO pfwi);

		[DllImport("user32.dll")]
		public static extern IntPtr GetActiveWindow();

        #endregion
	}
}