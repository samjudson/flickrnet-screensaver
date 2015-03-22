using System;
using System.Xml;

namespace FlickrNetScreensaver.Filters
{
    public abstract class BaseFilter
    {
        public abstract string ToXml();

    }
    public class PhotoFilter
    {
        public FilterGroupType FilterGroupType;

        public BaseFilter Filter { get; set; }

        public EveryoneFilter EveryoneFilter { get { return Filter as EveryoneFilter; } }
        public GroupFilter GroupFilter { get { return Filter as GroupFilter; } }
        public UserFilter UserFilter { get { return Filter as UserFilter; } }

        public override string ToString()
        {
            return Filter.ToString();
        }

        public string ToXml()
        {
            return Filter.ToXml();
        }

        public PhotoFilter()
        {
        }

        public PhotoFilter(string xmlText)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlText);

            var type = "";

            var parentNode = xmlDoc.GetElementsByTagName("Type");
            foreach (XmlNode childrenNode in parentNode)
            {
                type = childrenNode.InnerText;
            }

            switch (type)
            {
                case "User":
                    FilterGroupType = FilterGroupType.User;
                    Filter = new UserFilter(xmlText);
                    break;
                case "Everyone":
                    FilterGroupType = FilterGroupType.Everyone;
                    Filter = new EveryoneFilter(xmlText);
                    break;
                case "Group":
                    FilterGroupType = FilterGroupType.Group;
                    Filter = new GroupFilter(xmlText);
                    break;


            }

            throw new Exception("Unrecognized Filter string of type '" + type + "'!");
        }
    }
}
