using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Threading.Tasks;
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
	public class FScreensaver : Form
	{

		private static readonly Queue<Photo> PreviousPhotos = new Queue<Photo>();

		public static void AddToPreviousPhotosQueue(Photo photo)
		{
			while( PreviousPhotos.Count >= 5 )
			{
				PreviousPhotos.Dequeue();
			}
			PreviousPhotos.Enqueue(photo);
		}

		private IContainer components;
		private readonly IPictureDrawer _drawer;

		private readonly Flickr _flickr = FlickrFactory.GetInstance();

	    private readonly Random _random = new Random();
		private readonly int _screenNumber;
		private System.Windows.Forms.Timer _timerReloadPhotos;
		private System.Windows.Forms.Timer _timerLoadNextPhoto;
		Point _mouseXy = Point.Empty;
        private BackgroundWorker _loadNextPhotoWorker;
        private PictureBox _networkErrorIcon;
		private PictureBox _flickrImage;

		public FScreensaver(int screenNumber)
		{
			//
			// Required for Windows Form Designer support
			//

			Logger.Debug("FScreensaver Constructor");
			Logger.Debug("ScreenNumber: " + screenNumber);

			InitializeComponent();

			_screenNumber = screenNumber;

			_drawer = DrawerFactory.GetDrawer(this);
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
            this._timerLoadNextPhoto = new System.Windows.Forms.Timer(this.components);
            this._flickrImage = new System.Windows.Forms.PictureBox();
            this._timerReloadPhotos = new System.Windows.Forms.Timer(this.components);
            this._loadNextPhotoWorker = new System.ComponentModel.BackgroundWorker();
            this._networkErrorIcon = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this._flickrImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._networkErrorIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // TimerLoadNextPhoto
            // 
            this._timerLoadNextPhoto.Interval = 60000;
            this._timerLoadNextPhoto.Tick += new System.EventHandler(this.TimerLoadNextPhotoTick);
            // 
            // flickrImage
            // 
            this._flickrImage.Image = ((System.Drawing.Image)(resources.GetObject("flickrImage.Image")));
            this._flickrImage.Location = new System.Drawing.Point(0, 0);
            this._flickrImage.Name = "_flickrImage";
            this._flickrImage.Size = new System.Drawing.Size(90, 30);
            this._flickrImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this._flickrImage.TabIndex = 1;
            this._flickrImage.TabStop = false;
            // 
            // TimerReloadPhotos
            // 
            this._timerReloadPhotos.Tick += new System.EventHandler(this.TimerReloadPhotosTick);
            // 
            // loadNextPhotoWorker
            // 
            this._loadNextPhotoWorker.WorkerSupportsCancellation = true;
            this._loadNextPhotoWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.LoadNextPhotoWorkerDoWork);
            this._loadNextPhotoWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.LoadNextPhotoWorkerRunWorkerCompleted);
            // 
            // NetworkErrorIcon
            // 
            this._networkErrorIcon.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._networkErrorIcon.Image = global::FlickrNetScreensaver.Properties.Resources.NetworkDisconnectedSmall;
            this._networkErrorIcon.Location = new System.Drawing.Point(483, 343);
            this._networkErrorIcon.Margin = new System.Windows.Forms.Padding(0);
            this._networkErrorIcon.Name = "_networkErrorIcon";
            this._networkErrorIcon.Size = new System.Drawing.Size(75, 75);
            this._networkErrorIcon.TabIndex = 2;
            this._networkErrorIcon.TabStop = false;
            this._networkErrorIcon.Visible = false;
            // 
            // FScreensaver
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(567, 427);
            this.Controls.Add(this._networkErrorIcon);
            this.Controls.Add(this._flickrImage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Name = "FScreensaver";
            this.Text = "Flickr Screensaver";
            this.Load += new System.EventHandler(this.FScreensaverLoad);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.FScreensaverMouseMove);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FScreensaverMouseMove);
            this.Closing += new System.ComponentModel.CancelEventHandler(this.FScreensaverClosing);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.FScreensaverKeyPress);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FScreensaverMouseMove);
            ((System.ComponentModel.ISupportInitialize)(this._flickrImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._networkErrorIcon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void FScreensaverLoad(object sender, EventArgs e)
		{
			Logger.Debug("Loading");

			Bounds = Screen.AllScreens[_screenNumber].Bounds;

			// Multiple monitor support
			if( Screen.AllScreens.Length > 1 && _screenNumber == 0 )
			{
				for(var i = 1; i < Screen.AllScreens.Length; i++)
				{
					var f = new FScreensaver(i);
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
			_timerReloadPhotos.Interval = 1000*60*15; // Reload photos collection every 15 minutes
			_timerReloadPhotos.Enabled = true;

			_timerLoadNextPhoto.Enabled = true;
			TimerLoadNextPhotoTick(null, EventArgs.Empty);

            System.Net.NetworkInformation.NetworkChange.NetworkAvailabilityChanged += NetworkChangeNetworkAvailabilityChanged;

		}

        void NetworkChangeNetworkAvailabilityChanged(object sender, System.Net.NetworkInformation.NetworkAvailabilityEventArgs e)
        {
            _networkErrorIcon.Visible = !e.IsAvailable;
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
				_timerLoadNextPhoto.Stop();
				_timerReloadPhotos.Stop();

				// Unknown error occur - exit gracefully
				Visible = false;
				MessageBox.Show("The screensaver has exited as an error occurred (" + ex.Message + ")");
				Application.Exit();
				return;
			}

			 // Fractions of a minutes
			_timerLoadNextPhoto.Interval = (int)Math.Round(1000 * 60 * Settings.Default.DrawerDelayTime, 0);
		}

		private void LoadUser()
		{
            var userName = Settings.Default.ShowUserUsername;
			var userType = Settings.Default.ShowUserType;
			FoundUser u;
            var photos = new List<Photo>();
            const PhotoSearchExtras extras = PhotoSearchExtras.OwnerName | PhotoSearchExtras.AllUrls;

			switch(userType)
			{
				case "Set":
                    var photosetCollection = _flickr.PhotosetsGetPhotos(Settings.Default.ShowUserSetId, extras, 1, 500);
                    photos.AddRange(photosetCollection);
                    if (photosetCollection.Total > 500)
                    {
                        photosetCollection = _flickr.PhotosetsGetPhotos(Settings.Default.ShowUserSetId, extras, 2, 500);
                        photos.AddRange(photosetCollection);
                    }
					break;
				case "Tag":
					u = _flickr.PeopleFindByUserName(userName);
                    var tags = Settings.Default.ShowUserTag;
                    var o = new PhotoSearchOptions
                                {
                                    UserId = u.UserId,
                                    ContentType = ContentTypeSearch.PhotosOnly,
                                    Tags = tags,
                                    TagMode = TagMode.AllTags,
                                    Extras = extras,
                                    PerPage = 500
                                };
			        var tagCollection = _flickr.PhotosSearch(o);
                    photos.AddRange(tagCollection);
                    if (tagCollection.Total > 500)
                    {
                        o.Page = 2;
                        tagCollection = _flickr.PhotosSearch(o);
                        photos.AddRange(tagCollection);
                    }
					break;
				case "Fav":
					u = _flickr.PeopleFindByUserName(userName);
                    var favCollection = _flickr.FavoritesGetPublicList(u.UserId, DateTime.MinValue, DateTime.MinValue, extras, 1, 500);
                    photos.AddRange(favCollection);
                    if (favCollection.Total > 500)
                    {
                        favCollection = _flickr.FavoritesGetPublicList(u.UserId, DateTime.MinValue, DateTime.MinValue, extras, 2, 500);
                        photos.AddRange(favCollection);
                    }
					break;
				case "Contacts":
                    u = _flickr.PeopleFindByUserName(userName);
                    if (Settings.Default.ShowUserContact == "Own")
                    {
                        var o2 = new PhotoSearchOptions
                                     {
                                         UserId = u.UserId,
                                         Contacts = ContactSearch.AllContacts,
                                         ContentType = ContentTypeSearch.PhotosOnly,
                                         PerPage = 500,
                                         Extras = extras
                                     };
                        photos.AddRange(_flickr.PhotosSearch(o2));
                    }
                    else
                    {
                        photos.AddRange(_flickr.PhotosGetContactsPublicPhotos(u.UserId, extras));
                    }
					break;
				case "All":
				default:
					u = _flickr.PeopleFindByUserName(userName);
                    photos.AddRange(_flickr.PeopleGetPublicPhotos(u.UserId, 1, 200, SafetyLevel.None, extras));
					break;
			}
            ImageManager.Initialise(photos);
        }

		private void LoadGroup()
		{
			var groupid = _flickr.UrlsLookupGroup("http://www.flickr.com/groups/" + Settings.Default.ShowGroupName);
            var photos = new List<Photo>();
            const PhotoSearchExtras extras = PhotoSearchExtras.OwnerName | PhotoSearchExtras.AllUrls;
            var photoCollection = _flickr.GroupsPoolsGetPhotos(groupid, null, null, extras, 1, 500);
            photos.AddRange(photoCollection);

            // Get page 2 if there are lots of photos.
            if (photoCollection.Total > 500)
            {
                photoCollection = _flickr.GroupsPoolsGetPhotos(groupid, null, null, extras, 2, 500);
                photos.AddRange(photoCollection);
            }

			ImageManager.Initialise(photos);
		}

		private void LoadEveryone()
		{
            var photos = new List<Photo>();

            if (Settings.Default.ShowEveryoneType == "Recent")
            {
                const PhotoSearchExtras extras = PhotoSearchExtras.OwnerName | PhotoSearchExtras.AllUrls;
                var photoCollection = _flickr.PhotosGetRecent(1, 500, extras);
                photos.AddRange(photoCollection);
                photoCollection = _flickr.PhotosGetRecent(2, 500, extras);
                photos.AddRange(photoCollection);
            }
            else
            {
                if (Settings.Default.ShowEveryoneTagInteresting)
                {
                    var o = new PhotoSearchOptions
                                {
                                    Tags = Settings.Default.ShowEveryoneTag,
                                    TagMode = TagMode.AllTags,
                                    PerPage = 50,
                                    SortOrder =
                                        PhotoSearchSortOrder.InterestingnessDescending
                                };
                    o.Extras |= PhotoSearchExtras.OwnerName | PhotoSearchExtras.AllUrls;
                    for (var i = 0; i < 20; i++)
                    {
                        o.MinUploadDate = DateTime.Today.AddDays(-i);
                        o.MaxUploadDate = DateTime.Today.AddDays(-i).AddHours(23).AddHours(59);
                        photos.AddRange(_flickr.PhotosSearch(o));
                    }
                }
                else
                {
                    var o = new PhotoSearchOptions
                                {
                                    Tags = Settings.Default.ShowEveryoneTag,
                                    TagMode = TagMode.AllTags,
                                    PerPage = 500
                                };
                    o.Extras |= PhotoSearchExtras.OwnerName | PhotoSearchExtras.AllUrls;
                    photos.AddRange(_flickr.PhotosSearch(o));
                }

                ImageManager.Initialise(photos);

            }
		}

	    public bool SuspendMouseMove { get; set; }

	    public void FScreensaverMouseMove(object sender, MouseEventArgs e)
		{
            CheckMouseMovement(sender, e.X, e.Y);
        }

        [System.Diagnostics.Conditional("RELEASE")]
        private void CheckMouseMovement(object sender, int x, int y)
        {
			if( sender != this )
			{
				var p2 = ((Control)sender).PointToScreen(new Point(x, y));
				var p = PointToClient(p2);
				x = p.X;
				y = p.Y;
			}

			if (!SuspendMouseMove && !_mouseXy.IsEmpty)
			{
				if (_mouseXy != new Point(x, y))
				{
                    Application.Exit();
				}
			}
			_mouseXy = new Point(x, y);
		}

		private void TimerLoadNextPhotoTick(object sender, EventArgs e)
		{
			Logger.Debug("TimerLoadNextPhoto_Tick");

			StartDoNextPhoto();
		}

		private void StartDoNextPhoto()
		{
			Logger.Debug("Starting next photo - Enter");

            if (_loadNextPhotoWorker.IsBusy) return;

			_timerLoadNextPhoto.Stop();

            _loadNextPhotoWorker.RunWorkerAsync();

			Logger.Debug("Starting next photo - Exit");
		}

		private void MoveFlickrImage()
		{
            Logger.Debug("MoveFlickrImage");

			if( _flickrImage.Top == 0 && _flickrImage.Left == 0 )
			{
				_flickrImage.Left = Width - _flickrImage.Width;
			}
			else if( _flickrImage.Top == 0 && _flickrImage.Left != 0 )
			{
				_flickrImage.Top = Height - _flickrImage.Height;
			}
			else if( _flickrImage.Top != 0 && _flickrImage.Left != 0 )
			{
				_flickrImage.Left = 0;
			}
			else if( _flickrImage.Top != 0 && _flickrImage.Left == 0 )
			{
				_flickrImage.Top = 0;
			}

			Refresh();
		}

        private void DoLoadNextPhoto()
        {
            Logger.Debug("DoLoadNextPhoto - Enter");

            var p = ImageManager.NextPhoto;
            var url = ImageManager.CalcUrl(p);

            AddToPreviousPhotosQueue(p);

            Image img;
            using (var client = new WebClient())
            {
                try
                {
                    img = Image.FromStream(client.OpenRead(url));
                }
                catch (FlickrException ex)
                {
                    Visible = false;
                    MessageBox.Show("The screensaver has exited as an error occurred (" + ex.Message + ")");
                    Application.Exit();
                    return;
                }
            }

            _drawer.ChangeImage(img, p);
        }

		private void TimerReloadPhotosTick(object sender, EventArgs e)
		{
			//LoadSettings();
		}

		private void FScreensaverKeyPress(object sender, KeyPressEventArgs e)
		{
            if (e.KeyChar == ' ')
            {
                e.Handled = true;
                StartDoNextPhoto();
                return;
            }
#if DEBUG
		    if (e.KeyChar == 'q')
		    {
		        Close();
		    }
#else
            Close();
#endif
		}

		private void FScreensaverClosing(object sender, CancelEventArgs e)
		{
			Logger.Debug("FScreensaver_Closing");

			try
			{
                if (_loadNextPhotoWorker.IsBusy) _loadNextPhotoWorker.CancelAsync();
			}
			catch
			{
			}

            var photoIds = new List<string>();
			while(PreviousPhotos.Count > 0 )
			{
                Photo photo = PreviousPhotos.Dequeue();
                photoIds.Add(photo.PhotoId);
			}
            Settings.Default.RecentPhotos = String.Join("|",photoIds.ToArray());
            Settings.Default.Save();
		}

		private static class Logger
		{
			private static readonly ILog Log = LogManager.GetLogger(typeof(FScreensaver));

            [System.Diagnostics.Conditional("DEBUG")]
			public static void Debug(string debugLine)
			{
				Log.Debug(debugLine);
			}
		}

        private void LoadNextPhotoWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            DoLoadNextPhoto();
        }

        private void LoadNextPhotoWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (_random.Next(0, 2) == 0) MoveFlickrImage();

            _timerLoadNextPhoto.Start();
        }

	}
}
