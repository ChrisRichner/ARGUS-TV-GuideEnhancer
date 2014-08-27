namespace GuideEnricher.EpisodeMatchMethods
{
    using System.Collections.Generic;
    using System.Linq;

    using GuideEnricher.Model;

    using TvdbLib.Data;

    public class FirstSentenceInDescriptionMatchMethod : MatchMethodBase
    {
        protected System.Text.RegularExpressions.Regex firstSentence = new System.Text.RegularExpressions.Regex(@"^[\w\s]+(?=\.)");

        public override string MethodName
        {
            get
            {
                return "First Sentence in Description";
            }
        }

        public override bool Match(GuideEnricherProgram enrichedGuideProgram, List<TvdbEpisode> episodes)
        {
            var match = firstSentence.Match(enrichedGuideProgram.Description);
            if (match == null || string.IsNullOrEmpty(match.Value))
                return false;
            
            this.MatchAttempts++;
            var matchedEpisode = episodes.FirstOrDefault(x => x.EpisodeName == match.Value);
            if (matchedEpisode != null)
            {
                return this.Matched(enrichedGuideProgram, matchedEpisode);
            }
            
            return false;
        }
    }
}