using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace FlickrNetScreensaver.DrawOptions
{
	/// <summary>
	/// Summary description for AbstractOptions.
	/// </summary>
	public class AbstractOptions : System.Windows.Forms.UserControl
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public AbstractOptions()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call

		}

		virtual public void SaveSettings()
		{
		}

		virtual public void LoadSettings()
		{
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// AbstractOptions
			// 
			this.Name = "AbstractOptions";
			this.Size = new System.Drawing.Size(256, 344);

		}
		#endregion
	}
}
