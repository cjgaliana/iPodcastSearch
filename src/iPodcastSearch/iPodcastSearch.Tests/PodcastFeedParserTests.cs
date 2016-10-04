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
        public async Task GetPodcastFromFeedUrlAsyc_ValidFeedFromSimplePodcastWordpress_ReturnsData()
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
        public void GetPodcastFromFeedUrlAsyc_ValidFeedFromSoundcloud_ReturnsData()
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

        private string LoadXML(string path)
        {
            var text = File.ReadAllText(path);
            return text;
        }
    }
}