using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace FlickrNetScreensaver
{
    public enum FilterGroupType
    {
        Everyone,
        User,
        Group
    };

    public enum UserFilterType
    {
        All,
        Favorite,
        Contacts,
        Set,
        Tags
    }

    public class PhotoFilter
    {
        public FilterGroupType FilterGroupType;

        public UserFilter UserFilter { get; set; }

        public EveryoneFilter EveryoneFilter { get; set; }

        public GroupFilter GroupFilter { get; set; }

        public override string ToString()
        {
            switch (FilterGroupType)
            {
                case FilterGroupType.Everyone:
                    return EveryoneFilter.ToString();

                case FilterGroupType.Group:
                    return GroupFilter.ToString();

                case FilterGroupType.User:
                    return UserFilter.ToString();
            }

            return "ERROR!";
        }

        public string ToXml()
        {
            switch (FilterGroupType)
            {
                case FilterGroupType.Everyone:
                    return EveryoneFilter.ToXml();

                case FilterGroupType.Group:
                    return GroupFilter.ToXml();

                case FilterGroupType.User:
                    return UserFilter.ToXml();
            }

            return "ERROR!";
        }

        public PhotoFilter()
        {
        }

        public PhotoFilter(string str)
        {
            if (UserFilter.IsUserFilter(str))
            {
                FilterGroupType = FilterGroupType.User;
                UserFilter = new UserFilter(str);
            }
            else if (EveryoneFilter.IsEveryoneFilter(str))
            {
                FilterGroupType = FilterGroupType.Everyone;
                EveryoneFilter = new EveryoneFilter(str);
            }
            else if (GroupFilter.IsGroupFilter(str))
            {
                FilterGroupType = FilterGroupType.Group;
                GroupFilter = new GroupFilter(str);
            }
            else
            {
                throw new Exception("Unrecognized Filter string!");
            }
        }
    }

    public class UserFilter
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

        public string ToXml()
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

        public static bool IsUserFilter(string str)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(str);

            var type = "";


            var parentNode = xmlDoc.GetElementsByTagName("Type");
            foreach (XmlNode childrenNode in parentNode)
            {
                type = childrenNode.InnerText;
            }

            return type.Equals("User");
        }

    }


    public class GroupFilter
    {
        public string GroupName { get; set; }

        public string ToXml()
        {
            return "<PhotoFilter><Type>Group</Type><Name>" + GroupName + "</Name></PhotoFilter>";
        }

        public override string ToString()
        {
            return "Group:" + GroupName;
        }

        public static bool IsGroupFilter(string str)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(str);

            var type = "";


            var parentNode = xmlDoc.GetElementsByTagName("Type");
            foreach (XmlNode childrenNode in parentNode)
            {
                type = childrenNode.InnerText;
            }

            return type.Equals("Group");
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

    public class EveryoneFilter
    {
        public enum EveryoneFilterType
        {
            Recent,
            Tags
        }

        public EveryoneFilterType filter { get; set; }

        public string tags { get; set; }

        public bool sortByInterestingness { get; set; }

        public string ToXml()
        {
            string str = "<PhotoFilter> <Type>Everyone</Type> " +
                "<EveryoneFilterType>" + filter.ToString() + "</EveryoneFilterType> " +
                "<Tags>" + tags + "</Tags> " +
                "<SortByInterestingness>" + sortByInterestingness.ToString() + "</SortByInterestingness> </PhotoFilter>";

            return str;
        }

        public override string ToString()
        {
            string str = "Everyone, Filter: " + filter.ToString() + ", Tags:";
            str = str + tags;
            str = str + ", SortByInterestingness:" + sortByInterestingness.ToString();

            return str;
        }

        public static bool IsEveryoneFilter(string str)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(str);

            string type = "";


            XmlNodeList parentNode = xmlDoc.GetElementsByTagName("Type");
            foreach (XmlNode childrenNode in parentNode)
            {
                type = childrenNode.InnerText;
            }

            if (type.Equals("Everyone"))
            {
                return true;
            }

            return false;
        }

        public EveryoneFilter()
        {
        }

        public EveryoneFilter(string str)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(str);

            string value = "";

            XmlNodeList parentNode = xmlDoc.GetElementsByTagName("EveryoneFilterType");
            foreach (XmlNode childrenNode in parentNode)
            {
                value = childrenNode.InnerText;
            }

            if (value.Equals("Recent"))
            {
                filter = EveryoneFilterType.Recent;
            }
            else if (value.Equals("Tags"))
            {
                filter = EveryoneFilterType.Tags;
            }

            parentNode = xmlDoc.GetElementsByTagName("Tags");
            foreach (XmlNode childrenNode in parentNode)
            {
                value = childrenNode.InnerText;
            }

            tags = value;

            parentNode = xmlDoc.GetElementsByTagName("SortByInterestingness");
            foreach (XmlNode childrenNode in parentNode)
            {
                value = childrenNode.InnerText;
            }

            sortByInterestingness = value.Equals("True");
        }

    }

}
