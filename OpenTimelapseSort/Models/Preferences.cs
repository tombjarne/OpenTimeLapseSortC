using System.ComponentModel.DataAnnotations;

namespace OpenTimelapseSort.Models
{
    internal class Preferences
    {
        [Key]
        public int Id { get; set; } // unique Id - always 1
        public bool UseManualSettings { get; set; }
        public double SequenceInterval { get; set; }
        public int SequenceIntervalGenerosity { get; set; }
        public int SequenceImageCount { get; set; }

        public Preferences(bool useManualSettings, double sequenceInterval,
            int sequenceIntervalGenerosity, int sequenceImageCount)
        {
            UseManualSettings = useManualSettings;
            SequenceInterval = sequenceInterval;
            SequenceIntervalGenerosity = sequenceIntervalGenerosity;
            SequenceImageCount = sequenceImageCount;
            Id = 1;
        }
    }
}