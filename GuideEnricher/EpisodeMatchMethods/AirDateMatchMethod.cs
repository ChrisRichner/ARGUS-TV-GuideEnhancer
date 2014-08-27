namespace GuideEnricher.EpisodeMatchMethods
{
    using System.Collections.Generic;
    using System.Reflection;
    using GuideEnricher.Model;
    using log4net;
    using TvdbLib.Data;
    using System.Linq;

    public class AirDateMatchMethod : MatchMethodBase
    {
        public override string MethodName
        {
            get { return "Original Air Date";  }
        }

        public override bool Match(GuideEnricherProgram guideProgram, List<TvdbEpisode> episodes)
        {
            if (!guideProgram.PreviouslyAiredTime.HasValue)
            {
                return false;
            }

            this.MatchAttempts++;
            var match = episodes.Where(e => e.FirstAired == guideProgram.PreviouslyAiredTime).FirstOrDefault();

            if (match != null)
            {
                return this.Matched(guideProgram, match);
            }

            return this.Unmatched(guideProgram);
        }
    }
}