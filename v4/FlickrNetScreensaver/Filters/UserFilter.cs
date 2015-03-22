using System.Collections.Generic;
using System.Xml;

namespace FlickrNetScreensaver.Filters
{
    public class UserFilter : BaseFilter
    {
        public string Username { get; set; }

        public UserFilterType FilterType { get; set; }

        public string FilterDetails { get; set; }

        public string SetId { get; set; }

        public override string ToString()
        {
            var l = new List<string>(new[] {"User: " + Username, "Type: " + FilterType});
            if (!string.IsNullOrEmpty(FilterDetails))
            {
                l.Add("Tags: " + FilterDetails);
            }
            if (!string.IsNullOrEmpty(SetId))
            {
                l.Add("Set: " + SetId);
            }
            return string.Join(", ", l);
        }

        public override string ToXml()
        {
            var str = "<PhotoFilter> <Type>User</Type> " +
                      "<Name>" + Username + "</Name>" +
                      "<UserFilterType>" + FilterType + "</UserFilterType> " +
                      "<FilterDetails>" + FilterDetails + "</FilterDetails>" +
                      "<SetId>" + SetId + "</SetId> </PhotoFilter>";

            return str;
        }

        public UserFilter()
        {
        }

        public UserFilter(string str)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(str);

            var value = "";

            var parentNode = xmlDoc.GetElementsByTagName("Name");
            foreach (XmlNode childrenNode in parentNode)
            {
                value = childrenNode.InnerText;
            }

            Username = value;

            parentNode = xmlDoc.GetElementsByTagName("UserFilterType");
            foreach (XmlNode childrenNode in parentNode)
            {
                value = childrenNode.InnerText;
            }

            if (value.Equals("All"))
            {
                FilterType = UserFilterType.All;
            }
            else if (value.Equals("Favorite"))
            {
                FilterType = UserFilterType.Favorite;
            }
            else if (value.Equals("Contacts"))
            {
                FilterType = UserFilterType.Contacts;
            }
            else if (value.Equals("Set"))
            {
                FilterType = UserFilterType.Set;
            }
            else if (value.Equals("Tags"))
            {
                FilterType = UserFilterType.Tags;
            }

            parentNode = xmlDoc.GetElementsByTagName("FilterDetails");
            foreach (XmlNode childrenNode in parentNode)
            {
                value = childrenNode.InnerText;
            }

            FilterDetails = value;
        }
    }
}