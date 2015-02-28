namespace GuideEnricher.EpisodeMatchMethods
{
    using System.Collections.Generic;
    using Model;
    using TvdbLib.Data;

    public interface IEpisodeMatchMethod
    {
        int MatchAttempts { get; }
        
        int SuccessfulMatches { get; }

        string MethodName { get; }

        bool Match(GuideEnricherProgram enrichedGuideProgram, List<TvdbEpisode> episodes);
    }
}