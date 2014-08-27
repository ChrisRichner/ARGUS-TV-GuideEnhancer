namespace GuideEnricher.Model
{
    using System.Collections.Generic;

    public class GuideProgramEqualityComparer : IEqualityComparer<GuideEnricherProgram>
    {
        public bool Equals(GuideEnricherProgram x, GuideEnricherProgram y)
        {
            return x.GuideProgramId == y.GuideProgramId;
        }

        public int GetHashCode(GuideEnricherProgram obj)
        {
            return obj.GetHashCode();
        }
    }
}