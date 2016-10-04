namespace iPodcastSearch
{
    using iPodcastSearch.Models;
    using iPodcastSearch.Models.iTunes;
    using System;
    using System.Xml.Linq;

    public static class iTunesExtensions
    {
        public static bool IsExplicit(this XElement element)
        {
            // More info http://lists.apple.com/archives/syndication-dev/2005/Nov/msg00002.html#_Toc526931684
            var actual = element.GetStringFromItunes("explicit");
            return actual.IsExplicit();
        }

        public static bool IsExplicit(this string value)
        {
            var isExplicit = string.Equals(value, "yes", StringComparison.CurrentCultureIgnoreCase);
            return isExplicit;
        }

        public static DateTime ToDateTime(this string value)
        {
            var dateTime = DateTime.MinValue;

            DateTime.TryParse(value, out dateTime);

            return dateTime;
        }

        public static Podcast ToPodcast(this iTunesPodcast iTunesPodcast)
        {
            var podcast = new Podcast
            {
                Name = iTunesPodcast.Name,
                Author = iTunesPodcast.ArtistName,
                Id = iTunesPodcast.Id,
                EpisodeCount = iTunesPodcast.EpisodeCount,
                Copyright = iTunesPodcast.Copyright,
                Description = iTunesPodcast.Description,
                FeedUrl = iTunesPodcast.FeedUrl,
                IsExplicit = iTunesPodcast.Explicitness.IsExplicit(),
                Image = iTunesPodcast.ArtworkUrlLarge,
                SubTitle = iTunesPodcast.ReleaseDate,
                LastUpdate = iTunesPodcast.ReleaseDate.ToDateTime(),
                Language = iTunesPodcast.Country,
                Website = iTunesPodcast.PodcastViewUrl
            };

            return podcast;
        }
    }
}