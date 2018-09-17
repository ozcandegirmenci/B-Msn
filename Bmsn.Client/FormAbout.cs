using System;
using System.Windows.Forms;

namespace Bmsn.Client
{
    /// <summary>
    /// About form
    /// </summary>
    public partial class FormAbout : Form
	{
        #region Initialization

        /// <summary>
        /// Initialize a new instance of this class
        /// </summary>
        public FormAbout()
		{
			InitializeComponent();
		}

        #endregion

        #region Private Methods

        private void btnClose_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			System.Diagnostics.Process.Start(@"http://www.ozcandegirmenci.com");
		}

        #endregion
    }
}