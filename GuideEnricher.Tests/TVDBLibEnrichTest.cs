namespace GuideEnricher.Tests
{
    using System;
    using System.Collections.Generic;

    using Config;
    using EpisodeMatchMethods;
    using GuideEnricher.tvdb;

    using log4net.Config;

    using NUnit.Framework;

    using Should;
    using System.Globalization;


    /// <summary>
    /// Use this class to test actual episodes against TVDB
    /// This is useful to test if a series will be matched correctly given a configuration
    /// </summary>
    /// These tests are being ignored for now as TVDB is not always responding and slowing down tests
    /// It's bad practice anyways ;)
    [TestFixture]
    //[Ignore]
    public class TVDBLibEnrichTest
    {
        private List<TestProgram> testPrograms;

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            BasicConfigurator.Configure();
        }

        private void CreateTestData()
        {
            this.testPrograms = new List<TestProgram>();
            //            this.testPrograms.Add(new TestProgram("House", "the fix (153)", 153, "S07E21"));
            //            this.testPrograms.Add(new TestProgram("Chuck", "Chuck versus the last details (83)", 83, "S05E05"));
            //            this.testPrograms.Add(new TestProgram("Family Guy", "Brian sings and swings (70)", 70, "S04E19"));
            //            this.testPrograms.Add(new TestProgram("Family Guy", "Deep Throats", 74, "S04E23"));
            //            this.testPrograms.Add(new TestProgram("The Big Bang Theory", "The Zazzy Substitution", 0, "S04E03"));
            //            this.testPrograms.Add(new TestProgram("Castle", "Pretty Dead (59)", 59, "S03E23"));
            this.testPrograms.Add(new TestProgram("Shark Tank", "Episode 2", 202, "S02E02"));
        }

        [Test]
        public void SeriesWithPunctuationsAreMatchedCorrectly()
        {
            var matchMethods = new List<IEpisodeMatchMethod>();

            var tvDbService = new TvDbService("tvdbCache", Config.Instance.ApiKey);
            var tvdbLib = new TvdbLibAccess(Config.Instance, matchMethods, tvDbService);

            var seriesID = tvdbLib.getSeriesId("American Dad");

            seriesID.ShouldEqual(73141);
        }

        [Test]
        public void TestEnricherMethods()
        {
            var tvDbService = new TvDbService(Config.Instance.CacheFolder, Config.Instance.ApiKey);
            var enricher = new TvdbLibAccess(Config.Instance, EpisodeMatchMethodLoader.GetMatchMethods(), tvDbService);
            this.CreateTestData();

            bool pass = true;
            foreach (var testProgram in this.testPrograms)
            {
                try
                {
                    var series = enricher.GetTvdbSeries(enricher.getSeriesId(testProgram.Title), false);
                    enricher.EnrichProgram(testProgram, series);
                    if (testProgram.EpisodeNumberDisplay == testProgram.ExpectedEpisodeNumberDisplay)
                    {
                        Console.WriteLine(string.Format(CultureInfo.CurrentCulture, "Correctly matched {0} - {1}", testProgram.Title, testProgram.EpisodeNumberDisplay));
                    }
                    else
                    {
                        Console.WriteLine(string.Format(CultureInfo.CurrentCulture, "Unable to match {0} - {1}", testProgram.Title, testProgram.SubTitle));
                        pass = false;
                    }

                }
                catch (Exception exception)
                {
                    pass = false;
                    Console.WriteLine(string.Format(CultureInfo.CurrentCulture, "Couldn't match {0} - {1}", testProgram.Title, testProgram.SubTitle));
                    Console.WriteLine(exception.Message);
                }
            }

            if (pass)
            {
                Assert.Pass();
            }
            else
            {
                Assert.Fail();
            }
        }

        [Test]
        public void TestMappingNameWithID()
        {
            var lawOrderProgram = new TestProgram("Law & Order: Special Victims Unit", "Identity", 0, "S06E12");
            var seriesNameMap = new Dictionary<string, string>(1);
            seriesNameMap.Add("Law & Order: Special Victims Unit", "id=75692");
            var mockConfig = new Moq.Mock<IConfiguration>();
            mockConfig.Setup(x => x.getSeriesNameMap()).Returns(seriesNameMap);
            var tvDbApi = new TvDbService(TestContext.CurrentContext.WorkDirectory, Config.Instance.ApiKey);
            var enricher = new TvdbLibAccess(mockConfig.Object, EpisodeMatchMethodLoader.GetMatchMethods(), tvDbApi);
            var series = enricher.GetTvdbSeries(enricher.getSeriesId(lawOrderProgram.Title), false);
            enricher.EnrichProgram(lawOrderProgram, series);
            Assert.IsTrue(lawOrderProgram.EpisodeIsEnriched());
        }

        [Test]
        public void TestMappingRegex()
        {
            var lawOrderProgram = new TestProgram("Stargate Atlantis123", "Common Ground", 0, "S03E07");
            var seriesNameMap = new Dictionary<string, string>(1);
            seriesNameMap.Add("regex=Stargate Atl.*", "Stargate Atlantis");
            var mockConfig = new Moq.Mock<IConfiguration>();
            mockConfig.Setup(x => x.getSeriesNameMap()).Returns(seriesNameMap);
            var tvDbApi = new TvDbService(TestContext.CurrentContext.WorkDirectory, Config.Instance.ApiKey);
            var enricher = new TvdbLibAccess(mockConfig.Object, EpisodeMatchMethodLoader.GetMatchMethods(), tvDbApi);
            var series = enricher.GetTvdbSeries(enricher.getSeriesId(lawOrderProgram.Title), false);
            enricher.EnrichProgram(lawOrderProgram, series);
            Assert.IsTrue(lawOrderProgram.EpisodeIsEnriched());
        }

        [Test]
        public void TestRegularMapping()
        {
            var lawOrderProgram = new TestProgram("Stargate Atlantis123", "Common Ground", 0, "S03E07");
            var seriesNameMap = new Dictionary<string, string>(1);
            seriesNameMap.Add("Stargate Atlantis123", "Stargate Atlantis");
            var mockConfig = new Moq.Mock<IConfiguration>();
            mockConfig.Setup(x => x.getSeriesNameMap()).Returns(seriesNameMap);
            var tvDbApi = new TvDbService(TestContext.CurrentContext.WorkDirectory, Config.Instance.ApiKey);
            var enricher = new TvdbLibAccess(mockConfig.Object, EpisodeMatchMethodLoader.GetMatchMethods(), tvDbApi);
            var series = enricher.GetTvdbSeries(enricher.getSeriesId(lawOrderProgram.Title), false);
            enricher.EnrichProgram(lawOrderProgram, series);
            Assert.IsTrue(lawOrderProgram.EpisodeIsEnriched());
        }

        [Test]
        public void TestAgathaChristiesMarple()
        {
            // Arrange
            var program = new TestProgram("Agatha Christie's Marple", "Murder at the Vicarage", 0, "S01E02");
            var seriesNameMap = new Dictionary<string, string>(1);
            seriesNameMap.Add("Agatha Christie's Marple", "id=78895");
            var mockConfig = new Moq.Mock<IConfiguration>();
            mockConfig.Setup(x => x.getSeriesNameMap()).Returns(seriesNameMap);
            mockConfig.Setup(x => x.UpdateMatchedEpisodes).Returns(true);
            mockConfig.Setup(x => x.UpdateSubtitlesParameter).Returns(true);
            var tvDbApi = new TvDbService(TestContext.CurrentContext.WorkDirectory, Config.Instance.ApiKey);
            var enricher = new TvdbLibAccess(mockConfig.Object, EpisodeMatchMethodLoader.GetMatchMethods(), tvDbApi);
            var series = enricher.GetTvdbSeries(enricher.getSeriesId(program.Title), false);
            // Act
            enricher.EnrichProgram(program, series);
            // Assert
            Assert.IsTrue(program.EpisodeIsEnriched());
            program.EpisodeNumber.ShouldEqual(2);
            program.SeriesNumber.ShouldEqual(1);
        }
    }
}
