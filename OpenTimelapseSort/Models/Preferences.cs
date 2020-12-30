using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OpentimelapseSort.Models
{

    class Preferences
    {
        [Key]
        public int id { get; set; }

        public bool useCopy { get; set; }

        public bool useAutoNaming { get; set; }

        public bool useAutoDetectInterval { get; set; }

        public double sequenceInterval { get; set; }

        public int sequenceImageCount { get; set; }

        public Preferences(bool useAutoDetectInterval, bool useCopy, bool useAutoNaming, double sequenceInterval, int sequenceImageCount)
        {
            this.useAutoDetectInterval = useAutoDetectInterval;
            this.useCopy = useCopy;
            this.useAutoNaming = useAutoNaming;
            this.sequenceInterval = sequenceInterval;
            this.sequenceImageCount = sequenceImageCount;
            this.id = 0;
        }

        public Preferences() { }
    }

}