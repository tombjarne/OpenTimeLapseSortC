using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OpentimelapseSort.Models
{

    class Preferences
    {
        [Key]
        public int id { get; set; }

        public bool useCopy { get; set; }

        public bool useAutoDetectInterval { get; set; }

        public double sequenceInterval { get; set; }

        public int sequenceImageCount { get; set; }

        public Preferences(bool useAutoDetectInterval, bool useCopy, double sequenceInterval, int sequenceImageCount)
        {
            this.useAutoDetectInterval = useAutoDetectInterval;
            this.useCopy = useCopy;
            this.sequenceInterval = sequenceInterval;
            this.sequenceImageCount = sequenceImageCount;
            this.id = 0;
        }

        public Preferences() { }
    }

}