using System;
using FlickrNetScreensaver.Properties;

namespace FlickrNetScreensaver
{
	/// <summary>
	/// Summary description for DrawerFactory.
	/// </summary>
	public sealed class DrawerFactory
	{
		public static IPictureDrawer GetDrawer(FScreensaver pictureForm)
		{
            return GetDrawer(pictureForm, Settings.Default.Drawer);
		}

		private static IPictureDrawer GetDrawer(FScreensaver pictureForm, string name)
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
                case "Stack":
                    drawer = new Drawers.StackDrawer();
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
                case "Stack":
                    return new DrawOptions.StackOptions();
				case "Plain":
				default:
					return new DrawOptions.PlainOptions();
			}
		}

		public static string[] GetNames()
		{
			return new string[] { "Plain", "Postcard", "Moving", "Stack" };
		}
	}
}
