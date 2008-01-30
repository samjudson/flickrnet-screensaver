using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Threading;

using FlickrNet;
using log4net;
using FlickrNetScreensaver.Properties;

namespace FlickrNetScreensaver
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class FScreensaver : System.Windows.Forms.Form
	{

		private static Queue previousPhotos = new Queue();

		public static void AddToQueue(string photoId)
		{
			while( previousPhotos.Count >= 5 )
			{
				previousPhotos.Dequeue();
			}
			previousPhotos.Enqueue(photoId);
		}

		private System.ComponentModel.IContainer components;
		private IPictureDrawer drawer;

		private Flickr flickr = FlickrFactory.GetInstance();

		private bool fillScreen = false;
		private Random random = new Random();
		private int ScreenNumber = 0;
		private System.Windows.Forms.Timer TimerReloadPhotos;
		private System.Windows.Forms.Timer TimerLoadNextPhoto;
		Point MouseXY = Point.Empty;
        private BackgroundWorker loadNextPhotoWorker;
		private System.Windows.Forms.PictureBox flickrImage;

		public FScreensaver(int screenNumber)
		{
			//
			// Required for Windows Form Designer support
			//

			Logger.Debug("FScreensaver Constructor");
			Logger.Debug("ScreenNumber: " + screenNumber);

			InitializeComponent();

			ScreenNumber = screenNumber;

			drawer = DrawerFactory.GetDrawer(this);
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FScreensaver));
            this.TimerLoadNextPhoto = new System.Windows.Forms.Timer(this.components);
            this.flickrImage = new System.Windows.Forms.PictureBox();
            this.TimerReloadPhotos = new System.Windows.Forms.Timer(this.components);
            this.loadNextPhotoWorker = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)(this.flickrImage)).BeginInit();
            this.SuspendLayout();
            // 
            // TimerLoadNextPhoto
            // 
            this.TimerLoadNextPhoto.Interval = 60000;
            this.TimerLoadNextPhoto.Tick += new System.EventHandler(this.TimerLoadNextPhoto_Tick);
            // 
            // flickrImage
            // 
            this.flickrImage.Image = ((System.Drawing.Image)(resources.GetObject("flickrImage.Image")));
            this.flickrImage.Location = new System.Drawing.Point(0, 0);
            this.flickrImage.Name = "flickrImage";
            this.flickrImage.Size = new System.Drawing.Size(90, 30);
            this.flickrImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.flickrImage.TabIndex = 1;
            this.flickrImage.TabStop = false;
            // 
            // TimerReloadPhotos
            // 
            this.TimerReloadPhotos.Tick += new System.EventHandler(this.TimerReloadPhotos_Tick);
            // 
            // loadNextPhotoWorker
            // 
            this.loadNextPhotoWorker.WorkerSupportsCancellation = true;
            this.loadNextPhotoWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.loadNextPhotoWorker_DoWork);
            this.loadNextPhotoWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.loadNextPhotoWorker_RunWorkerCompleted);
            // 
            // FScreensaver
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.Controls.Add(this.flickrImage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Name = "FScreensaver";
            this.Text = "Flickr Screensaver";
            this.Load += new System.EventHandler(this.FScreensaver_Load);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.FScreensaver_MouseMove);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FScreensaver_MouseMove);
            this.Closing += new System.ComponentModel.CancelEventHandler(this.FScreensaver_Closing);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.FScreensaver_KeyPress);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FScreensaver_MouseMove);
            ((System.ComponentModel.ISupportInitialize)(this.flickrImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void FScreensaver_Load(object sender, System.EventArgs e)
		{
			Logger.Debug("Loading");

			Bounds = Screen.AllScreens[ScreenNumber].Bounds;

			// Multiple monitor support
			if( Screen.AllScreens.Length > 1 && ScreenNumber == 0 )
			{
				for(int i = 1; i < Screen.AllScreens.Length; i++)
				{
					FScreensaver f = new FScreensaver(i);
					f.Show();
				}
			}

			Cursor.Hide();

#if !DEBUG
			TopMost = true;
#endif

			Refresh();
			Application.DoEvents();

			LoadSettings();

			// Start timer off
			TimerReloadPhotos.Interval = 1000*60*15; // Reload photos collection every 15 minutes
			TimerReloadPhotos.Enabled = true;

			TimerLoadNextPhoto.Enabled = true;
			TimerLoadNextPhoto_Tick(null, EventArgs.Empty);

		}

		private void LoadSettings()
		{
			try
			{
                switch (Settings.Default.ShowType)
				{
					case "Everyone":
						LoadEveryone();
						break;
					case "Group":
						LoadGroup();
						break;
					case "User":
					default:
						LoadUser();
						break;
				}
			}
			catch(FlickrException ex)
			{
				TimerLoadNextPhoto.Stop();
				TimerReloadPhotos.Stop();

				// Unknown error occur - exit gracefully
				this.Visible = false;
				MessageBox.Show("The screensaver has exited as an error occurred (" + ex.Message + ")");
				Application.Exit();
				return;
			}

			 // Fractions of a minutes
			TimerLoadNextPhoto.Interval = (int)Math.Round(1000 * 60 * Settings.Default.DrawerDelayTime, 0);

            fillScreen = Settings.Default.DrawerFillScreen;
		}

		private void LoadUser()
		{
            string userName = Settings.Default.ShowUserUsername;
			string userType = Settings.Default.ShowType;
			FoundUser u = null;
            Photo[] photos = null;

			switch(userType)
			{
				case "Set":
                    photos = flickr.PhotosetsGetPhotos(Settings.Default.ShowUserSetId);
					break;
				case "Tag":
					u = flickr.PeopleFindByUsername(userName);
                    string tags = Settings.Default.ShowUserTag;
                    photos = flickr.PhotosSearch(u.UserId, tags, TagMode.AllTags, "", PhotoSearchExtras.OwnerName).PhotoCollection;
					break;
				case "Fav":
					u = flickr.PeopleFindByUsername(userName);
                    photos = flickr.FavoritesGetPublicList(u.UserId, 200, 1).PhotoCollection;
					break;
				case "Contacts":
                    if (Settings.Default.ShowUserContact == "Own")
                    {
                        photos = flickr.PhotosGetContactsPhotos(200, false, false, false).PhotoCollection;
                    }
                    else
                    {
                        u = flickr.PeopleFindByUsername(userName);
                        photos = flickr.PhotosGetContactsPublicPhotos(u.UserId, 200, false, false, false).PhotoCollection;
                    }
					break;
				case "All":
				default:
					u = flickr.PeopleFindByUsername(userName);
					photos = flickr.PhotosSearch(u.UserId, "", TagMode.AllTags, "", 200, 1).PhotoCollection;
					break;
			}
            ImageManager.Initialise(photos);
        }

		private void LoadGroup()
		{
			string groupid = flickr.UrlsLookupGroup("http://www.flickr.com/groups/" + Settings.Default.ShowGroupName);
			ImageManager.Initialise(flickr.GroupPoolGetPhotos(groupid, 200, 1).PhotoCollection);
		}

		private void LoadEveryone()
		{
			switch(Settings.Default.ShowEveryoneType)
			{
				case "Recent":
					ImageManager.Initialise(flickr.PhotosGetRecent().PhotoCollection);
					break;
				case "Tag":
				default:
					ImageManager.Initialise(flickr.PhotosSearch(Settings.Default.ShowEveryoneTag, TagMode.AllTags, null).PhotoCollection);
					break;
			}
		}

		private bool _suspendMouseMove;

		public bool SuspendMouseMove
		{
			get { return _suspendMouseMove; }
			set { _suspendMouseMove = value; }
		}

		public void FScreensaver_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
#if DEBUG
            return;
#endif
			int x = e.X;
			int y = e.Y;
			if( sender != this )
			{
				Point p2 = ((Control)sender).PointToScreen(new Point(x, y));
				Point p = this.PointToClient(p2);
				x = p.X;
				y = p.Y;
			}
			//Logger.Debug("Mouse Move called by " + sender.ToString());
			//Logger.Debug("Mouse Moved to " + x + " " + y);
			//Logger.Debug("Suspend Mouse Move = " + SuspendMouseMove);

			if (!SuspendMouseMove && !MouseXY.IsEmpty)
			{
				if (MouseXY != new Point(x, y))
				{
					Close();
				}
				if (e.Clicks > 0)
				{
					Close();
				}
			}
			MouseXY = new Point(x, y);
		}

		private void TimerLoadNextPhoto_Tick(object sender, System.EventArgs e)
		{
			Logger.Debug("TimerLoadNextPhoto_Tick");

			StartDoNextPhoto();
		}

		private void StartDoNextPhoto()
		{
			Logger.Debug("Starting next photo - Enter");

            if (loadNextPhotoWorker.IsBusy) return;

			TimerLoadNextPhoto.Stop();

            loadNextPhotoWorker.RunWorkerAsync();

			Logger.Debug("Starting next photo - Exit");
		}

		private void MoveFlickrImage()
		{
            Logger.Debug("MoveFlickrImage");

			if( flickrImage.Top == 0 && flickrImage.Left == 0 )
			{
				flickrImage.Left = Width - flickrImage.Width;
			}
			else if( flickrImage.Top == 0 && flickrImage.Left != 0 )
			{
				flickrImage.Top = Height - flickrImage.Height;
			}
			else if( flickrImage.Top != 0 && flickrImage.Left != 0 )
			{
				flickrImage.Left = 0;
			}
			else if( flickrImage.Top != 0 && flickrImage.Left == 0 )
			{
				flickrImage.Top = 0;
			}

			Refresh();
		}

        private void DoLoadNextPhoto()
        {
            Logger.Debug("DoLoadNextPhoto - Enter");

            Photo p = ImageManager.NextPhoto;
            string url = ImageManager.NextPhotoUrl;
            ImageManager.PopPhoto();

            AddToQueue(p.PhotoId);

            Image img = null;

            try
            {
                img = Image.FromStream(flickr.DownloadPicture(url));
            }
            catch (FlickrException ex)
            {
                this.Visible = false;
                MessageBox.Show("The screensaver has exited as an error occurred (" + ex.Message + ")");
                Application.Exit();
                return;
            }

            drawer.ChangeImage(img, p);

            return;
        }

		private void TimerReloadPhotos_Tick(object sender, System.EventArgs e)
		{
			//LoadSettings();
		}

		private void FScreensaver_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
            if (e.KeyChar == ' ')
            {
                e.Handled = true;
                StartDoNextPhoto();
            }
            else
            {
#if DEBUG
                if (e.KeyChar == 'q')
                    Application.Exit();
#else
                Application.Exit();
#endif
            }
		}

		private void FScreensaver_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			Logger.Debug("FScreensaver_Closing");

			try
			{
                if (loadNextPhotoWorker.IsBusy) loadNextPhotoWorker.CancelAsync();
			}
			catch
			{
			}

			string photoIds = "";
			while(previousPhotos.Count > 0 )
			{
				photoIds += (string)previousPhotos.Dequeue() + "|";
			}
			if( photoIds.Length > 0 ) photoIds = photoIds.Substring(0, photoIds.Length-1);
            Settings.Default.RecentPhotos = photoIds;
            Settings.Default.Save();
		}

		#region Private Logger Class
		private class Logger
		{

#if DEBUG
			private static readonly ILog log = LogManager.GetLogger(typeof(FScreensaver));
#endif

			public static void Debug(string debugLine)
			{
#if DEBUG
				log.Debug(debugLine);
#endif
			}
		}
		#endregion

        private void loadNextPhotoWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            DoLoadNextPhoto();
        }

        private void loadNextPhotoWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (random.Next(0, 2) == 0) MoveFlickrImage();

            TimerLoadNextPhoto.Start();
        }

	}
}
