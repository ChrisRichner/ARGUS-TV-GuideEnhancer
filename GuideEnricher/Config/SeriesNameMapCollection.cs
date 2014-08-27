namespace GuideEnricher.Config
{
    using System.Configuration;

    [ConfigurationCollection(typeof(SeriesNameMap), AddItemName = "seriesMap", CollectionType = ConfigurationElementCollectionType.BasicMap)]
    public class SeriesNameMapCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new SeriesNameMap();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((SeriesNameMap) element).SchedulesDirectName;
        }

        public void Add(SeriesNameMap element)
        {
            BaseAdd(element);
        }

        public void Clear()
        {
            BaseClear();
        }

        public int IndexOf(SeriesNameMap element)
        {
            return BaseIndexOf(element);
        }

        public void Remove(SeriesNameMap element)
        {
            if(BaseIndexOf(element) >= 0)
            {
                BaseRemove(element.SchedulesDirectName);
            }
        }

        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        public SeriesNameMap this[int index]
        {
            get { return (SeriesNameMap) BaseGet(index); }
            set
            {
                if(BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }
    }
}