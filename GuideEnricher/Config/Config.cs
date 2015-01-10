namespace GuideEnricher.Config
{
    using System.Collections.Generic;
    using System.Configuration;

    public sealed class Config : IConfiguration
    {
        private static readonly Config configInstance = new Config();

        private Config()
        {
        }

        public static Config Instance
        {
            get
            {
                return configInstance;
            }
        }

        public string GetProperty(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        public Dictionary<string, string> getSeriesNameMap()
        {
            SeriesNameMapsSection mapSec = ConfigurationManager.GetSection("seriesMapping") as SeriesNameMapsSection;
            if (mapSec == null)
            {
                return new Dictionary<string, string>(0);
            }

            Dictionary<string, string> series = new Dictionary<string, string>(mapSec.SeriesMapping.Count);

            for (int i = 0; i < mapSec.SeriesMapping.Count; i++)
            {
                series.Add(mapSec.SeriesMapping[i].SchedulesDirectName, mapSec.SeriesMapping[i].TvdbComName);
            }

            return series;
        }

        public List<string> getIgnoredSeries()
        {
            var mapSec = ConfigurationManager.GetSection("seriesMapping") as SeriesNameMapsSection;

            if (mapSec == null)
            {
                return null;
            }

            List<string> l = new List<string>();

            for (int i = 0; i < mapSec.SeriesMapping.Count; i++)
            {
                if (mapSec.SeriesMapping[i].Ignore)
                {
                    l.Add(mapSec.SeriesMapping[i].SchedulesDirectName);
                }
            }
            return l;
        }

        public bool UpdateMatchedEpisodes
        {
            get
            {
                bool returnVal;
                if(!bool.TryParse(ConfigurationManager.AppSettings["updateAll"], out returnVal))
                {
                    return false;
                }

                return returnVal;
            }
        }

        public bool UpdateSubtitlesParameter
        {
            get
            {
                bool returnVal;
                if (!bool.TryParse(ConfigurationManager.AppSettings["updateSubtitles"], out returnVal))
                {
                    return false;
                }

                return returnVal;
            }
        }

        public bool UpdateDescription
        {
            get
            {
                bool returnVal;
                if (!bool.TryParse(ConfigurationManager.AppSettings["episodeInDescription"], out returnVal))
                {
                    return false;
                }

                return returnVal;
            }
        }

        public string CacheFolder
        {
            get
            {
                return ConfigurationManager.AppSettings["TvDbLibCache"];
            }
        }

        public string ApiKey
        {
            get
            {
                return "BBB734ABE146900D";
            }
        }
    }
}
