namespace GuideEnricher.EpisodeMatchMethods
{
    using System.Collections.Generic;
    using System.Linq;

    using GuideEnricher.Model;

    using TvdbLib.Data;
    using System.Text.RegularExpressions;

    public class DescriptionStartsWithEpisodeTitleMatchMethod : MatchMethodBase
    {
        
        public override string MethodName
        {
            get
            {
                return "Description Starts With Episode Title";
            }
        }

        public override bool Match(GuideEnricherProgram enrichedGuideProgram, List<TvdbEpisode> episodes)
        {
            var description = enrichedGuideProgram.Description;
            if (description == null || string.IsNullOrEmpty(description))
                return false;

            description = description.ToLower();
            this.MatchAttempts++;
            var matchedEpisode = episodes.FirstOrDefault(x => !string.IsNullOrEmpty(x.EpisodeName) && new Regex(string.Concat("^", x.EpisodeName, "\\W"), RegexOptions.IgnoreCase).IsMatch(description));
            if (matchedEpisode != null)
            {
                return this.Matched(enrichedGuideProgram, matchedEpisode);
            }
            
            return false;
        }
    }
}