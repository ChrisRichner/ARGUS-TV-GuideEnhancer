namespace GuideEnricher.Model
{
    using System;
    using ArgusTV.DataContracts;

    public class GuideEnricherProgram : GuideProgram, IProgramSummary
    {
        protected GuideProgram guideProgram;

        protected GuideEnricherProgram()
        {
        }

        public GuideEnricherProgram(GuideProgram guideProgram)
        {
            this.guideProgram = guideProgram;
            this.Matched = this.EpisodeIsEnriched();
            this.OriginalSubTitle = this.SubTitle;
        }

        public bool EpisodeIsEnriched()
        {
            if (!this.guideProgram.EpisodeNumber.HasValue)
            {
                return false;
            }

            if (!this.guideProgram.SeriesNumber.HasValue)
            {
                return false;
            }

            return this.EpisodeNumberDisplay.Equals(Enricher.FormatSeasonAndEpisode(this.guideProgram.SeriesNumber.Value, this.guideProgram.EpisodeNumber.Value));
        }

        public GuideProgram GuideProgram { get { return this.guideProgram; } }

        public string OriginalSubTitle { get; set; }

        public int LookupSeasonNumber { get; set; }

        public int LookupEpisodeNumber { get; set; }

        public bool Matched { get; set; }

        public bool Ignore { get; set; }

        public override string ToString()
        {
            return string.Format("{0}-{1}", this.Title, this.SubTitle);
        }

        public string CreateProgramTitle()
        {
            return this.guideProgram.CreateProgramTitle();
        }

        public string CreateEpisodeTitle()
        {
            return this.guideProgram.CreateEpisodeTitle();
        }

        public string CreateCombinedDescription(bool includeEpisodeTitle)
        {
            return this.guideProgram.CreateCombinedDescription(includeEpisodeTitle);
        }

        public Guid GetUniqueUpcomingProgramId(Guid channelId)
        {
            return this.guideProgram.GetUniqueUpcomingProgramId(channelId);
        }

        public Guid GuideProgramId
        {
            get { return this.guideProgram.GuideProgramId; }
            set { this.guideProgram.GuideProgramId = value; }
        }

        public Guid GuideChannelId
        {
            get { return this.guideProgram.GuideChannelId; }
            set { this.guideProgram.GuideChannelId = value; }
        }

        public string Title
        {
            get { return this.guideProgram.Title; }
            set { this.guideProgram.Title = value; }
        }

        public DateTime StartTime
        {
            get { return this.guideProgram.StartTime; }
            set { this.guideProgram.StartTime = value; }
        }

        public DateTime StopTime
        {
            get { return this.guideProgram.StopTime; }
            set { this.guideProgram.StopTime = value; }
        }

        public DateTime? PreviouslyAiredTime
        {
            get { return this.guideProgram.PreviouslyAiredTime; }
            set { this.guideProgram.PreviouslyAiredTime = value; }
        }

        public string SubTitle
        {
            get { return this.guideProgram.SubTitle; }
            set { this.guideProgram.SubTitle = value; }
        }

        public string Description
        {
            get { return this.guideProgram.Description; }
            set { this.guideProgram.Description = value; }
        }

        public string Category
        {
            get { return this.guideProgram.Category; }
            set { this.guideProgram.Category = value; }
        }

        public bool IsRepeat
        {
            get { return this.guideProgram.IsRepeat; }
            set { this.guideProgram.IsRepeat = value; }
        }

        public bool IsPremiere
        {
            get { return this.guideProgram.IsPremiere; }
            set { this.guideProgram.IsPremiere = value; }
        }

        public GuideProgramFlags Flags
        {
            get { return this.guideProgram.Flags; }
            set { this.guideProgram.Flags = value; }
        }

        public int? SeriesNumber
        {
            get { return this.guideProgram.SeriesNumber; }
            set { this.guideProgram.SeriesNumber = value; }
        }

        public string EpisodeNumberDisplay
        {
            get { return this.guideProgram.EpisodeNumberDisplay; }
            set { this.guideProgram.EpisodeNumberDisplay = value; }
        }

        public int? EpisodeNumber
        {
            get { return this.guideProgram.EpisodeNumber; }
            set { this.guideProgram.EpisodeNumber = value; }
        }

        public int? EpisodeNumberTotal
        {
            get { return this.guideProgram.EpisodeNumberTotal; }
            set { this.guideProgram.EpisodeNumberTotal = value; }
        }

        public int? EpisodePart
        {
            get { return this.guideProgram.EpisodePart; }
            set { this.guideProgram.EpisodePart = value; }
        }

        public int? EpisodePartTotal
        {
            get { return this.guideProgram.EpisodePartTotal; }
            set { this.guideProgram.EpisodePartTotal = value; }
        }

        public string Rating
        {
            get { return this.guideProgram.Rating; }
            set { this.guideProgram.Rating = value; }
        }

        public double? StarRating
        {
            get { return this.guideProgram.StarRating; }
            set { this.guideProgram.StarRating = value; }
        }

        public string[] Directors
        {
            get { return this.guideProgram.Directors; }
            set { this.guideProgram.Directors = value; }
        }

        public string[] Actors
        {
            get { return this.guideProgram.Actors; }
            set { this.guideProgram.Actors = value; }
        }

        public DateTime LastModifiedTime
        {
            get { return this.guideProgram.LastModifiedTime; }
            set { this.guideProgram.LastModifiedTime = value; }
        }

        public bool IsDeleted
        {
            get { return this.guideProgram.IsDeleted; }
            set { this.guideProgram.IsDeleted = value; }
        }

        public int Version
        {
            get { return this.guideProgram.Version; }
            set { this.guideProgram.Version = value; }
        }
    }

    public static class GuideProgramExtensionMethods
    {
        public static int GetValidEpisodeNumber(this GuideEnricherProgram guideProgram)
        {
            int episodeNumber;
            if (!guideProgram.EpisodeNumber.HasValue)
            {
                int.TryParse(guideProgram.SubTitle, out episodeNumber);
            }
            else
            {
                episodeNumber = guideProgram.EpisodeNumber.Value;
            }

            return episodeNumber;
        }

    }
}