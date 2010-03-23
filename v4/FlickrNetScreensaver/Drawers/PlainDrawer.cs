using System;
using System.Drawing;
using System.Windows.Forms;
using FlickrNet;
using FlickrNetScreensaver.Properties;

namespace FlickrNetScreensaver.Drawers
{
	/// <summary>
	/// Summary description for PlainDrawer.
	/// </summary>
	public class PlainDrawer: IPictureDrawer
	{
		private FScreensaver _pictureForm;
		private PictureBox _pictureBox;

		private string _text;
		private bool _showText;

        public PlainDrawer()
		{
            _showText = Settings.Default.PlainShowText;
		}

		public void SetText(string description, string username)
		{
			if( description == null || description.Length == 0 ) description = "Untitled";

			_text = "[" + description + "] by (" + username + ")";
		}

		#region IPictureDrawer Members

		public void Initialise(FScreensaver pictureForm)
		{
			Logger.Debug("PlainDrawer Initialising");

			_pictureForm = pictureForm;

			_pictureBox = new PictureBox();
			_pictureForm.Controls.Add(_pictureBox);
			_pictureBox.SendToBack();

			_pictureBox.MouseUp += new MouseEventHandler(_pictureForm.FScreensaver_MouseMove);
			_pictureBox.MouseDown += new MouseEventHandler(_pictureForm.FScreensaver_MouseMove);
			_pictureBox.MouseMove += new MouseEventHandler(_pictureForm.FScreensaver_MouseMove);
		}

		public void ChangeImage(System.Drawing.Image img, Photo photo)
		{
			Logger.Debug("PlainDrawer ChangeImage");

			int width = _pictureForm.Width;
			int height = _pictureForm.Height;

			if( _showText )
			{
				SetText(photo.Title, photo.OwnerName);
			}
			

			if( _showText )
			{
				// Draw text on slightly larger image
				Image textImage = new Bitmap(img.Width, img.Height + 25);
				using( Graphics textGraphics = Graphics.FromImage(textImage))
				{
					textGraphics.FillRectangle(Brushes.Black, 0, 0, textImage.Width, textImage.Height);
					textGraphics.DrawImage(img, 0, 0, img.Width, img.Height);

					textGraphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
					Font f = new Font("san-serif", 12F);
					Brush b = Brushes.White;

					textGraphics.DrawString(_text, f, b, 0, img.Height + 5, StringFormat.GenericTypographic);

					img.Dispose();
					textGraphics.Dispose();
				}

				img = textImage;
			}

			// If too big for the screen shrink to fit.
			if( img.Width >= width || img.Height >= height )
			{
				double scale = 1.0;
				if( img.Width > width ) scale = width / (double)img.Width;
				if( img.Height > height && (scale < (height / (double)img.Height)) ) scale = height / (double)img.Height;

				scale = scale * 0.8;

				Image img2 = new Bitmap((int)(img.Width * scale), (int)(img.Height * scale));
				using( Graphics g = Graphics.FromImage(img2) )
				{
					g.DrawImage(img, 0, 0, (int)(img.Width * scale), (int)(img.Height * scale));
					g.Dispose();
				}
				img.Dispose();
				img = img2;
			}

			// Standard post production :)
			DrawerAssist.FinalImage(ref img, width, height);

            DrawImage(img.Width, img.Height, (width - img.Width)/2, (height - img.Height)/2, img);
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

		#endregion

		#region Private Logger Class
		private class Logger
		{

#if DEBUG
			private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(PlainDrawer));
#endif

			public static void Debug(string debugLine)
			{
#if DEBUG
				log.Debug(debugLine);
#endif
			}
		}
		#endregion


	}
}
