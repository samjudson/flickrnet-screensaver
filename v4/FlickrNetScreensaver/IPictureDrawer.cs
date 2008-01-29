using System;
using System.Drawing;
using System.Windows.Forms;

namespace FlickrNetScreensaver
{
	/// <summary>
	/// Summary description for IPictureDrawer.
	/// </summary>
	public interface IPictureDrawer
	{
		void Initialise(FScreensaver pictureForm);
		void ChangeImage(Image img, FlickrNet.Photo photo);
	}
}
