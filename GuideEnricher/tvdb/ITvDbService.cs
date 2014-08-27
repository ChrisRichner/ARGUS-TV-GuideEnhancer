// // --------------------------------------------------------------------------------------------------------------------
// // <copyright file="ITvDbService.cs" company="Flex Tech Services Inc.">
// //   Copyright (c) 2012, Flex Tech Services Inc.
// // </copyright>
// // <summary>
// //
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------
namespace GuideEnricher.tvdb
{
    using System.Collections.Generic;

    using TvdbLib.Data;

    public interface ITvDbService
    {
        List<TvdbLanguage> Languages { get; }

        TvdbSeries GetSeries(int seriesId, TvdbLanguage language, bool loadEpisodes, bool loadActors, bool loadBanners);

        TvdbSeries ForceReload(TvdbSeries series, bool loadEpisodes, bool loadActors, bool loadBanners);

        List<TvdbSearchResult> SearchSeries(string name, TvdbLanguage language);
    }
}