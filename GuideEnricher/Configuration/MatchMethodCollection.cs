namespace GuideEnricher.Config
{
    using System.Configuration;

    public class MatchMethodCollection : ConfigurationElementCollection
    {

        public MatchMethodElement this[int index]
        {
            get { return (MatchMethodElement)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        public void Add(MatchMethodElement matchMethod)
        {
            BaseAdd(matchMethod);
        }

        public void Clear()
        {
            BaseClear();
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new MatchMethodElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((MatchMethodElement)element).MethodName;
        }

        public void Remove(MatchMethodElement matchMethod)
        {
            BaseRemove(matchMethod.MethodName);
        }

        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        public void Remove(string name)
        {
            BaseRemove(name);
        }
    }
}
