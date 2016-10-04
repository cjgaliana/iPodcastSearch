namespace iPodcastSearch
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using iPodcastSearch.Models;

    public static class PodcastFeedParser
    {
        public static async Task<Podcast> LoadFeedAsync(string url)
        {
            // Get XML from web
            var client = new HttpClient();
            var xmlString = await client.GetStringAsync(url);

            // Parse data
            var podcast = ParseFeed(xmlString);
            podcast.FeedUrl = url;

            return podcast;
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
                    var title = item.GetString("title");
                    var subtitle = item.GetStringFromItunes("subtitle");
                    var author = item.GetStringFromItunes("author");

                    var description = item.GetString("description");

                    // More info http://lists.apple.com/archives/syndication-dev/2005/Nov/msg00002.html#_Toc526931684
                    var isExplicit = item.IsExplicit();

                    var image = item.GetStringFromItunes("image");
                    var duration = item.GetStringFromItunes("duration");

                    var website = item.GetString("link");
                    var pubDate = item.GetDateTime("pubDate");

                    var summary = item.GetStringFromItunes("summary");

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
                                     AudioFileUrl = audioUrl,
                                     AudioFileSize = audioSize,
                                     AudioFileType = audioType,
                                     IsExplicit = isExplicit,
                                     Image = image,
                                     Link = website,
                                     PubDate = pubDate,
                                     Subtitle = subtitle,
                                     Summary = summary,
                                     Duration = duration
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
            var title = channel.GetString("title");
            var link = channel.GetString("link");
            var lastBuildDate = channel.GetString("lastBuildDate");
            var description = channel.GetString("description");
            var language = channel.GetString("language");
            var copyright = channel.GetString("copyright");


            var image = channel.GetImageUrl();

            // Itunes
            var subtitle = channel.GetStringFromItunes("subtitle");
            var author = channel.GetStringFromItunes("author");
            var isExplicit = channel.IsExplicit();

            var summary = channel.GetStringFromItunes("summary");

            // Todo: get the name/email from owner tag
            //var owner = channel.Element(itunesNamespace + "owner");
            //var ownerName = owner.Element(itunesNamespace + "name")?.Value;
            //var ownerEmail = owner.Element(itunesNamespace + "email")?.Value;


            // TODO: Parse categories
            // Categories
            //< itunes:category text = "Technology" >
            // < itunes:category text = "Podcasting" ></ itunes:category >
            //  </ itunes:category >
            //   < itunes:category text = "Technology" >
            //        < itunes:category text = "Software How-To" ></ itunes:category >
            //         </ itunes:category >

            // TODO: Android tags
            //var androidEmail = channel.Element(googleplayNamespace + "email")?.Value;
            //var androidAuthor = channel.Element(googleplayNamespace + "author")?.Value;
            //var androidIsExplicit = channel.Element(googleplayNamespace + "explicit")?.Value;
            //var androidDescription = channel.Element(googleplayNamespace + "description")?.Value;
            //// TODO: Fix images
            //var androidImage = channel.Element(googleplayNamespace + "image")?.ExtractAttribute("href");

            var feedUrl = channel.GetStringFromAtom("link");


            podcast.Language = language;
            podcast.Author = author;
            podcast.Image = image;
            podcast.Copyright = copyright;
            podcast.Name = title;
            podcast.Description = !string.IsNullOrWhiteSpace(summary) ? summary : description;
            podcast.IsExplicit = isExplicit;
            podcast.FeedUrl = feedUrl;
            podcast.Website = link;
            podcast.LastUpdate = lastBuildDate.ToDateTime();
            podcast.SubTitle = subtitle;

            return podcast;
        }
    }
}