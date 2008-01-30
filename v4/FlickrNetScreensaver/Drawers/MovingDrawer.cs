using System;
using System.Drawing;
using System.Windows.Forms;

namespace FlickrNetScreensaver.Drawers
{
	/// <summary>
	/// Summary description for MovingDrawer.
	/// </summary>
	public class MovingDrawer : IPictureDrawer
	{
		private FScreensaver _pictureForm;
		private PictureBox _pictureBox;
		private Timer _timer;

		public MovingDrawer()
		{
		}

		public void Initialise(FScreensaver pictureForm)
		{
			Logger.Debug("MoveDrawer Initialise");

			_pictureForm = pictureForm;

			_pictureBox = new PictureBox();
			_pictureForm.Controls.Add(_pictureBox);
			_pictureBox.SendToBack();

			_pictureBox.MouseMove += new MouseEventHandler(_pictureForm.FScreensaver_MouseMove);

			_timer = new Timer();
			_timer.Tick += new EventHandler(_timer_Tick);
			_timer.Interval = 100;
			_timer.Start();

		}

		private Point _originalLocation;
		private Size _boxSize;
		private Point _destination = new Point(0, 0);
		private Point _source;
		private int iteration = 0;
		private Random _rand = new Random();

		private void _timer_Tick(object sender, EventArgs e)
		{
			if( _pictureBox.Image == null ) return;

			if( iteration % 100 == 0 )
			{
				iteration = 0;

				Logger.Debug("Tick - Iteration loop");

				if( _source == new Point(0, 0) )
				{
					PickStartingSource();
				}
				else
				{
					_source = _destination;
				}

				PickDestination();
			
			}

			Point delta = new Point(_source.X + (_destination.X - _source.X)*iteration/100, _source.Y + (_destination.Y - _source.Y)*iteration/100);

			_pictureForm.SuspendMouseMove = true;

			_pictureBox.Top = delta.Y;
			_pictureBox.Left = delta.X;

			_pictureForm.SuspendMouseMove = false;

			iteration++;
		}

		private void PickStartingSource()
		{
			int length = _boxSize.Width*2 + _boxSize.Height*2;
			int distance = _rand.Next(Math.Abs(length));

			if( distance < Math.Abs(_boxSize.Width) )
			{
				// bottom row
				_source = new Point(-distance, 0);
			}
			else if( distance < Math.Abs(_boxSize.Width + _boxSize.Height))
			{
				// left line - along bottom and up
				_source = new Point(-Math.Abs(_boxSize.Width), -(distance - Math.Abs(_boxSize.Width)));
			}
			else if( distance < Math.Abs(_boxSize.Width * 2 + _boxSize.Height) )
			{
				// top row - along bottom, up left and back across
				_source = new Point(-(distance - Math.Abs(_boxSize.Width + _boxSize.Height)), -Math.Abs(_boxSize.Height));
			}
			else
			{
				// right hand side
				_source = new Point(0,-(distance - Math.Abs(_boxSize.Width * 2 + _boxSize.Height)));
			}

			if( _source.X > 0 || _source.Y > 0 ) System.Diagnostics.Debug.Fail("");

			Logger.Debug("PickStartingSource = " + _source.ToString());
		}

		private void PickDestination()
		{
			if( _source.X == 0 ) 
			{
				_destination.X = -_boxSize.Width;
				_destination.Y = -_rand.Next(_boxSize.Height);
			}
			else if ( _source.Y == 0 )
			{
				_destination.Y = -_boxSize.Height;
				_destination.X = -_rand.Next(_boxSize.Width);
			}
			else if ( _source.X == -_boxSize.Height )
			{
				_destination.X = 0;
				_destination.Y = -_rand.Next(_boxSize.Height);
			}
			else
			{
				_destination.Y = 0;
				_destination.X = -_rand.Next(_boxSize.Width);
			}

			if( _destination.X > 0 || _destination.Y > 0 ) System.Diagnostics.Debug.Fail("");

			Logger.Debug("PickDestination = " + _destination.ToString());
		}


		public void ChangeImage(System.Drawing.Image img, FlickrNet.Photo photo)
		{
			Logger.Debug("MoveDrawer ChangeImage");

			int width = _pictureForm.Width;
			int height = _pictureForm.Height;


			// If too small for screen scale to overflow
			int newWidth = img.Width, newHeight = img.Height;

			if( img.Width > width && img.Height > height )
			{
				// Do nothing
			}
			else
			{
				double scale1 = width / (double)img.Width * 1.1;
				double scale2 = height / (double)img.Height * 1.1;

				if( scale1 <= 1 )
				{
					newWidth = (int)(newWidth * scale2);
					newHeight = (int)(newHeight * scale2);
				}
				else if (scale2 <= 1 )
				{
					newWidth = (int)(newWidth * scale1);
					newHeight = (int)(newHeight * scale1);
				}
				else
				{
					newWidth = (int)(newWidth * Math.Max(scale1, scale2));
					newHeight = (int)(newHeight * Math.Max(scale1, scale2));
				}
			}

			if( img.Width != newWidth || img.Height != newHeight )
			{
				Image img2 = new Bitmap(newWidth,newHeight);
				Graphics g = Graphics.FromImage(img2);
				g.DrawImage(img, 0, 0, newWidth, newHeight);
				g.Dispose();
				img.Dispose();
				img = img2;
			}

			_pictureForm.SuspendMouseMove = true;

			_pictureBox.Width = img.Width;
			_pictureBox.Height = img.Height;
			_pictureBox.Left = (width - img.Width)/2;
			_pictureBox.Top = (height - img.Height)/2;
			_pictureBox.Image = img;

			_originalLocation = _pictureBox.Location;
			_boxSize = new Size(Math.Abs(_pictureBox.Left), Math.Abs(_pictureBox.Top));

			_source = new Point(0, 0);
			iteration = 0;

			_pictureForm.SuspendMouseMove = false;

			Logger.Debug("Form Size = " + _pictureForm.Size);
			Logger.Debug("Image Size = " + img.Size);
			Logger.Debug("Image Location = " + _pictureBox.Location);
		}

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
