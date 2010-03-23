using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using FlickrNetScreensaver.Properties;

namespace FlickrNetScreensaver.DrawOptions
{
	public class StackOptions : FlickrNetScreensaver.DrawOptions.AbstractOptions
	{
		protected System.Windows.Forms.Label label1;
        protected System.Windows.Forms.ColorDialog colorDialog1;
        protected System.Windows.Forms.Label label2;
        protected System.Windows.Forms.ComboBox Angle;
        protected System.Windows.Forms.Button ColorPicker;
        protected System.Windows.Forms.CheckBox AlternateLeftRight;
        protected System.Windows.Forms.CheckBox RandomAngle;
        protected System.Windows.Forms.PictureBox colorPicture;
        protected ComboBox BorderSize;
        protected Label label3;
        protected System.ComponentModel.IContainer components = null;

        public StackOptions()
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
            this.BorderSize = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.colorPicture)).BeginInit();
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
            // borderSize
            // 
            this.BorderSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.BorderSize.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10"});
            this.BorderSize.Location = new System.Drawing.Point(96, 118);
            this.BorderSize.Name = "borderSize";
            this.BorderSize.Size = new System.Drawing.Size(56, 21);
            this.BorderSize.TabIndex = 9;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 118);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Border Size:";
            // 
            // StackOptions
            // 
            this.Controls.Add(this.BorderSize);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.colorPicture);
            this.Controls.Add(this.RandomAngle);
            this.Controls.Add(this.AlternateLeftRight);
            this.Controls.Add(this.ColorPicker);
            this.Controls.Add(this.Angle);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "StackOptions";
            this.Size = new System.Drawing.Size(208, 192);
            ((System.ComponentModel.ISupportInitialize)(this.colorPicture)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		public override void LoadSettings()
		{
    		SetColor(Settings.Default.StackColor);
            Angle.SelectedItem = Settings.Default.StackAngle.ToString();
            RandomAngle.Checked = Settings.Default.StackRandomAngle;
            AlternateLeftRight.Checked = Settings.Default.StackAlternate;
            BorderSize.SelectedItem = Settings.Default.StackBorder.ToString();
		}

		public override void SaveSettings()
		{
            Settings.Default.StackColor = theColor;
            Settings.Default.StackAngle = int.Parse(Angle.SelectedItem.ToString());
            Settings.Default.StackRandomAngle = RandomAngle.Checked;
            Settings.Default.StackAlternate = AlternateLeftRight.Checked;
            Settings.Default.StackBorder = double.Parse(BorderSize.Text);
		}

		private void ColorPicker_Click(object sender, System.EventArgs e)
		{
			colorDialog1.Color = theColor;

			if( colorDialog1.ShowDialog() != DialogResult.OK ) return;

			SetColor(colorDialog1.Color);
		}

		protected Color theColor;

        protected void SetColor(Color color)
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

