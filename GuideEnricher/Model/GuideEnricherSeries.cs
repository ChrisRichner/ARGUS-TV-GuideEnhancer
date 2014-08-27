namespace GuideEnricher.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class GuideEnricherSeries
    {
        private readonly bool updateAll;
        private readonly bool updateSubtitles;
        private readonly bool updateDescription;

        private static readonly GuideProgramEqualityComparer guideProgramEqualityComparer = new GuideProgramEqualityComparer();

        public GuideEnricherSeries(String title, bool updateAllParameter, bool updateSubtitlesParameter, bool updateDescription)
        {
            this.Title = title;
            this.updateAll = updateAllParameter;
            this.updateSubtitles = updateSubtitlesParameter;
            this.updateDescription = updateDescription;

            this.PendingPrograms = new List<GuideEnricherProgram>();
            this.SuccessfulPrograms = new List<GuideEnricherProgram>();
            this.FailedPrograms = new List<GuideEnricherProgram>();
            this.IgnoredPrograms = new List<GuideEnricherProgram>();
        }

        public List<GuideEnricherProgram> PendingPrograms { get; set; }

        public List<GuideEnricherProgram> SuccessfulPrograms { get; set; }

        public List<GuideEnricherProgram> FailedPrograms { get; set; }

        public List<GuideEnricherProgram> IgnoredPrograms { get; set; }

        public string Title { get; set; }
        
        public int TvDbSeriesID { get; set; }
        
        public bool isRefreshed { get; set; }
        
        public bool isIgnored { get; set; }
        

        public void AddProgram(GuideEnricherProgram program)
        {
            if (!this.updateAll && program.Matched)
            {
                if (!this.IgnoredPrograms.Contains(program, guideProgramEqualityComparer))
                {
                    this.IgnoredPrograms.Add(program);  
                }
            }
            else
            {
                if (!this.PendingPrograms.Contains(program, guideProgramEqualityComparer))
                {
                    this.PendingPrograms.Add(program);
                }
            }
        }

        public void AddAllToEnrichedPrograms(GuideEnricherProgram program)
        {
            SuccessfulPrograms.Add(program);
            PendingPrograms.Remove(program);
            // only if SubTitle is NOT empty
            var similarPrograms = this.FindSimilarPrograms(this.PendingPrograms.FindAll(x => !string.IsNullOrEmpty(x.SubTitle) && x.SubTitle == program.OriginalSubTitle), program);
            if (similarPrograms.Count > 0)
            {
                PendingPrograms = new List<GuideEnricherProgram>(PendingPrograms.Except(similarPrograms));
                foreach (GuideEnricherProgram similarProgram in similarPrograms)
                {
                    similarProgram.SeriesNumber = program.SeriesNumber;
                    similarProgram.EpisodeNumber = program.EpisodeNumber;
                    similarProgram.EpisodeNumberDisplay = program.EpisodeNumberDisplay;
                    if (this.updateSubtitles)
                    {
                        similarProgram.SubTitle = program.SubTitle;
                    }

                    if (this.updateDescription)
                    {
                        similarProgram.Description = program.Description;
                    }
                }

                SuccessfulPrograms.AddRange(similarPrograms);
            }
        }

        public List<GuideEnricherProgram> FindSimilarPrograms(List<GuideEnricherProgram> pendingPrograms, GuideEnricherProgram program)
        {
            List<GuideEnricherProgram> similarPrograms = pendingPrograms;
            return similarPrograms;
        }

        public void AddAllToFailedPrograms(GuideEnricherProgram program)
        {
            FailedPrograms.Add(program);
            PendingPrograms.Remove(program);
            List<GuideEnricherProgram> similarPrograms = PendingPrograms.FindAll(x => x.SubTitle == program.OriginalSubTitle && !(string.IsNullOrEmpty(program.OriginalSubTitle)));
            if (similarPrograms.Count > 0)
            {
                PendingPrograms = new List<GuideEnricherProgram>(PendingPrograms.Except(similarPrograms));                
                FailedPrograms.AddRange(similarPrograms);
            }
        }

        public void TvDbInformationRefreshed()
        {
            this.isRefreshed = true;
            this.PendingPrograms.AddRange(this.FailedPrograms);
            this.FailedPrograms.Clear();
        }

        public override string ToString()
        {
            return this.Title;
        }
    }
}