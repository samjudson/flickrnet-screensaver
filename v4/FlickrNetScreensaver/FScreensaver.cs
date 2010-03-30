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
using System.Collections.Generic;
using System.Net;

namespace FlickrNetScreensaver
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class FScreensaver : System.Windows.Forms.Form
	{

		private static Queue<Photo> previousPhotos = new Queue<Photo>();

		public static void AddToPreviousPhotosQueue(Photo photo)
		{
			while( previousPhotos.Count >= 5 )
			{
				previousPhotos.Dequeue();
			}
			previousPhotos.Enqueue(photo);
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
        private PictureBox NetworkErrorIcon;
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
            this.NetworkErrorIcon = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.flickrImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NetworkErrorIcon)).BeginInit();
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
            // NetworkErrorIcon
            // 
            this.NetworkErrorIcon.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.NetworkErrorIcon.Image = global::FlickrNetScreensaver.Properties.Resources.NetworkDisconnectedSmall;
            this.NetworkErrorIcon.Location = new System.Drawing.Point(483, 343);
            this.NetworkErrorIcon.Margin = new System.Windows.Forms.Padding(0);
            this.NetworkErrorIcon.Name = "NetworkErrorIcon";
            this.NetworkErrorIcon.Size = new System.Drawing.Size(75, 75);
            this.NetworkErrorIcon.TabIndex = 2;
            this.NetworkErrorIcon.TabStop = false;
            this.NetworkErrorIcon.Visible = false;
            // 
            // FScreensaver
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(567, 427);
            this.Controls.Add(this.NetworkErrorIcon);
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
            ((System.ComponentModel.ISupportInitialize)(this.NetworkErrorIcon)).EndInit();
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

            System.Net.NetworkInformation.NetworkChange.NetworkAvailabilityChanged += new System.Net.NetworkInformation.NetworkAvailabilityChangedEventHandler(NetworkChange_NetworkAvailabilityChanged);

		}

        void NetworkChange_NetworkAvailabilityChanged(object sender, System.Net.NetworkInformation.NetworkAvailabilityEventArgs e)
        {
            NetworkErrorIcon.Visible = !e.IsAvailable;
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
			string userType = Settings.Default.ShowUserType;
			FoundUser u = null;
            List<Photo> photos = new List<Photo>();
            PhotoSearchExtras extras = PhotoSearchExtras.OwnerName | PhotoSearchExtras.AllUrls;

			switch(userType)
			{
				case "Set":
                    var photosetCollection = flickr.PhotosetsGetPhotos(Settings.Default.ShowUserSetId, extras, 1, 500);
                    photos.AddRange(photosetCollection);
                    if (photosetCollection.Total > 500)
                    {
                        photosetCollection = flickr.PhotosetsGetPhotos(Settings.Default.ShowUserSetId, extras, 2, 500);
                        photos.AddRange(photosetCollection);
                    }
					break;
				case "Tag":
					u = flickr.PeopleFindByUserName(userName);
                    string tags = Settings.Default.ShowUserTag;
                    PhotoSearchOptions o = new PhotoSearchOptions();
                    o.UserId = u.UserId;
                    o.ContentType = ContentTypeSearch.PhotosOnly;
                    o.Tags = tags;
                    o.TagMode = TagMode.AllTags;
                    o.Extras = extras;
                    o.PerPage = 500;
                    var tagCollection = flickr.PhotosSearch(o);
                    photos.AddRange(tagCollection);
                    if (tagCollection.Total > 500)
                    {
                        o.Page = 2;
                        tagCollection = flickr.PhotosSearch(o);
                        photos.AddRange(tagCollection);
                    }
					break;
				case "Fav":
					u = flickr.PeopleFindByUserName(userName);
                    var favCollection = flickr.FavoritesGetPublicList(u.UserId, DateTime.MinValue, DateTime.MinValue, extras, 1, 500);
                    photos.AddRange(favCollection);
                    if (favCollection.Total > 500)
                    {
                        favCollection = flickr.FavoritesGetPublicList(u.UserId, DateTime.MinValue, DateTime.MinValue, extras, 2, 500);
                        photos.AddRange(favCollection);
                    }
					break;
				case "Contacts":
                    u = flickr.PeopleFindByUserName(userName);
                    if (Settings.Default.ShowUserContact == "Own")
                    {
                        PhotoSearchOptions o2 = new PhotoSearchOptions();
                        o2.UserId = u.UserId;
                        o2.Contacts = ContactSearch.AllContacts;
                        o2.ContentType = ContentTypeSearch.PhotosOnly;
                        o2.PerPage = 500;
                        o2.Extras = extras;
                        photos.AddRange(flickr.PhotosSearch(o2));
                    }
                    else
                    {
                        photos.AddRange(flickr.PhotosGetContactsPublicPhotos(u.UserId, extras));
                    }
					break;
				case "All":
				default:
					u = flickr.PeopleFindByUserName(userName);
                    photos.AddRange(flickr.PeopleGetPublicPhotos(u.UserId, 1, 200, SafetyLevel.None, extras));
					break;
			}
            ImageManager.Initialise(photos);
        }

		private void LoadGroup()
		{
			string groupid = flickr.UrlsLookupGroup("http://www.flickr.com/groups/" + Settings.Default.ShowGroupName);
            List<Photo> photos = new List<Photo>();
            PhotoSearchExtras extras = PhotoSearchExtras.OwnerName | PhotoSearchExtras.AllUrls;
            PhotoCollection photoCollection = flickr.GroupsPoolsGetPhotos(groupid, null, null, extras, 1, 500);
            photos.AddRange(photoCollection);

            // Get page 2 if there are lots of photos.
            if (photoCollection.Total > 500)
            {
                photoCollection = flickr.GroupsPoolsGetPhotos(groupid, null, null, extras, 2, 500);
                photos.AddRange(photoCollection);
            }

			ImageManager.Initialise(photos);
		}

		private void LoadEveryone()
		{
            List<Photo> photos = new List<Photo>();

            if (Settings.Default.ShowEveryoneType == "Recent")
            {
                PhotoSearchExtras extras = PhotoSearchExtras.OwnerName | PhotoSearchExtras.AllUrls;
                var photoCollection = flickr.PhotosGetRecent(1, 500, extras);
                photos.AddRange(photoCollection);
                photoCollection = flickr.PhotosGetRecent(2, 500, extras);
                photos.AddRange(photoCollection);
            }
            else
            {
                if (Settings.Default.ShowEveryoneTagInteresting == true)
                {
                    PhotoSearchOptions o = new PhotoSearchOptions();
                    o.Tags = Settings.Default.ShowEveryoneTag;
                    o.TagMode = TagMode.AllTags;
                    o.PerPage = 50;
                    o.SortOrder = PhotoSearchSortOrder.InterestingnessDescending;
                    o.Extras |= PhotoSearchExtras.OwnerName | PhotoSearchExtras.AllUrls;
                    for (int i = 0; i < 20; i++)
                    {
                        o.MinUploadDate = DateTime.Today.AddDays(-i);
                        o.MaxUploadDate = DateTime.Today.AddDays(-i).AddHours(23).AddHours(59);
                        photos.AddRange(flickr.PhotosSearch(o));
                    }
                }
                else
                {
                    PhotoSearchOptions o = new PhotoSearchOptions();
                    o.Tags = Settings.Default.ShowEveryoneTag;
                    o.TagMode = TagMode.AllTags;
                    o.PerPage = 500;
                    o.Extras |= PhotoSearchExtras.OwnerName | PhotoSearchExtras.AllUrls;
                    photos.AddRange(flickr.PhotosSearch(o));
                }

                ImageManager.Initialise(photos);

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
            CheckMouseMovement(sender, e.X, e.Y);
        }

        [System.Diagnostics.Conditional("RELEASE")]
        private void CheckMouseMovement(object sender, int x, int y)
        {
			if( sender != this )
			{
				Point p2 = ((Control)sender).PointToScreen(new Point(x, y));
				Point p = this.PointToClient(p2);
				x = p.X;
				y = p.Y;
			}

			if (!SuspendMouseMove && !MouseXY.IsEmpty)
			{
				if (MouseXY != new Point(x, y))
				{
                    Application.Exit();
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
            Uri url = ImageManager.CalcUrl(p);

            AddToPreviousPhotosQueue(p);

            Image img = null;
            using (WebClient client = new WebClient())
            {
                try
                {
                    img = Image.FromStream(client.OpenRead(url));
                }
                catch (FlickrException ex)
                {
                    this.Visible = false;
                    MessageBox.Show("The screensaver has exited as an error occurred (" + ex.Message + ")");
                    Application.Exit();
                    return;
                }
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
                {
                    Close();
                }
#else
                Close();
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

            List<string> photoIds = new List<string>();
			while(previousPhotos.Count > 0 )
			{
                Photo photo = previousPhotos.Dequeue();
                photoIds.Add(photo.PhotoId);
			}
            Settings.Default.RecentPhotos = String.Join("|",photoIds.ToArray());
            Settings.Default.Save();
		}

		private class Logger
		{
			private static readonly ILog log = LogManager.GetLogger(typeof(FScreensaver));

            [System.Diagnostics.Conditional("DEBUG")]
			public static void Debug(string debugLine)
			{
				log.Debug(debugLine);
			}
		}

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
