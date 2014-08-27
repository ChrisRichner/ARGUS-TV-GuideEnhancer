namespace GuideEnricher.Tests
{
    using ArgusTV.DataContracts;
    using GuideEnricher.Model;

    /// <summary>
    /// Used for unit testing, allows to set the expected result for episode number
    /// </summary>
    public class TestProgram : GuideEnricherProgram
    {
        public TestProgram(string title, string subTitle, int absoluteEpisodeNumber, string expectedEpisodeNumberDisplay)
        {
            this.guideProgram = new GuideProgram();
            this.Title = title;
            this.SubTitle = subTitle;
            this.EpisodeNumber = absoluteEpisodeNumber;
            this.ExpectedEpisodeNumberDisplay = expectedEpisodeNumberDisplay;
        }

        public string ExpectedEpisodeNumberDisplay { get; set; }
    }
}