using System.ComponentModel.DataAnnotations;

namespace OpentimelapseSort.Models
{

    internal class Preferences
    {
        [Key]
        public int Id { get; set; }

        public bool UseCopy { get; set; }

        public bool UseAutoDetectInterval { get; set; }

        public double SequenceInterval { get; set; }

        public int SequenceIntervalGenerosity { get; set; }

        public int SequenceImageCount { get; set; }

        public Preferences(bool useAutoDetectInterval, bool useCopy, double sequenceInterval, int sequenceIntervalGenerosity, int sequenceImageCount)
        {
            UseAutoDetectInterval = useAutoDetectInterval;
            UseCopy = useCopy;
            SequenceInterval = sequenceInterval;
            SequenceIntervalGenerosity = sequenceIntervalGenerosity;
            SequenceImageCount = sequenceImageCount;
            Id = 1;
        }
    }
}