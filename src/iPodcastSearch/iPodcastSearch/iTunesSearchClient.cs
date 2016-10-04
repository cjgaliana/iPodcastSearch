namespace iPodcastSearch
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using iPodcastSearch.Models;
    using iPodcastSearch.Models.iTunes;

    public class iTunesSearchClient : IPodcastSearchClient
    {
        /// <summary>
        ///     The base API url for iTunes lookups
        /// </summary>
        private readonly string _baseLookupUrl = "https://itunes.apple.com/lookup?{0}";

        /// <summary>
        ///     The base API url for iTunes search
        /// </summary>
        private readonly string _baseSearchUrl = "https://itunes.apple.com/search?{0}";

        private readonly string defaultLang = "us";
        private readonly int defaultResultLimit = 100;


        public Task<IList<Podcast>> SearchPodcastsAsync(string query)
        {
            return this.SearchPodcastsAsync(query, this.defaultResultLimit, this.defaultLang);
        }

        public Task<IList<Podcast>> SearchPodcastsAsync(string query, int resultLimit)
        {
            return this.SearchPodcastsAsync(query, resultLimit, this.defaultLang);
        }

        public async Task<IList<Podcast>> SearchPodcastsAsync(string query, int resultLimit, string countryCode)
        {
            var nvc = HttpUtility.ParseQueryString(string.Empty);

            nvc.Add("term", query);
            nvc.Add("media", "podcast");
            nvc.Add("attribute", "titleTerm");
            nvc.Add("limit", resultLimit.ToString());
            nvc.Add("country", countryCode);

            //  Construct the url:
            var apiUrl = string.Format(this._baseSearchUrl, nvc);

            //  Get the list of episodes
            var result = await this.MakeAPICall<PodcastListResult>(apiUrl);

            // Map data
            var podcasts = result.Podcasts.Select(x => x.ToPodcast()).ToList();

            return podcasts;
        }

        public async Task<Podcast> GetPodcastByIdAsync(long podcastId, bool includeEpisodes = false)
        {
            var nvc = HttpUtility.ParseQueryString(string.Empty);

            //  Set attributes for a podcast
            nvc.Add("id", podcastId.ToString());

            //  Construct the url:
            var apiUrl = string.Format(this._baseLookupUrl, nvc);

            //  Get the list of podcasts
            var result = await this.MakeAPICall<PodcastListResult>(apiUrl);
            var itunesPodcast = result.Podcasts.FirstOrDefault();

            var podcast = itunesPodcast?.ToPodcast();

            if ((podcast != null) && includeEpisodes)
            {
                podcast.Episodes = await this.GetPodcastEpisodesAsync(podcast.FeedUrl);
                podcast.EpisodeCount = podcast.Episodes.Count;
            }

            return podcast;
        }

        public async Task<IList<PodcastEpisode>> GetPodcastEpisodesAsync(string feedUrl)
        {
            var podcast = await PodcastFeedParser.LoadFeedAsync(feedUrl);
            return podcast.Episodes;
        }

        public async Task<Podcast> GetPodcastFromFeedUrlAsyc(string feedUrl)
        {
            var podcast = await PodcastFeedParser.LoadFeedAsync(feedUrl);
            return podcast;
        }

        /// <summary>
        ///     Makes an API call and deserializes return value to the specified type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="apiCall"></param>
        /// <returns></returns>
        private async Task<T> MakeAPICall<T>(string apiCall)
        {
            var client = new HttpClient();

            //  Make an async call to get the response
            var json = await client.GetStringAsync(apiCall).ConfigureAwait(false);

            //  Deserialize and return
            return JsonHelper.Deserialize<T>(json);
        }
    }
}