namespace iPodcastSearch.Models
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [DataContract]
    public class Podcast
    {
        public Podcast()
        {
            this.Episodes = new List<PodcastEpisode>();
        }

        public string Author { get; set; }

        public string SubTitle { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string FeedUrl { get; set; }

        public string Website { get; set; }

        public string Image { get; set; }

        public bool IsExplicit { get; set; }

        public string Language { get; set; }

        public string Copyright { get; set; }

        public int EpisodeCount { get; set; }

        public DateTime LastUpdate { get; set; }

        public IList<PodcastEpisode> Episodes { get; set; }

        /// <summary>
        /// Used for iTunes search reasons
        /// </summary>
        public long Id { get; set; }
    }
}