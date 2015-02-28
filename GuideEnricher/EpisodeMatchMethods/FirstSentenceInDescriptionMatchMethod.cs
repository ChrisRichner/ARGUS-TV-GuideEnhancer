namespace GuideEnricher.EpisodeMatchMethods
{
    using System.Collections.Generic;
    using System.Linq;

    using Model;

    using TvdbLib.Data;
    using System;

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

        public override bool Match(GuideEnricherProgram guideProgram, List<TvdbEpisode> episodes)
        {
            if (guideProgram == null) throw new ArgumentNullException("enrichedGuideProgram");
            if (IsStringPropertyNull(guideProgram, guideProgram.Description, "Description")) return false;
            //
            var match = firstSentence.Match(guideProgram.Description);
            if (match == null || string.IsNullOrEmpty(match.Value))
                return false;

            this.MatchAttempts++;
            var matchedEpisode = episodes.FirstOrDefault(x => x.EpisodeName == match.Value);
            if (matchedEpisode != null)
            {
                return this.Matched(guideProgram, matchedEpisode);
            }

            return false;
        }
    }
}