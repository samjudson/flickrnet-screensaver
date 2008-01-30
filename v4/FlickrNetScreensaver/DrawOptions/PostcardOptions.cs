using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using FlickrNetScreensaver.Properties;

namespace FlickrNetScreensaver.DrawOptions
{
	public class PostcardOptions : FlickrNetScreensaver.DrawOptions.AbstractOptions
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ColorDialog colorDialog1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox Angle;
		private System.Windows.Forms.Button ColorPicker;
		private System.Windows.Forms.CheckBox AlternateLeftRight;
		private System.Windows.Forms.CheckBox RandomAngle;
		private System.Windows.Forms.PictureBox colorPicture;
		private System.ComponentModel.IContainer components = null;

		public PostcardOptions()
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();
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
			this.label1 = new System.Windows.Forms.Label();
			this.colorDialog1 = new System.Windows.Forms.ColorDialog();
			this.label2 = new System.Windows.Forms.Label();
			this.Angle = new System.Windows.Forms.ComboBox();
			this.ColorPicker = new System.Windows.Forms.Button();
			this.AlternateLeftRight = new System.Windows.Forms.CheckBox();
			this.RandomAngle = new System.Windows.Forms.CheckBox();
			this.colorPicture = new System.Windows.Forms.PictureBox();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(80, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "Paper Colour:";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 40);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(72, 16);
			this.label2.TabIndex = 2;
			this.label2.Text = "Angle:";
			// 
			// Angle
			// 
			this.Angle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.Angle.Items.AddRange(new object[] {
													   "5",
													   "10",
													   "15",
													   "20",
													   "25",
													   "30",
													   "35",
													   "40",
													   "45",
													   "50",
													   "55",
													   "60",
													   "65",
													   "70",
													   "75",
													   "80",
													   "85",
													   "90"});
			this.Angle.Location = new System.Drawing.Point(96, 40);
			this.Angle.Name = "Angle";
			this.Angle.Size = new System.Drawing.Size(56, 21);
			this.Angle.TabIndex = 3;
			// 
			// ColorPicker
			// 
			this.ColorPicker.Location = new System.Drawing.Point(168, 16);
			this.ColorPicker.Name = "ColorPicker";
			this.ColorPicker.Size = new System.Drawing.Size(16, 16);
			this.ColorPicker.TabIndex = 4;
			this.ColorPicker.Text = ".";
			this.ColorPicker.Click += new System.EventHandler(this.ColorPicker_Click);
			// 
			// AlternateLeftRight
			// 
			this.AlternateLeftRight.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.AlternateLeftRight.Location = new System.Drawing.Point(8, 64);
			this.AlternateLeftRight.Name = "AlternateLeftRight";
			this.AlternateLeftRight.Size = new System.Drawing.Size(144, 24);
			this.AlternateLeftRight.TabIndex = 5;
			this.AlternateLeftRight.Text = "Alternate Left/Right:";
			// 
			// RandomAngle
			// 
			this.RandomAngle.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.RandomAngle.Location = new System.Drawing.Point(8, 88);
			this.RandomAngle.Name = "RandomAngle";
			this.RandomAngle.Size = new System.Drawing.Size(144, 24);
			this.RandomAngle.TabIndex = 6;
			this.RandomAngle.Text = "Make Angle Random:";
			// 
			// colorPicture
			// 
			this.colorPicture.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.colorPicture.Location = new System.Drawing.Point(96, 16);
			this.colorPicture.Name = "colorPicture";
			this.colorPicture.Size = new System.Drawing.Size(72, 16);
			this.colorPicture.TabIndex = 7;
			this.colorPicture.TabStop = false;
			// 
			// PostcardOptions
			// 
			this.Controls.Add(this.colorPicture);
			this.Controls.Add(this.RandomAngle);
			this.Controls.Add(this.AlternateLeftRight);
			this.Controls.Add(this.ColorPicker);
			this.Controls.Add(this.Angle);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Name = "PostcardOptions";
			this.Size = new System.Drawing.Size(208, 192);
			this.ResumeLayout(false);

		}
		#endregion

		public override void LoadSettings()
		{
    		SetColor(Settings.Default.PostcardColor);

            Angle.SelectedItem = Settings.Default.PostcardAngle;
            RandomAngle.Checked = Settings.Default.PostcardRandomAngle;
            AlternateLeftRight.Checked = Settings.Default.PostcardAlternate;
		}

		public override void SaveSettings()
		{
            Settings.Default.PostcardColor = theColor;
            Settings.Default.PostcardAngle = Angle.SelectedItem.ToString();
            Settings.Default.PostcardRandomAngle = RandomAngle.Checked;
            Settings.Default.PostcardAlternate = AlternateLeftRight.Checked;
		}

		private void ColorPicker_Click(object sender, System.EventArgs e)
		{
			colorDialog1.Color = theColor;

			if( colorDialog1.ShowDialog() != DialogResult.OK ) return;

			SetColor(colorDialog1.Color);
		}

		private Color theColor;

		private void SetColor(Color color)
		{
			theColor = color;

            Image img = new Bitmap(colorPicture.Width, colorPicture.Height);
            using (Brush b = new SolidBrush(color))
            {
                using (Graphics g = Graphics.FromImage(img))
                {
                    g.FillRectangle(b, 0, 0, img.Width, img.Height);
                    g.Dispose();
                }
                b.Dispose();
            }

			if( colorPicture.Image != null )
			{
				colorPicture.Image.Dispose();
			}

			colorPicture.Image = img;
		}

	}
}

