using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using FlickrNetScreensaver.Properties;

namespace FlickrNetScreensaver.DrawOptions
{
	public class PlainOptions : FlickrNetScreensaver.DrawOptions.AbstractOptions
	{
		private System.Windows.Forms.CheckBox ShowText;
		private System.ComponentModel.IContainer components = null;

		public PlainOptions()
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.ShowText = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// ShowText
			// 
			this.ShowText.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.ShowText.Location = new System.Drawing.Point(8, 16);
			this.ShowText.Name = "ShowText";
			this.ShowText.TabIndex = 0;
			this.ShowText.Text = "Show Text:";
			// 
			// PlainOptions
			// 
			this.Controls.Add(this.ShowText);
			this.Name = "PlainOptions";
			this.Size = new System.Drawing.Size(200, 136);
			this.ResumeLayout(false);

		}
		#endregion

		public override void LoadSettings()
		{
            ShowText.Checked = Settings.Default.PlainShowText;
		}

		public override void SaveSettings()
		{
            Settings.Default.PlainShowText = ShowText.Checked;
		}


	}
}

