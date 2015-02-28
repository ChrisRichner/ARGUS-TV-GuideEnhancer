namespace GuideEnricher.Config
{
    using System.Configuration;

    public class SeriesNameMapsSection : ConfigurationSection
    {
        private static readonly ConfigurationProperty _propSeriesMap = new ConfigurationProperty(
            null,
            typeof(SeriesNameMapCollection),
            null,
            ConfigurationPropertyOptions.IsDefaultCollection
            );

        private static ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();

        static SeriesNameMapsSection()
        {
            _properties.Add(_propSeriesMap);
        }

        [ConfigurationProperty("", Options = ConfigurationPropertyOptions.IsDefaultCollection)]
        public SeriesNameMapCollection SeriesMapping
        {
            get { return (SeriesNameMapCollection) base[_propSeriesMap]; }
        }
    }
}