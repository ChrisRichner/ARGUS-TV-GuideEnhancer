namespace GuideEnricher.EpisodeMatchMethods
{
    using System.Collections.Generic;
    using System.Linq;

    using Model;

    using TvdbLib.Data;
    using System.Text.RegularExpressions;
    using System;

    public class DescriptionStartsWithEpisodeTitleMatchMethod : MatchMethodBase
    {

        public override string MethodName
        {
            get
            {
                return "Description Starts With Episode Title";
            }
        }

        public override bool Match(GuideEnricherProgram guideProgram, List<TvdbEpisode> episodes)
        {
            if (guideProgram == null) throw new ArgumentNullException("enrichedGuideProgram");
            if (IsStringPropertyNull(guideProgram, guideProgram.Description, "Description")) return false;
            //
            var description = guideProgram.Description.ToLower();
            this.MatchAttempts++;
            var matchedEpisode = episodes.FirstOrDefault(x => !string.IsNullOrEmpty(x.EpisodeName) && new Regex(string.Concat("^", x.EpisodeName, "\\W"), RegexOptions.IgnoreCase).IsMatch(description));
            if (matchedEpisode != null)
            {
                return this.Matched(guideProgram, matchedEpisode);
            }

            return false;
        }
    }
}