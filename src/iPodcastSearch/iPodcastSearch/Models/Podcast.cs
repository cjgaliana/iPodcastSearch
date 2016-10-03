namespace iPodcastSearch.Models
{
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

        [DataMember(Name = "collectionName")]
        public string Name { get; set; }

        [DataMember(Name = "longDescription")]
        public string Description { get; set; }

        [DataMember(Name = "feedUrl")]
        public string FeedUrl { get; set; }

        public string Website { get; set; }

        public string Image { get; set; }

        [DataMember(Name = "releaseDate")]
        public string LastUpdate { get; set; }

        [DataMember(Name = "collectionExplicitness")]
        public bool IsExplicit { get; set; }

        [DataMember(Name = "country")]
        public string Language { get; set; }

        [DataMember(Name = "copyright")]
        public string Copyright { get; set; }

        [DataMember(Name = "trackCount")]
        public int EpisodeCount { get; set; }

        public IList<PodcastEpisode> Episodes { get; set; }
    }
}