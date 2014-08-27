namespace GuideEnricher.Tests
{

    using ArgusTV.DataContracts;
    using GuideEnricher.Model;
    using NUnit.Framework;
    using Should;

    [TestFixture]
    public class GuiderEnricherProgramTests
    {
        [Test]
        public void EpisodeIsEnricherReturnsTrueWhenEpisodeNumberAndSeasonEpisode()
        {
            var guideProgram = new GuideProgram();
            guideProgram.SeriesNumber = 3;
            guideProgram.EpisodeNumber = 10;
            guideProgram.EpisodeNumberDisplay = "S03E10";
            var program = new GuideEnricherProgram(guideProgram);
            program.EpisodeIsEnriched().ShouldBeTrue();
        }

        [Test]
        public void EpisodeIsEnricherReturnsFalseWhenNoEpisodeNumber()
        {
            var guideProgram = new GuideProgram();
            var program = new GuideEnricherProgram(guideProgram);
            program.EpisodeIsEnriched().ShouldBeFalse();
        }

        [Test]
        public void EpisodeIsEnricherReturnsFalseWhenNoSeasonEpisode()
        {
            var guideProgram = new GuideProgram();
            guideProgram.SeriesNumber = 3;
            guideProgram.EpisodeNumber = 10;
            guideProgram.EpisodeNumberDisplay = "Test";
            var program = new GuideEnricherProgram(guideProgram);
            program.EpisodeIsEnriched().ShouldBeFalse();
        }
    }
}