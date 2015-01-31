namespace GuideEnricher.EpisodeMatchMethods
{
    using System.Collections.Generic;
    using System.Reflection;
    using GuideEnricher.Model;
    using log4net;
    using TvdbLib.Data;
    using System;

    public class EpisodeTitleMatchMethod : MatchMethodBase
    {
        private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public override string MethodName
        {
            get { return "Episode Title"; }
        }       

        public override bool Match(GuideEnricherProgram guideProgram, List<TvdbEpisode> episodes)
        {
            if (guideProgram == null) throw new ArgumentNullException("guideProgram");
            if (IsStringPropertyNull(guideProgram, guideProgram.SubTitle, "SubTitle")) return false;
            //
            this.MatchAttempts++;
            foreach (var episode in episodes)
            {
                if (episode.EpisodeName == guideProgram.SubTitle)
                {
                    return this.Matched(guideProgram, episode);
                }
            }

            return this.Unmatched(guideProgram);
        }
    }
}