// // --------------------------------------------------------------------------------------------------------------------
// // <copyright file="ThreeOrFourDigitSeasonEpisodeMatchMethodTests.cs" company="Flex Tech Services Inc.">
// //   Copyright (c) 2012, Flex Tech Services Inc.
// // </copyright>
// // <summary>
// //
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------
namespace GuideEnricher.Tests.MatchMethodsTests
{
    using System.Collections.Generic;

    using ArgusTV.DataContracts;

    using EpisodeMatchMethods;
    using Model;

    using TvdbLib.Data;

    using Should;
    using Xunit;

    public class ThreeOrFourDigitSeasonEpisodeMatchMethodTests
    {
        private readonly NumericSeasonEpisodeMatchMethod matcher = new NumericSeasonEpisodeMatchMethod();

        private List<TvdbEpisode> episodes;

        public ThreeOrFourDigitSeasonEpisodeMatchMethodTests()
        {
            this.episodes = new List<TvdbEpisode>(3)
                                {
                                    new TvdbEpisode { SeasonNumber = 1, EpisodeNumber = 8, EpisodeName = "Test 1-8" },
                                    new TvdbEpisode { SeasonNumber = 11, EpisodeNumber = 8, EpisodeName = "Test 11-8" },
                                    new TvdbEpisode { SeasonNumber = 3, EpisodeNumber = 2, EpisodeName = "Test 3-2" }
                                };
        }

        [Fact]
        public void EpisodeAsSubTitle302MapsToSeason3Episode2()
        {
            var program = new GuideEnricherProgram(new GuideProgram { Title = "Some Program", SubTitle = "302" });
            this.matcher.Match(program, this.episodes);

            program.SeriesNumber.ShouldEqual(3);
            program.EpisodeNumber.ShouldEqual(2);
            program.SubTitle.ShouldEqual("Test 3-2");
            program.EpisodeNumberDisplay.ShouldEqual("S03E02");
        }

        [Fact]
        public void EpisodeNumber302MapsToSeason3Episode2()
        {
            var program = new GuideEnricherProgram(new GuideProgram { Title = "Some Program", EpisodeNumber = 302 });
            this.matcher.Match(program, this.episodes);

            program.SeriesNumber.ShouldEqual(3);
            program.EpisodeNumber.ShouldEqual(2);
            program.SubTitle.ShouldEqual("Test 3-2");
            program.EpisodeNumberDisplay.ShouldEqual("S03E02");
        }

        [Fact]
        public void EpisodeAsSubTitle1108MapsToSeason11Episode8()
        {
            var program = new GuideEnricherProgram(new GuideProgram { Title = "Some Program", SubTitle = "1108" });
            this.matcher.Match(program, this.episodes);

            program.SeriesNumber.ShouldEqual(11);
            program.EpisodeNumber.ShouldEqual(8);
            program.SubTitle.ShouldEqual("Test 11-8");
            program.EpisodeNumberDisplay.ShouldEqual("S11E08");
        }

        [Fact]
        public void EpisodeNumber1108MapsToSeason11Episode8()
        {
            var program = new GuideEnricherProgram(new GuideProgram { Title = "Some Program", EpisodeNumber = 1108 });
            this.matcher.Match(program, this.episodes);

            program.SeriesNumber.ShouldEqual(11);
            program.EpisodeNumber.ShouldEqual(8);
            program.SubTitle.ShouldEqual("Test 11-8");
            program.EpisodeNumberDisplay.ShouldEqual("S11E08");
        }
    }
}