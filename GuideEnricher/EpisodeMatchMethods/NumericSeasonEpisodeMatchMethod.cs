namespace GuideEnricher.EpisodeMatchMethods
{
    using System.Collections.Generic;
    using System.Linq;

    using GuideEnricher.Model;

    using TvdbLib.Data;

    public class NumericSeasonEpisodeMatchMethod : MatchMethodBase
    {
        public override string MethodName
        {
            get
            {
                return "Three or Four Digit Season Episode";
            }
        }

        public override bool Match(GuideEnricherProgram enrichedGuideProgram, List<TvdbEpisode> episodes)
        {
            var episodeNumber = enrichedGuideProgram.GetValidEpisodeNumber();

            this.MatchAttempts++;

            var matchedEpisode = episodes.FirstOrDefault(x => x.SeasonNumber == episodeNumber / 100 && x.EpisodeNumber == episodeNumber % 100);
            if (matchedEpisode != null)
            {
                return this.Matched(enrichedGuideProgram, matchedEpisode);
            }
            
            return this.Unmatched(enrichedGuideProgram);
        }
    }
}