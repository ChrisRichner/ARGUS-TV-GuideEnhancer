namespace GuideEnricher.EpisodeMatchMethods
{
    using System.Collections.Generic;
    using System.Linq;

    using Model;

    using TvdbLib.Data;
    using System.Text.RegularExpressions;
    using System;

    public class SeasonAndEpisodeInDescriptionMatchMethod : MatchMethodBase
    {
        protected List<Regex> regexes = new List<Regex>{
            new System.Text.RegularExpressions.Regex(@"(?<season>\d+)\.\s*\w+?,\s*\w+?\s*(?<episode>\d+)"),
            new System.Text.RegularExpressions.Regex(@"(?<episode>\d+)/(?<season>[IVX]+)")
        };

        public override string MethodName
        {
            get
            {
                return "Season and Episode in Description";
            }
        }

        public override bool Match(GuideEnricherProgram guideProgram, List<TvdbEpisode> episodes)
        {
            if (guideProgram == null) throw new ArgumentNullException("enrichedGuideProgram");
            //
            Match match = null;
            int index = 0;
            do
            {
                match = regexes[index++].Match(guideProgram.Description);
            } while (string.IsNullOrEmpty(match.Value) && index < regexes.Count);
            if (match != null && !string.IsNullOrEmpty(match.Value))
            {
                this.MatchAttempts++;
                int seasonNumber = 0;
                int episodeNumber = 0;
                if (!int.TryParse(match.Groups["season"].Value, out seasonNumber))
                {
                    // roman literal?
                    seasonNumber = this.RomanToNumeric(match.Groups["season"].Value);
                }
                if (!int.TryParse(match.Groups["episode"].Value, out episodeNumber))
                {
                    // roman literal?
                    episodeNumber = this.RomanToNumeric(match.Groups["episode"].Value);
                }
                if (seasonNumber != 0 && episodeNumber != 0)
                {
                    var matchedEpisode = episodes.FirstOrDefault(x => x.SeasonNumber == seasonNumber && x.EpisodeNumber == episodeNumber);
                    if (matchedEpisode != null)
                    {
                        return this.Matched(guideProgram, matchedEpisode);
                    }
                }
            }
            return this.Unmatched(guideProgram);
        }

        protected int RomanToNumeric(string romanNum)
        {
            int[] ints = romanNum.ToCharArray().Select(x => { return x == 'I' ? 1 : x == 'V' ? 5 : x == 'X' ? 10 : x == 'L' ? 50 : x == 'C' ? 100 : x == 'D' ? 500 : x == 'M' ? 1000 : 0; }).ToArray();
            int calc = 0;
            for (int i = 0; i < ints.Length; i++)
            {
                int mult = (i + 1 < ints.Length && ints[i + 1] > ints[i]) ? -1 : 1;
                calc += mult * ints[i];
            }
            return calc;
        }

    }
}