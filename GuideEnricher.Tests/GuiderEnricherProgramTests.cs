namespace GuideEnricher.Tests
{

    using ArgusTV.DataContracts;
    using Model;
    using Should;
    using Xunit;

    public class GuiderEnricherProgramTests
    {
        [Fact]
        public void EpisodeIsEnricherReturnsTrueWhenEpisodeNumberAndSeasonEpisode()
        {
            var guideProgram = new GuideProgram();
            guideProgram.SeriesNumber = 3;
            guideProgram.EpisodeNumber = 10;
            guideProgram.EpisodeNumberDisplay = "S03E10";
            var program = new GuideEnricherProgram(guideProgram);
            program.EpisodeIsEnriched().ShouldBeTrue();
        }

        [Fact]
        public void EpisodeIsEnricherReturnsFalseWhenNoEpisodeNumber()
        {
            var guideProgram = new GuideProgram();
            var program = new GuideEnricherProgram(guideProgram);
            program.EpisodeIsEnriched().ShouldBeFalse();
        }

        [Fact]
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