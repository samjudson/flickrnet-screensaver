using System;

namespace FlickrNetScreensaver
{
	/// <summary>
	/// Summary description for DrawerFactory.
	/// </summary>
	public sealed class DrawerFactory
	{
		public static IPictureDrawer GetDrawer(FScreensaver pictureForm)
		{
			if( Settings.Contains("Drawer") )
                return GetDrawer(pictureForm, Settings.Get("Drawer"));
			else
				return GetDrawer(pictureForm, null);
		}

		public static IPictureDrawer GetDrawer(FScreensaver pictureForm, string name)
		{
			IPictureDrawer drawer;


			switch(name)
			{
				case "Moving":
					drawer = new Drawers.MovingDrawer();
					break;
				case "Postcard":
					drawer = new Drawers.PostcardDrawer();
					break;
				case "Plain":
				default:
					drawer = new Drawers.PlainDrawer();
					break;
			}

			drawer.Initialise(pictureForm);

			return drawer;
		}

		public static DrawOptions.AbstractOptions GetOptionsControl(string name)
		{
			switch(name)
			{
				case "Moving":
					return new DrawOptions.MovingOptions();
				case "Postcard":
					return new DrawOptions.PostcardOptions();
				case "Plain":
				default:
					return new DrawOptions.PlainOptions();
			}
		}

		public static string[] GetNames()
		{
			return new string[] { "Plain", "Postcard", "Moving" };
		}
	}
}
