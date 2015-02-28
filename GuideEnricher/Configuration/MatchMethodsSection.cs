namespace GuideEnricher.Config
{
    using System.Configuration;

    public class MatchMethodsSection : ConfigurationSection
    {
        [ConfigurationProperty("MatchMethods", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(MatchMethodCollection), AddItemName = "add")]
        public MatchMethodCollection MatchMethods
        {
            get
            {
                return (MatchMethodCollection)base["MatchMethods"];
            }
        }
    }
}