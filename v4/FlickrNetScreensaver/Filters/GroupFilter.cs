using System.Xml;

namespace FlickrNetScreensaver.Filters
{
    public class GroupFilter : BaseFilter
    {
        public string GroupName { get; set; }

        public override string ToXml()
        {
            return "<PhotoFilter><Type>Group</Type><Name>" + GroupName + "</Name></PhotoFilter>";
        }

        public override string ToString()
        {
            return "Group:" + GroupName;
        }

        public GroupFilter()
        {
        }

        public GroupFilter(string str)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(str);

            var value = "";

            var parentNode = xmlDoc.GetElementsByTagName("Name");

            foreach (XmlNode childrenNode in parentNode)
            {
                value = childrenNode.InnerText;
            }

            GroupName = value;
        }
    }
}