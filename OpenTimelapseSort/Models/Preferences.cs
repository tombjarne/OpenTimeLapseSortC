namespace OpentimelapseSort.Models
{

    class Preferences
    {

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
        }

        public Preferences() { }
    }

}