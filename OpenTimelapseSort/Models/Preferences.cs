namespace OpentimelapseSort.Models
{

    class Preferences
    {

        private bool useCopy { get; set; }

        private bool useAutoDetectInterval { get; set; }

        private double sequenceInterval { get; set; }

        private int sequenceImageCount { get; set; }

        public Preferences(bool useAutoDetectInterval, bool useCopy, double sequenceInterval, int sequenceImageCount)
        {
            this.useAutoDetectInterval = useAutoDetectInterval;
            this.useCopy = useCopy;
            this.sequenceInterval = sequenceInterval;
            this.sequenceImageCount = sequenceImageCount;
        }
    }

}