using System.Xml;

namespace FlickrNetScreensaver.Filters
{
    public class EveryoneFilter : BaseFilter
    {
        public EveryoneFilterType Filter { get; set; }

        public string Tags { get; set; }

        public bool SortByInterestingness { get; set; }

        public override string ToXml()
        {
            var str = "<PhotoFilter><Type>Everyone</Type><EveryoneFilterType>" + Filter + "</EveryoneFilterType>" +
                         "<Tags>" + Tags + "</Tags><SortByInterestingness>" + SortByInterestingness + "</SortByInterestingness></PhotoFilter>";
            return str;
        }

        public override string ToString()
        {
            var str = "Everyone, Filter: " + Filter + ", Tags:";
            str = str + Tags;
            str = str + ", SortByInterestingness:" + SortByInterestingness;

            return str;
        }

        public EveryoneFilter()
        {
        }

        public EveryoneFilter(string str)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(str);

            var value = "";

            var parentNode = xmlDoc.GetElementsByTagName("EveryoneFilterType");
            foreach (XmlNode childrenNode in parentNode)
            {
                value = childrenNode.InnerText;
            }

            if (value.Equals("Recent"))
            {
                Filter = EveryoneFilterType.Recent;
            }
            else if (value.Equals("Tags"))
            {
                Filter = EveryoneFilterType.Tags;
            }

            parentNode = xmlDoc.GetElementsByTagName("Tags");
            foreach (XmlNode childrenNode in parentNode)
            {
                value = childrenNode.InnerText;
            }

            Tags = value;

            parentNode = xmlDoc.GetElementsByTagName("SortByInterestingness");
            foreach (XmlNode childrenNode in parentNode)
            {
                value = childrenNode.InnerText;
            }

            SortByInterestingness = value.Equals("True");
        }

    }
}