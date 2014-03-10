using System;
using System.Drawing;
using System.Windows.Forms;
using FlickrNetScreensaver.Properties;

namespace FlickrNetScreensaver.Drawers
{
	/// <summary>
	/// Summary description for PlainDrawer.
	/// </summary>
	public class PostcardDrawer: IPictureDrawer
	{
		private FScreensaver _pictureForm;
		private PictureBox _pictureBox;

		private readonly Color _realColor;
		private readonly int _angle;
		private readonly bool _randomAngle;
		private readonly bool _alternateAngle;

		public PostcardDrawer()
		{
            _realColor = Settings.Default.PostcardColor;
            if (!int.TryParse(Settings.Default.PostcardAngle, out _angle)) _angle = 10;
            _alternateAngle = Settings.Default.PostcardAlternate;
            _randomAngle = Settings.Default.PostcardRandomAngle;
		}

		public void Initialise(FScreensaver pictureForm)
		{
			_pictureForm = pictureForm;

			_pictureBox = new PictureBox();
			_pictureForm.Controls.Add(_pictureBox);
			_pictureBox.SendToBack();

			_pictureBox.MouseUp += _pictureForm.FScreensaverMouseMove;
			_pictureBox.MouseDown += _pictureForm.FScreensaverMouseMove;
			_pictureBox.MouseMove += _pictureForm.FScreensaverMouseMove;
		}

		public void ChangeImage(Image img, FlickrNet.Photo photo)
		{
			var width = _pictureForm.Width;
			var height = _pictureForm.Height;
			
			// Calculate any rotational angle (in degrees)
			var newAngle = _angle;
			var rand = new Random();
			if( _randomAngle ) newAngle = rand.Next(0, newAngle+1);
			if( _alternateAngle && rand.Next(0,2) == 1) newAngle = -newAngle;

			// Calculate Border
			var borderSize = Math.Max(img.Height * 0.1, img.Width * 0.1);

			// Convert to radians (rad = degrees * PI / 180)
			// Bit of trig to calculate the new width/height of final rotated image
			var currentAngleRadians = Math.Atan(img.Height * 1.0 / img.Width);
			var newAngleRadians = Math.Abs(newAngle) * Math.PI / 180.0;
			// Height and Width of rotated image.
			// * 1.01 for a bit of 'err on size of caution'-ing
			var nHeight = 1.01 * (img.Height + borderSize) * Math.Sin(currentAngleRadians + newAngleRadians)/Math.Sin(currentAngleRadians);
			var nWidth = 1.01 * (img.Width + borderSize) * Math.Cos(currentAngleRadians - newAngleRadians)/Math.Cos(currentAngleRadians);

			// Scale if the height and width exceed the max height and width
			var heightRatio = nHeight / height;
			var widthRatio = nWidth / width;

			// if either ratio is greater than 1
			var maxRatio = Math.Max(heightRatio, widthRatio);
			var scale = 1.0;
			if( maxRatio >= 1 )
			{
				scale = 1/maxRatio;
			}

            Image finalBmp;

			// create bitmap with white background
            using (Image interBmp = new Bitmap((int)(img.Width + borderSize), (int)(img.Height + borderSize)))
            {
                using (var interG = Graphics.FromImage(interBmp))
                {
                    interG.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    interG.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    interG.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

                    // Fill white
                    interG.FillRectangle(new SolidBrush(_realColor), 0, 0, interBmp.Width, interBmp.Height);

                    // Draw image in center of bitmap
                    interG.DrawImage(img, (interBmp.Width - img.Width) / 2, (interBmp.Height - img.Height) / 2, img.Width, img.Height);

                    // clear resources
                    interG.Dispose();
                }

                // Create final bitmap full screen
                finalBmp = new Bitmap((int)Math.Ceiling(nWidth), (int)Math.Ceiling(nHeight));
                using (var finalG = Graphics.FromImage(finalBmp))
                {
                    finalG.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    finalG.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
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
