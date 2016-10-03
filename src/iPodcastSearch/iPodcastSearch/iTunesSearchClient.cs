namespace iPodcastSearch
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using iPodcastSearch.Models;

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

        public Task<IList<Podcast>> GetPodcastsAsync(string query)
        {
            return this.GetPodcastsAsync(query, 100, this.defaultLang);
        }

        public Task<IList<Podcast>> GetPodcastsAsync(string query, int resultLimit)
        {
            return this.GetPodcastsAsync(query, resultLimit, this.defaultLang);
        }

        /// <summary>
        ///     Get a list of episodes for a given Podcast
        /// </summary>
        /// <param name="podcast">The Podcast name to search for</param>
        /// <param name="resultLimit">Limit the result count to this number</param>
        /// <param name="countryCode">
        ///     The two-letter country ISO code for the store you want to search.
        ///     See http://en.wikipedia.org/wiki/%20ISO_3166-1_alpha-2 for a list of ISO country codes
        /// </param>
        /// <returns></returns>
        public async Task<IList<Podcast>> GetPodcastsAsync(string podcast, int resultLimit, string countryCode)
        {
            var nvc = HttpUtility.ParseQueryString(string.Empty);

            nvc.Add("term", podcast);
            nvc.Add("media", "podcast");
            nvc.Add("attribute", "titleTerm");
            nvc.Add("limit", resultLimit.ToString());
            nvc.Add("country", countryCode);

            //  Construct the url:
            var apiUrl = string.Format(this._baseSearchUrl, nvc);

            //  Get the list of episodes
            var result = await this.MakeAPICall<PodcastListResult>(apiUrl);

            return result.Podcasts;
        }

        public async Task<Podcast> GetPodcastByIdAsync(long podcastId)
        {
            var nvc = HttpUtility.ParseQueryString(string.Empty);

            //  Set attributes for a podcast
            nvc.Add("id", podcastId.ToString());

            //  Construct the url:
            var apiUrl = string.Format(this._baseLookupUrl, nvc);

            //  Get the list of podcasts
            var result = await this.MakeAPICall<PodcastListResult>(apiUrl);
            return result.Podcasts.FirstOrDefault();
        }

        public async Task<IList<PodcastEpisode>> GetPodcastEpisodesAsync(string feedUrl)
        {
            var podcast = await FeedParser.LoadFeedAsync(feedUrl);
            return podcast.Episodes;
        }

        public async Task<Podcast> GetPodcastFromFeedUrlAsyc(string feedUrl)
        {
            var client = new HttpClient();
            var xmlString = await client.GetStringAsync(feedUrl);

            var podcast = FeedParser.ParseFeed(xmlString);
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