using System;
using System.Drawing;
using System.Windows.Forms;
using FlickrNetScreensaver.Properties;
using System.Drawing.Imaging;

/// <summary>
/// Danilo Migliarino
/// redcloud80@gmail.com
/// 02/16/2010
/// </summary>

namespace FlickrNetScreensaver.Drawers
{
	/// <summary>
	/// Summary description for StackableDrawer.
	/// </summary>
	public class StackDrawer: IPictureDrawer
	{
		private FScreensaver _pictureForm;
        private PictureBox _pictureBox;

        private Color realColor;
		private int angle;
		private bool randomAngle;
		private bool alternateAngle;
        private double border;

        public StackDrawer()
		{
            realColor = Settings.Default.StackColor;
            angle = Settings.Default.StackAngle;
            alternateAngle = Settings.Default.StackAlternate;
            randomAngle = Settings.Default.StackRandomAngle;
            border = Settings.Default.StackBorder;
		}

		public void Initialise(FScreensaver pictureForm)
		{
			_pictureForm = pictureForm;

            _pictureBox = new PictureBox();
            _pictureBox.Dock = DockStyle.Fill;
            _pictureBox.MouseUp += new MouseEventHandler(_pictureForm.FScreensaver_MouseMove);
            _pictureBox.MouseDown += new MouseEventHandler(_pictureForm.FScreensaver_MouseMove);
            _pictureBox.MouseMove += new MouseEventHandler(_pictureForm.FScreensaver_MouseMove);

            _pictureForm.Controls.Add(_pictureBox);
            _pictureBox.SendToBack();
		}

        public void ChangeImage(System.Drawing.Image img, FlickrNet.Photo photo)
        {
            int width = _pictureForm.Width;
            int height = _pictureForm.Height;

            // Calculate any rotational angle (in degrees)
            int newAngle = angle;
            Random rand = new Random();

            if (randomAngle)
            {
                newAngle = rand.Next(0, newAngle + 1);
            }

            if (alternateAngle && rand.Next(0, 2) == 1)
            {
                newAngle = -newAngle;
            }

            // Calculate Border
            double borderSize = Math.Max(img.Height * (border / 100), img.Width * (border / 100));

            // Convert to radians (rad = degrees * PI / 180)
            // Bit of trig to calculate the new width/height of final rotated image
            double currentAngleRadians = Math.Atan(img.Height * 1.0 / img.Width);
            double newAngleRadians = Math.Abs(newAngle) * Math.PI / 180.0;

            // Height and Width of rotated image.
            // * 1.01 for a bit of 'err on size of caution'-ing
            double nHeight = 1.01 * (img.Height + borderSize) * Math.Sin(currentAngleRadians + newAngleRadians) / Math.Sin(currentAngleRadians);
            double nWidth = 1.01 * (img.Width + borderSize) * Math.Cos(currentAngleRadians - newAngleRadians) / Math.Cos(currentAngleRadians);

            // Scale if the height and width exceed the max height and width
            double heightRatio = nHeight / height;
            double widthRatio = nWidth / width;

            // if either ratio is greater than 1
            double maxRatio = Math.Max(heightRatio, widthRatio);
            double scale = 1.0;

            if (maxRatio >= 1)
            {
                scale = 1 / maxRatio;
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

                //interBmp.Save(@"inter.jpg");

                // Create final bitmap full screen
                finalBmp = new Bitmap((int)Math.Ceiling(nWidth), (int)Math.Ceiling(nHeight));
                //finalBmp = new Bitmap(width, height);

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

            _pictureBox.Image = DrawerAssist.FinalImage(finalBmp, _pictureBox.Image, width, height, Settings.Default.DrawerFillScreen);
            _pictureForm.Refresh();
        }
	}
}
