using System.Threading.Tasks;

namespace iPodcastSearch
{
    using iPodcastSearch.Models;
    using System.Collections.Generic;

    public interface IPodcastSearchClient
    {
        Task<IList<Podcast>> SearchPodcastsAsync(string query);

        Task<IList<Podcast>> SearchPodcastsAsync(string query, int resultLimit);

        Task<IList<Podcast>> SearchPodcastsAsync(string query, int resultLimit, string countryCode);

        Task<Podcast> GetPodcastByIdAsync(long podcastId, bool includeEpisodes = false);

        Task<IList<PodcastEpisode>> GetPodcastEpisodesAsync(string feedUrl);

        Task<Podcast> GetPodcastFromFeedUrlAsyc(string feedUrl);
    }
}