namespace GuideEnricher
{
    using System.Collections.Generic;
    using TvdbLib.Data;

    public class TvEpisodeComparer : Comparer<TvdbEpisode>
    {
        public override int Compare(TvdbEpisode x, TvdbEpisode y)
        {
            if (x.SeasonNumber == y.SeasonNumber)
            {
                return x.EpisodeNumber.CompareTo(y.EpisodeNumber);
            }

            return x.SeasonNumber.CompareTo(y.SeasonNumber);
        }
    }
}