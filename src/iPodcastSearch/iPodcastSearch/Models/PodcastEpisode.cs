namespace iPodcastSearch.Models
{
    using System;

    public class PodcastEpisode
    {
        public string Title { get; set; }

        public string Subtitle { get; set; }

        public string Author { get; set; }

        public string Description { get; set; }

        public DateTime PubDate { get; set; }

        public string Link { get; set; }

        public bool IsExplicit { get; set; }

        public string Summary { get; set; }

        public string AudioFileUrl { get; set; }

        public string Image { get; set; }

        public string AudioFileSize { get; set; }

        public string AudioFileType { get; set; }
        public string Duration { get; set; }
    }
}