namespace iPodcastSearch.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    [TestClass]
    public class PodcastFeedParserTests
    {
        [TestInitialize]
        public void InitializeTest()
        {
        }

        [TestMethod]
        public async Task GetPodcastFromFeedUrlAsyc_ValidFeedFrom_ReturnsData()
        {
            //  Arrange
            var feedUrl = "http://jitpodcast.com/feed/podcast";

            //  Act
            var podcast = await PodcastFeedParser.LoadFeedAsync(feedUrl);

            //  Assert
            Assert.IsNotNull(podcast);
            Assert.IsTrue(podcast.Episodes.Any());
        }

        [TestMethod]
        public void GetPodcastFromFeedUrlAsyc_ValidFeedFromSimplePodcastWordpress_ReturnsData()
        {
            //  Arrange
            var fakeXML = this.LoadXML(Path.Combine("PodcastFeedTests", "JustInTimeFeed.xml"));

            //  Act
            //var podcast = await PodcastFeedParser.LoadFeedAsync(feedUrl);
            var podcast = PodcastFeedParser.ParseFeed(fakeXML);

            //  Assert
            Assert.IsNotNull(podcast);
            Assert.AreEqual("Just in Time Podcast", podcast.Name);
            Assert.IsFalse(podcast.IsExplicit);
            Assert.AreEqual(new DateTime(2016, 09, 02), podcast.LastUpdate.Date);
            Assert.AreEqual("Podcast sobre desarrollo de software", podcast.SubTitle);
            Assert.AreEqual("http://jitpodcast.com/wp-content/uploads/2016/09/iTunesLogo.png", podcast.Image);
            Assert.IsTrue(podcast.Episodes.Count == 1);
            Assert.IsTrue(podcast.EpisodeCount == 1);
            Assert.AreEqual("JIT Podcast", podcast.Author);
            Assert.AreEqual("© 2016 Just in Time Podcast", podcast.Copyright);
            Assert.AreEqual("es-ES", podcast.Language);
            Assert.AreEqual("http://jitpodcast.com/", podcast.Website);
            Assert.AreEqual("Podcast sobre desarrollo de software: desarrollo movil, buenas practicas, lenguajes y desvarios varios", podcast.Description);
        }

        [TestMethod]
        public void GetPodcastFromFeedUrlAsyc_ValidFeedFromSpreaker_ReturnsData()
        {
            //  Arrange
            var fakeXML = this.LoadXML(Path.Combine("PodcastFeedTests", "SpreakerFeed.xml"));

            //  Act
            //var podcast = await PodcastFeedParser.LoadFeedAsync(feedUrl);
            var podcast = PodcastFeedParser.ParseFeed(fakeXML);

            //  Assert
            Assert.IsNotNull(podcast);
            Assert.AreEqual("Nación Lumpen", podcast.Name);
            Assert.IsFalse(podcast.IsExplicit);
            Assert.AreEqual(DateTime.MinValue, podcast.LastUpdate.Date); // Not found
            Assert.AreEqual("El podcast de unos irreductibles programadores.", podcast.SubTitle);
            Assert.AreEqual("http://d3wo5wojvuv7l.cloudfront.net/t_rss_itunes_square_1400/images.spreaker.com/original/d01cf394fd32ea12e86296d3614b59c5.jpg", podcast.Image);
            Assert.IsTrue(podcast.Episodes.Count == 12);
            Assert.IsTrue(podcast.EpisodeCount == 12);
            Assert.AreEqual("Nación Lumpen", podcast.Author);
            Assert.AreEqual("Copyright Nación Lumpen", podcast.Copyright);
            Assert.AreEqual("es", podcast.Language);
            Assert.AreEqual("http://www.spreaker.com/user/nacionlumpen", podcast.Website);
            Assert.AreEqual("El podcast de unos irreductibles programadores.", podcast.Description);
        }

        [TestMethod]
        public void GetPodcastFromFeedUrlAsyc_ValidFeedFromSpreaker_ReturnsEpisodeData()
        {
            //  Arrange
            var fakeXML = this.LoadXML(Path.Combine("PodcastFeedTests", "SpreakerFeed.xml"));

            //  Act
            //var podcast = await PodcastFeedParser.LoadFeedAsync(feedUrl);
            var podcast = PodcastFeedParser.ParseFeed(fakeXML);
            var episode = podcast.Episodes.FirstOrDefault();

            //  Assert
            Assert.IsNotNull(episode);
            Assert.AreEqual("NL10: programación funcional", episode.Title);
            Assert.AreEqual("http://www.spreaker.com/user/nacionlumpen/nl10-programacion-funcional", episode.Link);
            Assert.IsTrue(!string.IsNullOrWhiteSpace(episode.Description));
            Assert.AreEqual("http://api.spreaker.com/download/episode/9557347/nl10_progfun.mp3", episode.AudioFileUrl);
            Assert.AreEqual("0", episode.AudioFileSize);
            Assert.AreEqual("audio/mpeg", episode.AudioFileType);
            Assert.AreEqual(new DateTime(2016, 10, 3), episode.PubDate.Date);
            Assert.AreEqual("Aprovechando el marco incomparable que nos ofrecía la Lambda World nos propusimos hablar sobre programación funcional con unos invitados excepcionales: Alfonso García-Caro, autor de Fable (compilador de F# a JS) y Alex Serrano, autor de Beginning...", episode.Subtitle);
            Assert.IsTrue(!string.IsNullOrWhiteSpace(episode.Summary));
            Assert.IsFalse(episode.IsExplicit);
            Assert.AreEqual("http://d3wo5wojvuv7l.cloudfront.net/t_rss_itunes_square_1400/images.spreaker.com/original/0d3a54bc5d795761083205300bf53d74.jpg", episode.Image);
        }

        private string LoadXML(string path)
        {
            var text = File.ReadAllText(path);
            return text;
        }
    }
}