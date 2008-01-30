using System;
using System.Drawing;
using System.Windows.Forms;

namespace FlickrNetScreensaver.Drawers
{
	/// <summary>
	/// Summary description for PlainDrawer.
	/// </summary>
	public class PostcardDrawer: IPictureDrawer
	{
		private FScreensaver _pictureForm;
		private PictureBox _pictureBox;

		private string color;
		private Color realColor;
		private int angle;
		private bool randomAngle;
		private bool alternateAngle;

		public PostcardDrawer()
		{
			color = Settings.Get("Postcard.Color");
			if( color == null || color.Length == 0 )
			{
				realColor = Color.WhiteSmoke;
			}
			else
			{
				string[] rgb = color.Split(',');
				realColor = Color.FromArgb(int.Parse(rgb[0]),int.Parse(rgb[1]),int.Parse(rgb[2]));
			}

			if( Settings.Contains("Postcard.Angle") )
			{
				angle = int.Parse(Settings.Get("Postcard.Angle"));
			}
			else
			{
				angle = 10;
			}

			if( Settings.Contains("Postcard.Alternate") )
			{
				alternateAngle = bool.Parse(Settings.Get("Postcard.Alternate"));
			}

			if( Settings.Contains("Postcard.RandomAngle") )
			{
				randomAngle = bool.Parse(Settings.Get("Postcard.RandomAngle"));
			}
		}

		public void Initialise(FScreensaver pictureForm)
		{
			_pictureForm = pictureForm;

			_pictureBox = new PictureBox();
			_pictureForm.Controls.Add(_pictureBox);
			_pictureBox.SendToBack();

			_pictureBox.MouseUp += new MouseEventHandler(_pictureForm.FScreensaver_MouseMove);
			_pictureBox.MouseDown += new MouseEventHandler(_pictureForm.FScreensaver_MouseMove);
			_pictureBox.MouseMove += new MouseEventHandler(_pictureForm.FScreensaver_MouseMove);
		}

		public void ChangeImage(System.Drawing.Image img, FlickrNet.Photo photo)
		{
			int width = _pictureForm.Width;
			int height = _pictureForm.Height;
			
			// Calculate any rotational angle (in degrees)
			int newAngle = angle;
			Random rand = new Random();
			if( randomAngle ) newAngle = rand.Next(0, newAngle+1);
			if( alternateAngle && rand.Next(0,2) == 1) newAngle = -newAngle;

			// Calculate Border
			double borderSize = Math.Max(img.Height * 0.1, img.Width * 0.1);

			// Convert to radians (rad = degrees * PI / 180)
			// Bit of trig to calculate the new width/height of final rotated image
			double currentAngleRadians = Math.Atan(img.Height * 1.0 / img.Width);
			double newAngleRadians = Math.Abs(newAngle) * Math.PI / 180.0;
			// Height and Width of rotated image.
			// * 1.01 for a bit of 'err on size of caution'-ing
			double nHeight = 1.01 * (img.Height + borderSize) * Math.Sin(currentAngleRadians + newAngleRadians)/Math.Sin(currentAngleRadians);
			double nWidth = 1.01 * (img.Width + borderSize) * Math.Cos(currentAngleRadians - newAngleRadians)/Math.Cos(currentAngleRadians);

			// Scale if the height and width exceed the max height and width
			double heightRatio = nHeight / height;
			double widthRatio = nWidth / width;

			// if either ratio is greater than 1
			double maxRatio = Math.Max(heightRatio, widthRatio);
			double scale = 1.0;
			if( maxRatio >= 1 )
			{
				scale = 1/maxRatio;
			}

            Image finalBmp;

			// create bitmap with white background
            using (Image interBmp = new Bitmap((int)(img.Width + borderSize), (int)(img.Height + borderSize)))
            {
                using (Graphics interG = Graphics.FromImage(interBmp))
                {
                    interG.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    interG.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    interG.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

                    // Fill white
                    interG.FillRectangle(new SolidBrush(realColor), 0, 0, interBmp.Width, interBmp.Height);

                    // Draw image in center of bitmap
                    interG.DrawImage(img, (interBmp.Width - img.Width) / 2, (interBmp.Height - img.Height) / 2, img.Width, img.Height);

                    // clear resources
                    interG.Dispose();
                }

                //interBmp.Save(@"C:\inter.jpg");


                // Create final bitmap full screen
                finalBmp = new Bitmap((int)Math.Ceiling(nWidth), (int)Math.Ceiling(nHeight));
                using (Graphics finalG = Graphics.FromImage(finalBmp))
                {
                    finalG.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    finalG.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    finalG.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

                    // Set origin
                    finalG.TranslateTransform(finalBmp.Width / 2, finalBmp.Height / 2);
                    // Rotate
                    finalG.RotateTransform(newAngle);
                    finalG.ScaleTransform((float)scale, (float)scale);
                    // Reset origin
                    finalG.TranslateTransform(-finalBmp.Width / 2, -finalBmp.Height / 2);

                    // Draw Image
                    finalG.DrawImage(interBmp, (finalBmp.Width - interBmp.Width) / 2, (finalBmp.Height - interBmp.Height) / 2, interBmp.Width, interBmp.Height);

                    finalG.Dispose();
                }
                interBmp.Dispose();
            }

			DrawerAssist.FinalImage(ref finalBmp, width, height);

            DrawImage(finalBmp.Width, finalBmp.Height, (width - finalBmp.Width) / 2, (height - finalBmp.Height) / 2, finalBmp);
		}

        private delegate void DrawImageDelegate(int width, int height, int left, int top, Image img);

        private void DrawImage(int width, int height, int left, int top, Image img)
        {
            if (_pictureForm.InvokeRequired)
            {
                _pictureForm.Invoke(new DrawImageDelegate(DrawImage), new object[] { width, height, left, top, img });
                return;
            }

            if (_pictureBox.Image != null)
                _pictureBox.Image.Dispose();

            _pictureBox.Width = width;
            _pictureBox.Height = height;
            _pictureBox.Left = left;
            _pictureBox.Top = top;
            _pictureBox.Image = img;

        }

	}
}
