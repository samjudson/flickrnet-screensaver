using System;
using System.Drawing;
using FlickrNetScreensaver.Properties;

namespace FlickrNetScreensaver.Drawers
{
	/// <summary>
	/// Summary description for DrawerAssist.
	/// </summary>
	public sealed class DrawerAssist
	{
        private static Random rand = new Random();
        private DrawerAssist() { }

		public static void FinalImage(ref Image image, int width, int height)
		{
			Image finalImage = new Bitmap(width, height);
			using( Graphics finalGraphics = Graphics.FromImage(finalImage)) 
			{
				finalGraphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
				finalGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
				finalGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

				bool fillScreen = Settings.Default.DrawerFillScreen;

				double scale = 1.0;

				if( fillScreen )
				{
					double outerRatio = (width * 1.0F / height);
					double innerRatio = (image.Width * 1.0F / image.Height);
					scale = 0.85;
					if( outerRatio <= innerRatio )
					{
						scale = scale * width * 1.0F / image.Width;
					}
					else
					{
						scale = scale * height * 1.0F / image.Height;
					}
				}
				else
				{
					double widthRatio = (width * 1.0F / image.Width);
					double heightRatio = (height * 1.0F / image.Height);
					if( widthRatio > 0.9 || heightRatio > 0.9 )
					{
						scale = 0.85 * Math.Min(1, Math.Max(widthRatio, heightRatio));
					}
				}

				int newWidth = (int)(image.Width * scale);
				int newHeight = (int)(image.Height * scale);

				finalGraphics.DrawImage(image, (width - newWidth) / 2, (height - newHeight) / 2, newWidth, newHeight);

				image.Dispose();
				finalGraphics.Dispose();
			}

			image = finalImage;
		}

        public static Image FinalImage(Image image, Image previousImage, int width, int height, bool fillScreen)
        {
            Image finalImage = new Bitmap(width, height);

            if (previousImage != null)
            {
                finalImage = previousImage;
            }
            
            using (Graphics finalGraphics = Graphics.FromImage(finalImage))
            {
                finalGraphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                finalGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                finalGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                double scale = 1.0;

                if (fillScreen)
                {
                    double outerRatio = (width * 1.0F / height);
                    double innerRatio = (image.Width * 1.0F / image.Height);
                    scale = 0.85;
                    if (outerRatio <= innerRatio)
                    {
                        scale = scale * width * 1.0F / image.Width;
                    }
                    else
                    {
                        scale = scale * height * 1.0F / image.Height;
                    }
                }
                else
                {
                    double widthRatio = (width * 1.0F / image.Width);
                    double heightRatio = (height * 1.0F / image.Height);
                    if (widthRatio > 0.9 || heightRatio > 0.9)
                    {
                        scale = 0.85 * Math.Min(1, Math.Max(widthRatio, heightRatio));
                    }
                }

                int newWidth = (int)(image.Width * scale);
                int newHeight = (int)(image.Height * scale);

                finalGraphics.DrawImage(image, rand.Next(0, width - newWidth), rand.Next(0, height - newHeight), newWidth, newHeight);
                finalGraphics.Dispose();
            }

            return finalImage;
        }
	}
}
