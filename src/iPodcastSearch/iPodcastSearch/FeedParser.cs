namespace iPodcastSearch
{
    using iPodcastSearch.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    public static class FeedParser
    {
        private static readonly XNamespace itunesNamespace = "http://www.itunes.com/dtds/podcast-1.0.dtd";
        private static readonly XNamespace googleplayNamespace = "http://www.google.com/schemas/play-podcasts/1.0";
        private static readonly XNamespace atomNamespace = "http://www.w3.org/2005/Atom";

        public static async Task<Podcast> LoadFeedAsync(string url)
        {
            var client = new HttpClient();
            var xmlString = await client.GetStringAsync(url);
            return FeedParser.ParseFeed(xmlString);
        }

        public static Podcast ParseFeed(string xml)
        {
            var xmlRoot = XDocument.Parse(xml);

            var podcast = ExtractPodcastMetadata(xmlRoot);
            var episodes = ExtractEpisodes(xmlRoot);
            podcast.Episodes = episodes;
            podcast.EpisodeCount = episodes.Count;
            return podcast;
        }

        private static IList<PodcastEpisode> ExtractEpisodes(XDocument xmlRoot)
        {
            var episodes = new List<PodcastEpisode>();
            var items = xmlRoot.Descendants("item");
            foreach (var item in items)
                try
                {
                    var title = item.Element("title")?.Value;
                    var subtitle = item.Element(itunesNamespace + "subtitle")?.Value;
                    var author = item.Element(itunesNamespace + "author")?.Value;

                    var description = item.Element("description")?.Value;
                    var duration = item.Element(itunesNamespace + "duration")?.Value;

                    var isExplicit = item.Element(itunesNamespace + "explicit")?.Value.ToLowerInvariant() ==
                                     "Yes".ToLowerInvariant();
                    var image = item.Element(itunesNamespace + "image")?.Value;

                    var link = item.Element("link")?.Value;
                    var pubDate = item.Element("pubDate")?.Value;

                    var summary = item.Element(itunesNamespace + "summary")?.Value;

                    var audioUrl = item.Element("enclosure")?.ExtractAttribute("href");
                    if (string.IsNullOrEmpty(audioUrl))
                    {
                        audioUrl = item.Element("enclosure")?.ExtractAttribute("url");
                    }
                    var audioSize = item.Element("enclosure")?.ExtractAttribute("length");
                    var audioType = item.Element("enclosure")?.ExtractAttribute("type");

                    episodes.Add(new PodcastEpisode
                    {
                        Title = title,
                        Author = author,
                        Description = description,
                        Duration = TimeSpan.Zero,
                        AudioFileUrl = audioUrl,
                        AudioFileSize = audioSize,
                        AudioFileType = audioType,
                        IsExplicit = isExplicit,
                        Image = image,
                        Link = link,
                        PubDate = pubDate,
                        Subtitle = subtitle,
                        Summary = summary
                    });
                }
                catch (Exception ex)
                {
                    var a = ex.Message;
                    throw;
                }

            return episodes;
        }

        private static Podcast ExtractPodcastMetadata(XDocument xmlRoot)
        {
            var podcast = new Podcast();

            var channel = xmlRoot?.Descendants("channel").FirstOrDefault();
            if (channel == null)
            {
                throw new Exception("Incorrect XML");
            }
            // General
            var title = channel.Element("title")?.Value;
            var link = channel.Element("link")?.Value;
            var lastBuildDate = channel.Element("lastBuildDate")?.Value;
            var description = channel.Element("description")?.Value;
            var language = channel.Element("language")?.Value;
            var copyright = channel.Element("copyright")?.Value;
            // TODO: Parse images
            //var image = new PodcastImage //image
            //            {
            //                Url = image //url
            //                Title = image //title
            //                Link = image //link
            //};

            // Itunes
            var subtitle = channel.Element(itunesNamespace + "subtitle")?.Value;
            var author = channel.Element(itunesNamespace + "author")?.Value;
            var isExplicit = channel.Element(itunesNamespace + "explicit")?.Value.ToLowerInvariant() !=
                             "clean".ToLowerInvariant();
            var itunesImage = channel.Element(itunesNamespace + "image")?.ExtractAttribute("href");

            var summary = channel.Element(itunesNamespace + "summary")?.Value;
            var owner = channel.Element(itunesNamespace + "owner")?.Value;
            //var ownerName = channel.Element(itunesNamespace + "name")?.Value;
            //var ownerEmail = channel.Element(itunesNamespace + "email")?.Value;

            // TODO: Parse categories
            // Categories
            //< itunes:category text = "Technology" >
            // < itunes:category text = "Podcasting" ></ itunes:category >
            //  </ itunes:category >
            //   < itunes:category text = "Technology" >
            //        < itunes:category text = "Software How-To" ></ itunes:category >
            //         </ itunes:category >

            // Android
            var androidEmail = channel.Element(googleplayNamespace + "email")?.Value;
            var androidAuthor = channel.Element(googleplayNamespace + "author")?.Value;
            var androidIsExplicit = channel.Element(googleplayNamespace + "explicit")?.Value;
            var androidDescription = channel.Element(googleplayNamespace + "description")?.Value;
            // TODO: Fix images
            var androidImage = channel.Element(googleplayNamespace + "image")?.ExtractAttribute("href");

            var feedUrl = channel.Element(atomNamespace + "link")?.Value;

            podcast.Language = language;
            podcast.Author = author;
            podcast.Image = itunesImage;
            podcast.Copyright = copyright;
            podcast.Name = title;
            podcast.Description = description;
            podcast.IsExplicit = isExplicit;
            podcast.FeedUrl = feedUrl;
            podcast.Website = link;
            podcast.LastUpdate = lastBuildDate;

            return podcast;
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
            return "";
        }
    }
}