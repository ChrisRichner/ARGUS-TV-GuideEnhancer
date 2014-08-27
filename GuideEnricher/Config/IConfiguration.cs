namespace GuideEnricher.Config
{
    using System.Collections.Generic;

    public interface IConfiguration
    {
        string getProperty(string key);
        Dictionary<string, string> getSeriesNameMap();
        List<string> getIgnoredSeries();

        bool UpdateMatchedEpisodes { get; }

        bool UpdateSubtitlesParameter { get; }

        bool UpdateDescription { get; }

        string CacheFolder { get; }

        string ApiKey { get; }
    }
}