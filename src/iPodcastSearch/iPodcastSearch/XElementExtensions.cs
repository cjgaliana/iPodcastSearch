namespace iPodcastSearch
{
    using System;
    using System.Xml.Linq;

    public static class XElementExtensions
    {
        private static readonly XNamespace itunesNamespace = "http://www.itunes.com/dtds/podcast-1.0.dtd";
        private static readonly XNamespace googleplayNamespace = "http://www.google.com/schemas/play-podcasts/1.0";
        private static readonly XNamespace atomNamespace = "http://www.w3.org/2005/Atom";

        public static string GetString(this XElement xElement, string name, string defaultValue = "")
        {
            var element = xElement.Element(name);
            if (element != null)
            {
                return element.Value;
            }

            return defaultValue;
        }

        public static string GetStringFromItunes(this XElement xElement, string name, string defaultValue = "")
        {
            var element = xElement.Element(itunesNamespace + name);
            if (element != null)
            {
                return element.Value;
            }

            return defaultValue;
        }

        public static string GetStringFromAtom(this XElement xElement, string name, string defaultValue = "")
        {
            var element = xElement.Element(atomNamespace + name);
            if (element != null)
            {
                return element.Value;
            }

            return defaultValue;
        }

        public static string GetStringFromGooglePlay(this XElement xElement, string name, string defaultValue = "")
        {
            var element = xElement.Element(googleplayNamespace + name);
            if (element != null)
            {
                return element.Value;
            }

            return defaultValue;
        }

        public static DateTime GetAndroidDateTime(this XElement xElement, string name)
        {
            var dateTime = DateTime.MinValue;
            var element = xElement.Element(googleplayNamespace + name);
            if (element != null)
            {
                var value = element.Value;
                if (DateTime.TryParse(value, out dateTime))
                {
                    return dateTime;
                }
            }

            return dateTime;
        }

        public static string ExtractAttribute(this XElement element, string attributeName)
        {
            if (element != null)
            {
                if (element.HasAttributes)
                {
                    var attr = element.Attribute(attributeName);
                    if (attr != null)
                    {
                        return attr.Value;
                    }
                }
            }
            return string.Empty;
        }

        public static string GetImageUrl(this XElement channel)
        {
            // Search for iTunes image
            var itunesImage = channel.GetStringFromItunes("image");
            if (!string.IsNullOrWhiteSpace(itunesImage))
            {
                return itunesImage;
            }

            // Get from iTunes using Href
            var itunesImageHref = channel.Element(itunesNamespace + "image");
            if (itunesImageHref != null)
            {
                var url = itunesImageHref.ExtractAttribute("href");
                if (!string.IsNullOrWhiteSpace(url))
                {
                    return url;
                }
                return itunesImage;
            }

            // Search for Android Image
            var googleImage = channel.GetStringFromGooglePlay("image");
            if (!string.IsNullOrWhiteSpace(googleImage))
            {
                return googleImage;
            }

            // Search for feed image tag
            var element = channel.Element("image");
            if (element != null)
            {
                var url = element.Element("url")?.Value;
                var title = element.Element("title")?.Value;
                var link = element.Element("link")?.Value;

                if (!string.IsNullOrWhiteSpace(url))
                {
                    return url;
                }
            }

            return string.Empty;
        }
    }
}