using System.Threading.Tasks;

namespace iPodcastSearch
{
    using iPodcastSearch.Models;
    using System.Collections.Generic;

    public interface IPodcastSearchClient
    {
        Task<IList<Podcast>> GetPodcastsAsync(string query);

        Task<IList<Podcast>> GetPodcastsAsync(string query, int resultLimit);

        Task<IList<Podcast>> GetPodcastsAsync(string query, int resultLimit, string countryCode);

        Task<Podcast> GetPodcastByIdAsync(long podcastId);

        Task<IList<PodcastEpisode>> GetPodcastEpisodesAsync(string feedUrl);

        Task<Podcast> GetPodcastFromFeedUrlAsyc(string feedUrl);
    }
}